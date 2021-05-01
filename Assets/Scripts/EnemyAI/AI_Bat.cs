using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Bat : Enemy_Handler
{
    protected override void Movement()
    {
        if (canMove && !dead)
        {
            if (!leftWalkSensor.isCollision())
            {
                canWalkLeft = true;
            }
            else
            {
                canWalkLeft = false;
            }

            if (!rightWalkSensor.isCollision())
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
                animator.SetBool("Walking", true);

            }
            else if (walkDirection == -1 && !canWalkLeft)    //If walking left and can't keep walking left, then start walking right
            {
                walkDirection = 1;
                spriteData.flipX = false;
                animator.SetBool("Walking", true);

            }
            else if (walkDirection == 1 && canWalkRight)     //If walking right and can keep walking right, then keep walking right
            {
                animator.SetBool("Walking", true);

            }
            else    //If walking right and can't keep walking right, then start walking left
            {
                walkDirection = -1;
                spriteData.flipX = true;
                animator.SetBool("Walking", true);
            }

            body2d.velocity = new Vector2(-walkDirection * moveSpeed, body2d.velocity.y);
        }
        else
        {
            body2d.velocity = new Vector2(0, body2d.velocity.y);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        CheckForDeath();
    }
}
