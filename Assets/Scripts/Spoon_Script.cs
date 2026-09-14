using System.Threading.Tasks;
using UnityEngine;

public class Spoon_Script : MonoBehaviour
{
    private bool isStirring = false;

    private float stir_distance = 0.0f;
    private float previous_frame_x = 0.0f;

    private GameObject level_controller;
    void Awake()
    {
        MonoBehaviour[] allScripts = FindObjectsByType<MonoBehaviour>();
        for (int i = 0; i < allScripts.Length; i++)
        {
           if(allScripts[i] is Level_Controller)
            {
                level_controller = allScripts[i].gameObject;
            }
               
        }
    }
    void OnCollisionEnter2D(Collision2D collider)
    {
        
        if (collider.gameObject.name == "Pot")
        {
            isStirring = true;
        }
    }
    void OnCollisionExit2D(Collision2D collider)
    {
        if (collider.gameObject.name == "Pot")
        {
            isStirring = false;
        }
    }

    void Update()
    {
        if (isStirring)
        {
            stir_distance += Mathf.Abs(gameObject.transform.position.x - previous_frame_x);
            previous_frame_x = gameObject.transform.position.x;
            if (stir_distance > 50.0f)
            {
                print("Stir Goal met!");
                stir_distance = 0.0f;
                level_controller.SendMessage("ready_cheese", new Vector3(0,0,1));
                gameObject.SendMessage("OnMouseUp");
                
            }
        }
        
    }

    

}
