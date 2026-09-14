using UnityEngine;

public class Lever_Control : MonoBehaviour
{
    public ager_handler Ager;
    public Sprite Held_Sprite;
    private Sprite starter_sprite;
    public UIAudio uIAudio;

    void Awake(){
        starter_sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    void OnMouseEnter(){
        gameObject.SendMessage("HoverOn");
    }
    void OnMouseExit(){
        gameObject.SendMessage("HoverOff");
    }

    void OnMouseDown()
    {
        if (Ager.hasCheese){
            //swap sprites
            Ager.LeverHold();
            gameObject.SendMessage("HoverOff");
            gameObject.GetComponent<SpriteRenderer>().sprite = Held_Sprite;
            Ager.SendMessage("HoverOff");
            uIAudio.PlayAudio("Lever");
        }
        
    }
    void OnMouseUp()
    {
        if (Ager.hasCheese){
            //swap sprites
            Ager.LeverUnHold();
            gameObject.SendMessage("HoverOn");
            gameObject.GetComponent<SpriteRenderer>().sprite = starter_sprite;
            Ager.SendMessage("HoverOn");
        }
        
    }
}
