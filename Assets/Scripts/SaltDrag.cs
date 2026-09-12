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
    private Vector3 homePosition;
    const float positionSmooth = 5;
    const float rotationSmooth = 5;

    private SpriteRenderer spriteRenderer;
    ParticleSystem saltParticles;
    private Vector2 potPosition;
    private Vector2 potSize;
    private bool overPot = false;
    private float saltTime = 0.0f;
    private bool salting = false;

    private void Awake()
    {
        homePosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        saltParticles = GameObject.Find("Salt Particles").GetComponent<ParticleSystem>();
    }

    private void Update() {
        // handle z ordering
        if (dragging) {
            if (!infront) {
                GameObject.Find("Pot").GetComponent<PotDrag>().frontLayer -= 1.0f;
                float frontLayer = GameObject.Find("Pot").GetComponent<PotDrag>().frontLayer;
                transform.position = new Vector3(transform.position.x, transform.position.y, frontLayer);
            }
            infront = true;
        } else {
            infront = false;
            transform.position = Vector3.Slerp(transform.position, homePosition, Time.deltaTime * positionSmooth);
        }

        // handle rotation
        if (overPot) {
            Quaternion target = Quaternion.Euler(0, 0, 180);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSmooth);
        } else {
            Quaternion target = Quaternion.Euler(0, 0, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSmooth);
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
