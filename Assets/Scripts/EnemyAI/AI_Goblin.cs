using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AI_Goblin : MonoBehaviour
{
    [SerializeField] private CollisionSensor leftWalkSensor;
    [SerializeField] private CollisionSensor rightWalkSensor;
    [SerializeField] private AttackSensor attackHitbox;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Player_Data playerData;

    private Rigidbody2D body2d;
    private Animator animator;
    private SpriteRenderer spriteData;

    private int playerMask;
    private int groundMask;
    private int walkDirection;
    private bool canWalkLeft;
    private bool canWalkRight;
    private bool canMove;

    [SerializeField] private Map_Data mapData;
    private int room;

    // Start is called before the first frame update
    void Awake()
    {
        groundMask = 11;
        playerMask = 10;
        body2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteData = GetComponent<SpriteRenderer>();
        room = mapData.currentRoom;
        walkDirection = -1;
        canMove = true;

        //Set layers for collision detection
        leftWalkSensor.layerMask = groundMask;
        rightWalkSensor.layerMask = groundMask;
        attackHitbox.layerMask = playerMask;

        if (moveSpeed <= 0)
        {
            Debug.Log("Move speed must be greater than 0; setting to default 5.");
            moveSpeed = 5f;
        }
    }

    void Movement()
    {
        if (canMove)
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
                //Debug.Log("Enemy stuck");
                animator.SetBool("Walking", false);
                walkDirection = 0;
            }
            else if (walkDirection == -1 && canWalkLeft)     //If walking left and can keep walking left, then keep walking left
            {

            }
            else if (walkDirection == -1 && !canWalkLeft)    //If walking left and can't keep walking left, then start walking right
            {
                walkDirection = 1;
                spriteData.flipX = false;
            }
            else if (walkDirection == 1 && canWalkRight)     //If walking right and can keep walking right, then keep walking right
            {

            }
            else    //If walking right and can't keep walking right, then start walking left
            {
                walkDirection = -1;
                spriteData.flipX = true;
            }

            body2d.velocity = new Vector2(walkDirection * moveSpeed, body2d.velocity.y);
            animator.SetBool("Walking", true);
            
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

    public void StartAttack()
    {
        animator.SetTrigger("Attack 1");
        canMove = false;
    }

    public void Attack(int damage)
    {

    }

    public void StartMoving()
    {
        canMove = true;
    }

    public void StopMoving()
    {
        canMove = false;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }
}
