using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class Pot_Handler : MonoBehaviour
{
   public void itemDropped(GameObject item)
    {
        //print(item.name);
        if (item.name.Contains("Salt") || item.name.Contains("Bacteria"))
        {
            Destroy(item);
        }
    }

}
