using UnityEngine;

public class Level_Controller : MonoBehaviour
{

    private bool isCameraMoving = false;
    private float camera_elapsed = 0.0f;
    public DraggableClass cheese_pickup;
    public ager_handler age_machine;

    private int cheese_level = 0;
    private int bac_level = 0;
    private int age_level = 0;

    public int current_screen = 0;    

    public void add_ingredient(int type)
    {
        if (type == 0)
        {
            cheese_level++;
        }else if (type == 1)
        {
            bac_level++;
        }
        else
        {
            age_level++;
            if (age_level >= 10)
            {
                age_machine.removeCheese();
                ready_cheese(cheese_pickup.initial_position);
            }
        }
    }

    public void remove_cheese(){
        age_machine.removeCheese();
        ready_cheese(cheese_pickup.initial_position);
        move_camera();
    }


    public void ready_cheese(Vector3 newPosition)
    {
        //cheese_pickup.transform.position = new Vector3(0,0,1);
        cheese_pickup.enabled = true;
        cheese_pickup.initial_position = newPosition;
        
        cheese_pickup.isHeld = true;
        cheese_pickup.SendMessage("OnMouseUp");

    }

    public void move_camera()
    {
        current_screen++;
        camera_elapsed = 0.0f;
        isCameraMoving = true;
        cheese_pickup.initial_position = new Vector3(20, 0, 1);
        print(cheese_level);
        print(bac_level);
        print(age_level);
    }

    void Update()
    {
        if (isCameraMoving)
        {
            camera_elapsed += Time.deltaTime;
            if (camera_elapsed >= 1.0f)
            {
                Camera.main.transform.position = new Vector3(Mathf.Lerp(30.0f * (current_screen - 1), 30.0f * current_screen, 1 - Mathf.Pow(1 - (camera_elapsed - 1.0f), 3)), 0, -10);
                if (camera_elapsed >= 2.0f)
                {
                    Camera.main.transform.position = new Vector3(30 * current_screen, 0, -10);
                    isCameraMoving = false;
                }
            }
            
        }
    }
}
