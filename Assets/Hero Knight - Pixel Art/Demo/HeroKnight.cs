using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroKnight : MonoBehaviour {

    [SerializeField] Player_Data player;
    [SerializeField] bool       noBlood = false;
    [SerializeField] GameObject slideDust;
    [SerializeField] GameObject attackHitbox;
    [SerializeField] LayerMask  enemyLayer;

    private Animator            animator;
    private Rigidbody2D         body2d;
    private Collider2D          pCollider2D;
    private Sensor_HeroKnight   groundSensor;
    private Sensor_HeroKnight   wallSensorR1;
    private Sensor_HeroKnight   wallSensorR2;
    private Sensor_HeroKnight   wallSensorL1;
    private Sensor_HeroKnight   wallSensorL2;
    private bool                grounded = false;
    private bool                rolling = false;
    private bool                jumping = false;
    private int                 facingDirection = 1;
    private int                 currentAttack = 0;
    private int                 totalJumps;
    private float               timeSinceAttack = 0.0f;
    private float               delayToIdle = 0.0f;
    private List<Trajectory>    trajectories;

    private void Awake()
    {
        player.playerPosition = transform;
        animator = GetComponent<Animator>();
        body2d = GetComponent<Rigidbody2D>();
        groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    // Use this for initialization
    void Start ()
    {
        trajectories = new List<Trajectory>();
        totalJumps = 0;

        Physics2D.IgnoreLayerCollision(9, 10);  //Ignore collisions between player and dead enemies
        Physics2D.IgnoreLayerCollision(8, 10);  //Ignore collisions between player and dead enemies

    }

    // Update is called once per frame
    void Update ()
    {
        player.playerPosition = transform;
        // Increase timer that controls attack combo
        timeSinceAttack += Time.deltaTime;

        //Check if character just landed on the ground
        if (!grounded && groundSensor.State())
        {
            grounded = true;
            animator.SetBool("Grounded", grounded);
        }

        //Check if character just started falling
        if (grounded && !groundSensor.State())
        {
            grounded = false;
            animator.SetBool("Grounded", grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0 && !rolling)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            facingDirection = 1;
        }
            
        else if (inputX < 0 && !rolling)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            facingDirection = -1;
        }

        //Attack hitbox position
        if(facingDirection == 1 && attackHitbox.transform.localPosition.x < 0)
            attackHitbox.transform.localPosition = new Vector2(attackHitbox.transform.localPosition.x * -1, attackHitbox.transform.localPosition.y);
        else if(facingDirection == -1 && attackHitbox.transform.localPosition.x > 0)
            attackHitbox.transform.localPosition = new Vector2(attackHitbox.transform.localPosition.x * -1, attackHitbox.transform.localPosition.y);

        // Move
        if (!rolling && timeSinceAttack > 0.5f)
            body2d.velocity = new Vector2(inputX * player.speed, body2d.velocity.y);

        //Set AirSpeed in animator
        animator.SetFloat("AirSpeedY", body2d.velocity.y);

        // -- Handle Animations --
        //Wall Slide
        animator.SetBool("WallSlide", (wallSensorR1.State() && wallSensorR2.State()) || (wallSensorL1.State() && wallSensorL2.State()));

        //Death
        if (Input.GetKeyDown("e") && !rolling)
        {
            animator.SetBool("noBlood", noBlood);
            animator.SetTrigger("Death");
        }
            
        //Hurt
        else if (Input.GetKeyDown("q") && !rolling)
            animator.SetTrigger("Hurt");

        //Attack
        else if(Input.GetMouseButtonDown(0) && timeSinceAttack > 0.25f && grounded && !rolling)
        {
            currentAttack++;
            
            body2d.velocity = new Vector2(0, body2d.velocity.y);

            Collider2D[] attackCollisions = Physics2D.OverlapCircleAll(attackHitbox.transform.position, attackHitbox.GetComponent<CircleCollider2D>().radius, enemyLayer);

            foreach(Collider2D enemy in attackCollisions)
            {
                Debug.Log("We hit " + enemy.name);
                if (enemy.GetComponent<Enemy_Handler>())
                {
                    enemy.GetComponent<Enemy_Handler>().TakeDamage(player.attackDamage);
                }
            }

            // Loop back to one after third attack
            if (currentAttack > 3)
                currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (timeSinceAttack > 1.0f)
                currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            animator.SetTrigger("Attack" + currentAttack);

            // Reset timer
            timeSinceAttack = 0.0f;
        }

        // Block
        else if (Input.GetMouseButtonDown(1) && !rolling)
        {
            animator.SetTrigger("Block");
            animator.SetBool("IdleBlock", true);
        }

        else if (Input.GetMouseButtonUp(1) && !rolling)
            animator.SetBool("IdleBlock", false);

        // Roll
        else if (Input.GetKeyDown("left shift") && !rolling && grounded)
        {
            rolling = true;
            animator.SetTrigger("Roll");
            body2d.velocity = new Vector2(facingDirection * player.rollSpeed, body2d.velocity.y);
        }

        //Jump
        else if (Input.GetKeyDown("space") && grounded && !rolling)
        {
            if(facingDirection > 0)
                trajectories.Add(new Trajectory(body2d.transform.position, player.speed, player.jumpSpeed, player.fallMultiplier, true));
            else
                trajectories.Add(new Trajectory(body2d.transform.position, -player.speed, player.jumpSpeed, player.fallMultiplier, true));
            totalJumps++;
            animator.SetTrigger("Jump");
            grounded = false;
            jumping = true;
            animator.SetBool("Grounded", grounded);
            body2d.velocity = new Vector2(body2d.velocity.x, player.jumpSpeed);
            groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            delayToIdle = 0.05f;
            animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            delayToIdle -= Time.deltaTime;
                if(delayToIdle < 0)
                    animator.SetInteger("AnimState", 0);
        }

        if(body2d.velocity.y < 0)
        {
            body2d.velocity += Vector2.up * Physics2D.gravity.y * (player.fallMultiplier - 1) * Time.deltaTime;
        }

        else if(body2d.velocity.y > 0 && !Input.GetKey("space"))
        {
            body2d.velocity += Vector2.up * Physics2D.gravity.y * (player.variableJumpMultiplier) * Time.deltaTime;
        }

        /*Debugging max height information
        if(body2d.velocity.y <= 0 && Input.GetKey("space"))
        {
            //Debug.Log("Max Height Calc " + (trajectories[0].startPos.y + (Mathf.Pow(player.jumpSpeed, 2) / (2 * -Physics2D.gravity.y)) ));
            //Debug.Log("Rigidbody pos " + body2d.position.y);
        }
        */

    }

    // Animation Events
    // Called in end of roll animation.
    void AE_ResetRoll()
    {
        rolling = false;
    }

    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (facingDirection == 1)
            spawnPosition = wallSensorR2.transform.position;
        else
            spawnPosition = wallSensorL2.transform.position;

        if (slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(facingDirection, 1, 1);
        }
    }

    private void OnDrawGizmos()
    {
        if (totalJumps > 0)
        {
            for (int i = 0; i < trajectories[totalJumps - 1].numPoints - 1; i++)
            {
                if (i % 2 == 0) Gizmos.color = Color.blue;
                else Gizmos.color = Color.green;

                Gizmos.DrawLine(trajectories[totalJumps - 1].points[i], trajectories[totalJumps - 1].points[i + 1]);
            }
        }
    }
}
