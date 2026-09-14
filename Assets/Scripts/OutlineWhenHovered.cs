using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.UI;
using UnityEngine;

//[RequireComponent()]
public class OutlineWhenHovered : MonoBehaviour
{
    private Material material;

    public void Awake() {
        material = GetComponent<Renderer>().material;
    }

    public void HoverOn() {
        material.SetInt("_Hovered", 1);
    }

    public void HoverOff() {
        material.SetInt("_Hovered", 0);
    }
}