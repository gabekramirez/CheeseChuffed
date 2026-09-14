using UnityEngine;

public class ager_handler : MonoBehaviour
{
    public Level_Controller level_controller;
    public GameObject rotator;
    private bool isMouseHeld = false;
    private float elapsed = 0.0f;
    private bool hasCheese = false;
    public GameObject cheese_sprite;

    public void itemDropped(GameObject item)
    {
        if (item.name == "CheesePickup" && !hasCheese)
        {
            item.transform.position = gameObject.transform.position + Vector3.back;
            hasCheese = true;
            cheese_sprite.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void removeCheese()
    {
        
        cheese_sprite.GetComponent<SpriteRenderer>().enabled = true;
        cheese_sprite.transform.position = gameObject.transform.position;
        OnMouseUp();
        
    }

    public void OnMouseDown()
    {
        if (hasCheese)
        {
            isMouseHeld = true;
            elapsed = 0.0f; 
        }
        
    }

    public void OnMouseUp()
    {
        isMouseHeld = false;
    }

    void Update()
    {
        if (isMouseHeld)
        {
            elapsed += Time.deltaTime;
            if (elapsed >= 1.0f)
            {
                elapsed = 0.0f;
                level_controller.add_ingredient(2);
                rotator.transform.eulerAngles -= new Vector3(0, 0, 36);
            }
        }
    }

}
