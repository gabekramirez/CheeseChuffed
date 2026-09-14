using UnityEngine;

public class ager_handler : MonoBehaviour
{
    public Level_Controller level_controller;
    public GameObject rotator;
    private bool isMouseHeld = false;
    private float elapsed = 0.0f;
    public bool hasCheese = false;
    public GameObject cheese_sprite;
    private bool can_cheese = true;

    public Sprite full_sprite;
    public Sprite empty_sprite;

    public void itemDropped(GameObject item)
    {
        if (item.name == "CheesePickup" && !hasCheese)
        {
            item.transform.position = gameObject.transform.position + Vector3.back;
            hasCheese = true;
            cheese_sprite.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<SpriteRenderer>().sprite = full_sprite;
        }
    }

    public void removeCheese()
    {
        
        cheese_sprite.GetComponent<SpriteRenderer>().enabled = true;
        cheese_sprite.transform.position = gameObject.transform.position;
        LeverUnHold();
        gameObject.GetComponent<SpriteRenderer>().sprite = empty_sprite;
        can_cheese = false;
    }

    public void LeverHold()
    {
        if (hasCheese)
        {
            isMouseHeld = true;
            elapsed = 0.0f; 
        }
        
    }

    public void LeverUnHold()
    {
        isMouseHeld = false;
    }

    void OnMouseDown(){
        if (can_cheese && hasCheese){
            level_controller.remove_cheese();
            
        }
        
    }

    void Update()
    {
        if (isMouseHeld && can_cheese)
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
