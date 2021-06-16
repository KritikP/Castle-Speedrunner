using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Boss : Enemy_Handler
{
    [SerializeField] private GameObject sensors;
    [SerializeField] private Collider2D[] hitboxesColliders;
    [SerializeField] private Collider2D bodyCollider;
    private HeroKnight player;

    protected override void InitEnemy()
    {
        base.InitEnemy();
        player = FindObjectOfType<HeroKnight>();
        hitboxesColliders = hitboxes.GetComponentsInChildren<Collider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        if (!animator.GetBool("DEBUG_canMove"))
        {
            canMove = false;
        }
        Movement();
        CheckForDeath();
    }

    public void StartBaseAttack()
    {
        if (!dead)
        {
            body2d.gravityScale = 0;
            animator.SetTrigger("Base Attack");
            canMove = false;
        }
    }

    public void StartSpinAttack()
    {
        if (!dead)
        {
            body2d.gravityScale = 0;
            animator.SetTrigger("Spin Attack");
            canMove = false;
        }
    }

    public void StartJumpAttack()
    {
        if (!dead)
        {
            body2d.gravityScale = 0;
            animator.SetTrigger("Jump Attack");
            canMove = false;
        }
    }

    public void StartLeapAttack()
    {
        if (!dead)
        {
            body2d.gravityScale = 0;
            animator.SetTrigger("Leap Attack");
            canMove = false;
        }
    }

    public void AE_ResetPosition()
    {
        gameObject.transform.position = bodyCollider.transform.position;
        bodyCollider.transform.localPosition = Vector3.zero;
    }

    public void AE_EnableCollider(int hitbox)
    {
        hitboxesColliders[hitbox].gameObject.SetActive(true);
    }

    public void AE_DisableCollider(int hitbox)
    {
        hitboxesColliders[hitbox].gameObject.SetActive(false);
    }

    public void AE_Attack(int hitbox)
    {
        List<Collider2D> hitColliders = new List<Collider2D>();
        hitboxesColliders[hitbox].OverlapCollider(playerFilter, hitColliders);

        foreach (Collider2D c in hitColliders)
        {
            if (c.GetComponent<IDamagable>() != null)
            {
                c.GetComponent<IDamagable>().TakeDamage(attackDamage);
                break;
            }
        }
        hitColliders.Clear();
    }

    public void AE_ResetAttackSensor(int hitbox)
    {
        body2d.gravityScale = 1;
        animator.SetBool("Attacking", false);
        //Debug.Log(hitboxesColliders[hitbox].name);
        if (hitbox >= 0 && hitboxesColliders.Length > hitbox)
            hitboxesColliders[hitbox].GetComponent<AttackSensor>()?.ResetAttackingState();
        else
            Debug.LogError("Invalid hitbox index chosen for " + name);
    }

    public void AE_StartAttack()
    {
        animator.SetBool("Attacking", true);
    }

    public void AttackRandomGround()
    {
       if(rand.Next(0, 2) == 0)
       {
            StartBaseAttack();
       }
       else
       {
            StartSpinAttack();
       }
    }

    public void AttackRandomJump()
    {
        if (rand.Next(0, 2) == 0)
        {
            StartLeapAttack();
        }
        else
        {
            StartJumpAttack();
        }
    }

    protected override void Movement()
    {
        if (canMove && !dead)
        {
            canWalkLeft = leftWalkSensor.isCollision() && !leftWallSensor.isCollision();
            canWalkRight = rightWalkSensor.isCollision() && !rightWallSensor.isCollision();

            int playerDirection = 0;
            float xDistance = player.transform.position.x - transform.position.x;
            float yDistance = player.transform.position.y - transform.position.y;

            if (xDistance <= 3 && xDistance >= -3)
            {
                StartSpinAttack();
            }
            else if(xDistance < -3)
            {
                playerDirection = -1;
            }
            else if (xDistance > 3)
            {
                playerDirection = 1;
            }

            if ((!canWalkLeft && !canWalkRight))
            {
                //Debug.Log("Enemy stuck");
                animator.SetBool("Walking", false);
                walkDirection = 0;
            }
            else if (canWalkLeft && playerDirection == -1)
            {
                walkDirection = -1;
            }
            else if(canWalkRight && playerDirection == 1)
            {
                walkDirection = 1;
            }
            else
            {
                animator.SetBool("Walking", true);
                if (walkDirection == 0)
                {
                    if (canWalkRight)
                        walkDirection = 1;
                    else
                        walkDirection = -1;
                }
                //Else, keep going
            }

            body2d.velocity = new Vector2(walkDirection * moveSpeed, body2d.velocity.y);

            if (!spriteData.flipX)
            {
                hitboxes.transform.eulerAngles = new Vector3(0, 0, 0);
                sensors.transform.eulerAngles = new Vector3(0, 0, 0);

            }
            else
            {
                hitboxes.transform.eulerAngles = new Vector3(0, 180, 0);
                sensors.transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }
        else
        {
            body2d.velocity = new Vector2(0, body2d.velocity.y);
        }

        if (walkDirection == 1)
        {
            spriteData.flipX = false;
        }
        else if (walkDirection == -1)
        {
            spriteData.flipX = true;
        }

    }

}