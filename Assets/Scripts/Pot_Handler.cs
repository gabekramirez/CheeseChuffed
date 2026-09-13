using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]

public class Pot_Handler : MonoBehaviour
{
   public void itemDropped(GameObject item)
    {
<<<<<<< Updated upstream
        print(item.name);
        if (item.name.StartsWith("Salt"))
=======
        //print(item.name);
        if (item.name.Contains("Salt") || item.name.Contains("Bacteria"))
>>>>>>> Stashed changes
        {
            Destroy(item);
        }
    }

}
