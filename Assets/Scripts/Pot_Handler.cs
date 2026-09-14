using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class Pot_Handler : MonoBehaviour
{

    public Level_Controller level_controller;
    public UIAudio uIAudio;
    public Sprite full_icon;
    public bool has_milk = false;
    
    public void itemDropped(GameObject item)
    {
        print(item.name);
        if ((item.name.Contains("Salt") || item.name.Contains("Bacteria")) && has_milk)
        {
            Destroy(item);
            if (item.name.Contains("Salt"))
            {
                level_controller.add_ingredient(0);
                uIAudio.PlayAudio("Splash");
            }
            else
            {
                level_controller.add_ingredient(1);
                uIAudio.PlayAudio("Splash");
            }
        }else if (item.name == "Milk"){
            Destroy(item);
            gameObject.GetComponent<SpriteRenderer>().sprite = full_icon;
            has_milk = true;
            uIAudio.PlayAudio("Splash");
            uIAudio.PlayLoopingAudio("Bubble");
        }
    }

}
