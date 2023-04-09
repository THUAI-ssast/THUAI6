using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombView : MonoBehaviour
{
    // TODO: to be decided
    public static float ExplosionDuration = 1.0f;

    private SpriteRenderer sr;
    private Animator animator;
    public Sprite explosionSprite;



    // TODO: to be implemented

    void Awake(){
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void OnExplode()
    {
        sr.sprite=explosionSprite;
        animator.enabled=true;
    }
}
