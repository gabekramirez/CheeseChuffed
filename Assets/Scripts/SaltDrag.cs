using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor.UI;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SaltDrag : MonoBehaviour
{
    private bool infront = false;

    private bool dragging = false;
    private Vector2 dragOffset;

    private SpriteRenderer spriteRenderer;
    ParticleSystem saltParticles;
    private Vector2 potPosition;
    private Vector2 potSize;
    private bool overPot = false;
    const float rotationTime = 0.2f;  // in seconds
    private float saltTime = 0.0f;
    private bool salting = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        saltParticles = GameObject.Find("Salt Particles").GetComponent<ParticleSystem>();
    }

    private void Update() {
        // handle z ordering
        float frontLayer = GameObject.Find("Pot").GetComponent<PotDrag>().frontLayer;
        if (dragging) {
            if (!infront) {
                frontLayer -= 1;
                transform.position = new Vector3(transform.position.x, transform.position.y, frontLayer);
            }
            infront = true;
        } else {
            infront = false;
        }

        // handle rotation
        if (overPot) {
            Quaternion target = Quaternion.Euler(0, 0, 180);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime / rotationTime);
        } else {
            Quaternion target = Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime / rotationTime);
        }

        // hande particles
        if (overPot && saltTime > 0) {
            if (!salting) {
                saltParticles.Play();
                salting = true;
            }
            saltTime -= Time.deltaTime;
        } else {
            if (salting) {
                saltParticles.Stop();
                salting = false;
            }
        }
     }

    public void OnMouseDrag() {
        Quaternion previousRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(0, 0, 0);

        potPosition = GameObject.Find("Pot").transform.position;
        potSize = GameObject.Find("Pot").GetComponent<SpriteRenderer>().bounds.size;

        saltTime = 0.1f;
        Vector2 relativeMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        if (!dragging) {
            dragging = true;
            dragOffset = -relativeMousePos;
        }
        transform.Translate(relativeMousePos + dragOffset);
        float x = transform.position.x - potPosition.x;
        float y = transform.position.y - potPosition.y;
        float w = (potSize.x + spriteRenderer.bounds.size.x) * 0.5f;
        float h = potSize.y * 0.5f;
        overPot = -w <= x && x <= w && y > h;
        transform.rotation = previousRotation;
    }

    public void OnMouseUp() {
        dragging = false;
        overPot = false;
    }
}
