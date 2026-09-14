using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class Pot_Handler : MonoBehaviour
{

    public Level_Controller level_controller;
    public Sprite full_icon;

    public void itemDropped(GameObject item)
    {
        //print(item.name);
        if (item.name.Contains("Salt") || item.name.Contains("Bacteria"))
        {
            Destroy(item);
            if (item.name.Contains("Salt"))
            {
                level_controller.add_ingredient(0);
            }
            else
            {
                level_controller.add_ingredient(1);
            }
        }else if (item.name == "Milk"){
            Destroy(item);
            gameObject.GetComponent<SpriteRenderer>().sprite = full_icon;
        }
    }

}
