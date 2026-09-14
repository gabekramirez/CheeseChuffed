using UnityEngine;

public class Lever_Control : MonoBehaviour
{
    public ager_handler Ager;
    public Sprite Held_Sprite;
    private Sprite starter_sprite;

    void Awake(){
        starter_sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
    }
    void OnMouseDown()
    {
        if (Ager.hasCheese){
            //swap sprites
            Ager.LeverHold();
            gameObject.SendMessage("HoverOff");
            gameObject.GetComponent<SpriteRenderer>().sprite = Held_Sprite;
        }
        
    }
    void OnMouseUp()
    {
        if (Ager.hasCheese){
            //swap sprites
            Ager.LeverUnHold();
            gameObject.SendMessage("HoverOn");
            gameObject.GetComponent<SpriteRenderer>().sprite = starter_sprite;
        }
        
    }
}
