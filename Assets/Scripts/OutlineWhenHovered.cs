using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor.UI;
using UnityEngine;

//[RequireComponent()]
public class OutlineWhenHovered : MonoBehaviour
{
    private Material material;

    public void Awake() {
        material = GetComponent<Renderer>().material;
    }

    public void OnMouseEnter() {
        material.SetInt("_Hovered", 1);
    }

    public void OnMouseExit() {
        material.SetInt("_Hovered", 0);
    }
}