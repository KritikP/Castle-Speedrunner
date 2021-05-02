using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy_Handler : MonoBehaviour, IDamagable
{
    protected int health;
    protected float moveSpeed;
    protected int attackDamage;
    
    protected Animator animator;
    protected SpriteRenderer spriteData;
    protected Rigidbody2D body2d;
    protected Collider2D[] colliders;

    protected ContactFilter2D playerFilter;
    protected float fade = 1f;
    protected int playerMask;
    protected int groundMask;
    
    //State data
    protected int walkDirection;
    protected bool canWalkLeft;
    protected bool canWalkRight;
    protected bool canMove;
    protected bool dead;

    [SerializeField] protected float pauseMovementTime = 1f;
    [SerializeField] protected CollisionSensor leftWalkSensor;
    [SerializeField] protected CollisionSensor rightWalkSensor;
    [SerializeField] protected AttackSensor attackHitbox;
    [SerializeField] protected Enemy_Data enemyData;
    [SerializeField] protected Player_Data playerData;
    [SerializeField] private Map_Data mapData;

    private int room;

    public void TakeDamage(int damage)
    {
        health -= damage;
        animator.SetTrigger("Hurt");
    }

    protected void CheckForDeath()
    {
        if (health <= 0)
        {
            //Debug.Log("Dead");
            dead = true;
            fade = fade - Time.deltaTime * 0.3f;
            gameObject.layer = 9;   //Dead enemies layer
            animator.SetTrigger("Death");
            
            Color c = spriteData.color;
            c.a = fade;
            spriteData.color = c;
            if (fade <= 0f)
                Destroy(gameObject);
        }
    }

    public virtual void StartAttack()
    {
        if (!dead)
        {
            animator.SetTrigger("Attack 1");
            canMove = false;
        }
    }

    public virtual void Attack()
    {
        if (!dead)
        {
            List<Collider2D> hitColliders = new List<Collider2D>();
            attackHitbox.GetComponent<Collider2D>().OverlapCollider(playerFilter, hitColliders);

            bool didHit = false;
            foreach (Collider2D c in hitColliders)
            {
                if (c.GetComponent<IDamagable>() != null && !didHit)
                {
                    c.GetComponent<IDamagable>().TakeDamage(attackDamage);
                    didHit = true;
                }
            }
            hitColliders.Clear();
        }
    }

    public void StartMoving()
    {
        canMove = true;
    }

    public void StopMoving()
    {
        canMove = false;
    }

    protected void InitEnemy()
    {
        room = mapData.currentRoom;
        groundMask = 11;
        playerMask = (int)Mathf.Log(playerData.playerLayer.value, 2f);
        dead = false;
        walkDirection = -1;
        canMove = true;

        animator = GetComponent<Animator>();
        spriteData = GetComponent<SpriteRenderer>();
        colliders = GetComponents<Collider2D>();
        body2d = GetComponent<Rigidbody2D>();

        health = enemyData.maxHealth;
        moveSpeed = enemyData.moveSpeed;
        attackDamage = enemyData.attackDamage;

        playerFilter = new ContactFilter2D();
        playerFilter.SetLayerMask(playerData.playerLayer);
        leftWalkSensor.layerMask = groundMask;
        rightWalkSensor.layerMask = groundMask;
        attackHitbox.layerMask = playerMask;

        if (moveSpeed <= 0)
        {
            Debug.Log("Move speed must be greater than 0; setting to default 5.");
            moveSpeed = 5f;
        }
    }

    protected virtual void Movement()
    {
        if (canMove && !dead)
        {
            if (leftWalkSensor.isCollision())
            {
                canWalkLeft = true;
            }
            else
            {
                canWalkLeft = false;
            }

            if (rightWalkSensor.isCollision())
            {
                canWalkRight = true;
            }
            else
            {
                canWalkRight = false;
            }

            if (!canWalkLeft && !canWalkRight)
            {
                Debug.Log("Enemy stuck");
                animator.SetBool("Walking", false);
                walkDirection = 0;
            }
            else if (walkDirection == -1 && canWalkLeft)     //If walking left and can keep walking left, then keep walking left
            {
                animator.SetBool("Walking", true);

            }
            else if (walkDirection == -1 && !canWalkLeft)    //If walking left and can't keep walking left, then start walking right
            {
                canMove = false;
                animator.SetBool("Walking", false);
                StartCoroutine(PauseMovementRoutine());
                
            }
            else if (walkDirection == 1 && canWalkRight)     //If walking right and can keep walking right, then keep walking right
            {
                animator.SetBool("Walking", true);

            }
            else    //If walking right and can't keep walking right, then start walking left
            {
                canMove = false;
                animator.SetBool("Walking", false);
                StartCoroutine(PauseMovementRoutine());
                walkDirection = -1;
                spriteData.flipX = true;
                animator.SetBool("Walking", true);
            }

            body2d.velocity = new Vector2(walkDirection * moveSpeed, body2d.velocity.y);

            if (spriteData.flipX)
            {
                attackHitbox.gameObject.GetComponent<PolygonCollider2D>().transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                attackHitbox.gameObject.GetComponent<PolygonCollider2D>().transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }
        else
        {
            body2d.velocity = new Vector2(0, body2d.velocity.y);
        }
    }

    /*
    protected void PauseMovement()
    {
        canMove = false;
        animator.SetBool("Walking", false);
        StartCoroutine(PauseMovementRoutine());
    }
    */

    private IEnumerator PauseMovementRoutine()
    {
        yield return new WaitForSeconds(1f);
        canMove = true;
        walkDirection = 1;
        spriteData.flipX = false;
        animator.SetBool("Walking", true);
    }

}
