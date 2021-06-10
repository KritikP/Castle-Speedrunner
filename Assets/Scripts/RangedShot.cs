using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedShot : MonoBehaviour
{
    private Animator animator;
    private Collider2D attackHitbox;
    private Rigidbody2D body2d;
    public LayerMask attackingLayerMask;
    public LayerMask groundLayerMask;
    public Player_Data playerData;

    [HideInInspector] public int damage = 10;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        attackHitbox = GetComponent<Collider2D>();
        body2d = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (1 << other.gameObject.layer == attackingLayerMask)
        {
            if (other.GetComponent<IDamagable>() != null && !playerData.invincible)
            {
                other.GetComponent<IDamagable>().TakeDamage(damage);
                animator.SetTrigger("Hit");
                body2d.velocity = new Vector2(0, 0);
            }
        }
        else if (1 << other.gameObject.layer == groundLayerMask)
        {
            animator.SetTrigger("Hit");
            body2d.velocity = new Vector2(0, 0);
        }
    }

    private void DestroyShot()
    {
        Destroy(gameObject);
    }
}