using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotDrag : MonoBehaviour
{
    public float frontLayer = 0.0f;
    private bool infront = false;

    private bool dragging = false;
    private Vector2 dragOffset;

    private void Update() {
        // handle z ordering
        if (dragging) {
            if (!infront) {
                frontLayer -= 1;
                transform.position = new Vector3(transform.position.x, transform.position.y, frontLayer);
            }
            infront = true;
        } else {
            infront = false;
        }
     }

    public void OnMouseDrag() {
        Vector2 relativeMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        if (!dragging)
        {
            dragging = true;
            dragOffset = -relativeMousePos;
        }
        transform.Translate(relativeMousePos + dragOffset);
    }

    public void OnMouseUp()  {
        dragging = false;
    }

    private void OnTriggerEnter2D(Collider2D other)  {
        Debug.Log("collision "+other.name);
    }
}
