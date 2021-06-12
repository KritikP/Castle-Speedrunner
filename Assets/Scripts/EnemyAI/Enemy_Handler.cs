using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy_Handler : MonoBehaviour, IDamagable
{
    protected int health;
    protected float moveSpeed;
    protected int attackDamage;
    protected ObjectPooler objectPooler;
    protected Animator animator;
    protected SpriteRenderer spriteData;
    protected Rigidbody2D body2d;
    protected Collider2D[] colliders;
    private GameObject[] powerUpsList;

    protected ContactFilter2D playerFilter;
    protected float fade = 1f;
    
    //State data
    protected int walkDirection;
    protected bool canWalkLeft;
    protected bool canWalkRight;
    protected bool canMove;
    protected bool dead = false;

    [SerializeField] protected int droppedCoins = 5;
    [SerializeField] protected float pauseMovementTime = 1f;
    [SerializeField] protected float pauseAttackTime = 0.2f;
    [SerializeField] protected CollisionSensor leftWalkSensor;
    [SerializeField] protected CollisionSensor rightWalkSensor;
    [SerializeField] protected CollisionSensor leftWallSensor;
    [SerializeField] protected CollisionSensor rightWallSensor;
    [SerializeField] protected CollisionSensor attackHitbox;
    [SerializeField] protected Enemy_Data enemyData;
    [SerializeField] protected Player_Data playerData;
    [SerializeField] private EnemyHealthBar healthBar;
    [SerializeField] private Material flashMat;

    protected LayerMask playerMask;
    protected LayerMask groundMask;
    protected System.Random rand;

    private int room;
    private float flashTime = 0.05f;

    public void TakeDamage(int damage)
    {
        health -= damage;
        healthBar.SetHealth(health);
        animator.SetTrigger("Hurt");
        StartCoroutine(HitFlash());
    }
    
    private IEnumerator HitFlash()
    {
        Material mat = spriteData.material;
        spriteData.material = flashMat;
        yield return new WaitForSeconds(flashTime);
        spriteData.material = mat;
    }

    private IEnumerator AttackPause()
    {
        animator.speed = 0;
        yield return new WaitForSeconds(pauseAttackTime);
        animator.speed = 1;
    }

    protected void CheckForDeath()
    {
        if (health <= 0)
        {
            if (!dead)
            {
                animator.SetTrigger("Death");
                animator.SetBool("isDead", true);
                StopCoroutine(PauseMovementRoutine());
                dead = true;
                gameObject.layer = 9;   //Dead enemies layer

                for(int i = 0; i < droppedCoins; i++)
                {
                    GameObject spawnedCoin = objectPooler.SpawnFromPool("Coin", transform.position, Quaternion.identity);
                    if(spawnedCoin != null)
                    {
                        spawnedCoin.GetComponent<Coin>().SetBodyType(RigidbodyType2D.Dynamic);
                        spawnedCoin.GetComponent<Rigidbody2D>().velocity = new Vector2(rand.Next(-20, 20) * 0.2f, rand.Next(0, 20) * 0.3f);
                    }
                }
                
            }

            fade = fade - Time.deltaTime * 0.3f;
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

    private void ResetAttackState()
    {
        attackHitbox.GetComponent<AttackSensor>().attacking = false;
    }

    protected void InitEnemy()
    {
        objectPooler = ObjectPooler.Instance;
        rand = new System.Random();
        powerUpsList = Resources.LoadAll<GameObject>("Prefabs/PowerUps");
        dead = false;
        walkDirection = 1;
        canMove = true;

        playerMask = LayerMask.GetMask("Player");
        groundMask = LayerMask.GetMask("Ground");

        animator = GetComponent<Animator>();
        spriteData = GetComponent<SpriteRenderer>();
        colliders = GetComponents<Collider2D>();
        body2d = GetComponent<Rigidbody2D>();

        health = enemyData.maxHealth;
        healthBar.SetMaxHealth(health);
        moveSpeed = enemyData.moveSpeed;
        attackDamage = enemyData.attackDamage;

        playerFilter = new ContactFilter2D();
        playerFilter.SetLayerMask(playerData.playerLayer);
        leftWalkSensor.layerMask = groundMask;
        rightWalkSensor.layerMask = groundMask;
        leftWallSensor.layerMask = groundMask;
        rightWallSensor.layerMask = groundMask;
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
            if (leftWalkSensor.isCollision() && !leftWallSensor.isCollision())
            {
                canWalkLeft = true;
            }
            else
            {
                canWalkLeft = false;
            }

            if (rightWalkSensor.isCollision() && !rightWallSensor.isCollision())
            {
                canWalkRight = true;
            }
            else
            {
                canWalkRight = false;
            }

            if (!canWalkLeft && !canWalkRight)
            {
                //Debug.Log("Enemy stuck");
                animator.SetBool("Walking", false);
                walkDirection = 0;
            }
            else if (walkDirection == -1 && !canWalkLeft)    //If walking left and can't keep walking left, then start walking right
            {
                StartCoroutine(PauseMovementRoutine());
            }
            else if (walkDirection == 1 && !canWalkRight)    //If walking right and can't keep walking right, then start walking left
            {
                StartCoroutine(PauseMovementRoutine());
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
                attackHitbox.gameObject.GetComponent<Collider2D>().transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                attackHitbox.gameObject.GetComponent<Collider2D>().transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }
        else
        {
            body2d.velocity = new Vector2(0, body2d.velocity.y);
        }

        if(walkDirection == 1)
        {
            spriteData.flipX = false;
        }
        else if (walkDirection == -1)
        {
            spriteData.flipX = true;
        }

    }

    private IEnumerator PauseMovementRoutine()
    {
        canMove = false;
        animator.SetBool("Walking", false);
        yield return new WaitForSeconds(pauseMovementTime);
        canMove = true;
        walkDirection = walkDirection * -1;
    }

}
