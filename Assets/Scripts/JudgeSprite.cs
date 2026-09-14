using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using Unity.Mathematics;

public class JudgeSprite : MonoBehaviour
{
    public bool eating = false;
    private float eatTime = 0.0f;
    private float startY;

    private void Awake()
    {
        startY = transform.position.y;
    }

    private void Update() {
        if (eating || math.abs(transform.position.y - startY) > 0.1f) {
            eatTime += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.01f * (float)Math.Sin(eatTime * 10.0f), transform.position.z);
        } else {
            transform.position = new Vector3(transform.position.x, startY, transform.position.z);
        }
     }
}
