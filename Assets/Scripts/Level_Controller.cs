using UnityEngine;

public class Level_Controller : MonoBehaviour
{

    private bool isCameraMoving = false;
    private float camera_elapsed = 0.0f;
    public DraggableClass cheese_pickup;

    private int cheese_level = 0;
    private int bac_level = 0;
    private int age_level = 0;

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
        }
    }
    public void ready_cheese()
    {
        //cheese_pickup.transform.position = new Vector3(0,0,1);
        cheese_pickup.initial_position = new Vector3(0, 0, 1);
        cheese_pickup.isHeld = true;
        cheese_pickup.SendMessage("OnMouseUp");
    }

    public void move_camera()
    {

        isCameraMoving = true;
        cheese_pickup.initial_position = new Vector3(20, 0, 1);
    }

    void Update()
    {
        if (isCameraMoving)
        {
            camera_elapsed += Time.deltaTime;
            if (camera_elapsed >= 1.0f)
            {
                Camera.main.transform.position = new Vector3(Mathf.Lerp(0.0f, 30.0f, 1 - Mathf.Pow(1 - (camera_elapsed - 1.0f), 3)), 0, -10);
                if (camera_elapsed >= 2.0f)
                {
                    Camera.main.transform.position = new Vector3(30, 0, -10);
                    isCameraMoving = false;
                }
            }
            
        }
    }
}
