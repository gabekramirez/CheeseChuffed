using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor.UI;
using UnityEngine;



[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]


public class DraggableClass : MonoBehaviour
{

    public Vector3 initial_position;
    public bool isHeld = false;
    private bool isTracing = false;

    private bool isReturning = false;
    private GameObject current_collider;

    public float zRot = 0.0f;

    const float MAX_DISTANCE = 25.0f;
    const float MAX_SPEED = 400.0f;
    public Texture2D hand_icon;
    public Texture2D grip_icon;
    Vector3 mouse_position;

    public OutlineWhenHovered highlight;

    private Level_Controller level_controller;
    void Awake()
    {
        initial_position = gameObject.transform.position;
        MonoBehaviour[] allScripts = FindObjectsByType<MonoBehaviour>();
        for (int i = 0; i < allScripts.Length; i++)
        {
           if(allScripts[i] is Level_Controller)
            {
                level_controller = allScripts[i] as Level_Controller;
            }
               
        }
    }

    void OnMouseOver()
    {
        if (!isHeld)
        {
           gameObject.SendMessage("HoverOn"); 
        }
        
    }
    void OnMouseExit()
    {
        if (!isHeld)
        {
           gameObject.SendMessage("HoverOff"); 
        }
        
    }
    public void OnMouseDown()
    {
        Cursor.SetCursor(grip_icon, new Vector2(16,16), CursorMode.Auto);
        isHeld = true;
        gameObject.transform.localScale = Vector3.one * 1.1f;
        gameObject.transform.eulerAngles = Vector3.forward * (10f + zRot);
        gameObject.SendMessage("HoverOff");
        if (highlight)
        {
            highlight.HoverOn();
        }
        if (gameObject.name == "CheesePickup" && level_controller.current_screen == 0)
        {
            level_controller.SendMessage("move_camera");
        }
    }

    public void OnMouseUp()
    {
        if (!isHeld){
            return;
        }
        Cursor.SetCursor(hand_icon, new Vector2(16f,16f), CursorMode.Auto);
        isHeld = false;
        if ((gameObject.transform.position - mouse_position).magnitude > 0.0f)
        {
            isTracing = true;
            mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;
        }
        else if (isHeld)
        {
            onDrop();
        }
        if (highlight)
        {
            highlight.HoverOff();
        }
        gameObject.transform.localScale = Vector3.one;
        gameObject.transform.eulerAngles = new Vector3(0,0,zRot);
    }

    void Update()
    {
        if (!isHeld && !isTracing && !isReturning)
        {
            
            return;
        }
        Vector3 frame_position = gameObject.transform.position;
        if (!isTracing)
        {
           mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
           mouse_position.z += 10.0f;
        }
        
        Vector3 goal_distance;
        if (isReturning)
        {
            goal_distance = initial_position - frame_position;
        }
        else
        {
            goal_distance = mouse_position - frame_position;
        }
        
        
        Vector3 frame_direction = goal_distance.normalized * Math.Clamp(goal_distance.magnitude/MAX_DISTANCE, 0, 1) * MAX_SPEED * Time.deltaTime;
        frame_position += frame_direction;

        if ((isTracing) && goal_distance.magnitude < .1f)
        {
            frame_position = mouse_position;
            isTracing = false;
            onDrop();
        }else if (isReturning && goal_distance.magnitude < .1f)
        {
            frame_position = initial_position;
            isReturning = false;
        }

        gameObject.transform.position = frame_position;
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        print("collided on main script");
        current_collider = collider.gameObject;
        
    }
    void OnCollisionExit2D(Collision2D collider)
    {
        if (current_collider == collider.gameObject)
        {
            current_collider = null;
        }
    }

    void onDrop()
    {
        //will add complexity later
        isReturning = true;
        
        
        
        if (current_collider != null && current_collider.tag == "Interactable")
        {
            current_collider.SendMessage("itemDropped", gameObject);

        }
        
    }

}
