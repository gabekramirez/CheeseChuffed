using UnityEngine;

public class Lever_Control : MonoBehaviour
{
    public ager_handler Ager;
    void OnMouseDown()
    {
        if (Ager.hasCheese){
            //swap sprites
            Ager.LeverHold();
        }
        
    }
    void OnMouseUp()
    {
        if (Ager.hasCheese){
            //swap sprites
            Ager.LeverUnHold();
        }
        
    }
}
