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
    private Vector3 initial_position;
    private bool isHeld = false;
    private bool isTracing = false;
    private bool isReturning = false;
    
    // Updated: Now storing Collider2D instead of the Collision2D event
    public List<Collider2D> current_colliders = new List<Collider2D>();

    const float MAX_DISTANCE = 25.0f;
    const float MAX_SPEED = 450.0f; // units per second (tune to taste)

    Vector3 mouse_position;

    void Awake()
    {
        initial_position = gameObject.transform.position;
    }

    public void OnMouseDown()
    {
        isHeld = true;
        gameObject.transform.localScale = Vector3.one * 1.1f;
        gameObject.transform.eulerAngles = Vector3.forward * 10f;
    }

    public void OnMouseUp()
    {
        isHeld = false;
        if ((gameObject.transform.position - mouse_position).magnitude > 0.0f)
        {
            isTracing = true;
            mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector3.forward * 10;
        }
        else
        {
            onDrop();
        }
        gameObject.transform.localScale = Vector3.one;
        gameObject.transform.eulerAngles = Vector3.zero;
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
        }
        else if (isReturning && goal_distance.magnitude < .1f)
        {
            frame_position = initial_position;
            isReturning = false;
        }

        gameObject.transform.position = frame_position;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Updated: Store the specific collider we hit
        if (!current_colliders.Contains(collision.collider))
        {
            current_colliders.Add(collision.collider);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Updated: Remove the specific collider we just stopped touching
        current_colliders.Remove(collision.collider);
    }

    void onDrop()
    {
        //will add complexity later
        isReturning = true;

        // Updated: Iterate through the stored colliders
        foreach (Collider2D col in current_colliders)
        {
            // Updated: Check the tag of the object we actually collided with
            if (col.gameObject.CompareTag("Interactable"))
            {
                Debug.Log(col.gameObject.tag);
                col.gameObject.SendMessage("itemDropped", gameObject);
            }
        }
    }
}