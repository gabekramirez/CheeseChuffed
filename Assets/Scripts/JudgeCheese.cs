using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class JudgeCheese : MonoBehaviour
{
    public bool eating = false;
    private bool playingParticles = false;

    private SpriteRenderer spriteRenderer;
    private ParticleSystem cheeseParticles;

    public void SetActive(bool active)
    {
        spriteRenderer.enabled = active;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cheeseParticles = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    private void Update() {
        if (eating) {
            if (!playingParticles) {
                cheeseParticles.Play();
                playingParticles = true;
            }
        } else {
            if (playingParticles) {
                cheeseParticles.Stop();
                playingParticles = false;
            }
        }
     }
}
