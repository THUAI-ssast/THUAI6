using UnityEngine;

public class BombView : MonoBehaviour
{
    public static float ExplosionDuration = 1.0f;

    private SpriteRenderer sr;
    private Animator animator;
    public Sprite explosionSprite;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void OnExplode()
    {
        sr.sprite = explosionSprite;
        animator.enabled = true;
    }
}
