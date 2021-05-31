using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_FireWorm : Enemy_Handler
{
    [SerializeField] private float fireballSpeed = 5f;
    [SerializeField] private Transform fireballSpawnPosition;

    public override void Attack()
    {
        GameObject fireball = Instantiate(Resources.Load<GameObject>("Prefabs/Ranged Attacks/Fireball"), fireballSpawnPosition.position, Quaternion.identity);
        fireball.GetComponent<RangedShot>().damage = attackDamage;
        if (spriteData.flipX)
        {
            fireball.GetComponent<Rigidbody2D>().velocity = new Vector2(-fireballSpeed, 0);
            fireball.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
            fireball.GetComponent<Rigidbody2D>().velocity = new Vector2(fireballSpeed, 0);
    }

    void Start()
    {
        InitEnemy();
    }
    
    void Update()
    {
        Movement();
        CheckForDeath();
        
    }
}
