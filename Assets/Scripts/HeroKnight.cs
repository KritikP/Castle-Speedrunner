using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class HeroKnight : MonoBehaviour {

    [SerializeField] private Player_Data player;
    [SerializeField] private Map_Data mapData;

    [SerializeField] bool       noBlood = false;
    [SerializeField] GameObject slideDust;
    [SerializeField] GameObject attackHitbox;
    [SerializeField] LayerMask  enemyLayer;

    private PlayerInput         input;
    private AudioManager        audioManager;
    private ContactFilter2D     contactFilter2d;
    public Animator            animator;
    private Rigidbody2D         body2d;
    private BoxCollider2D       boxCollider2d;
    private Player_Health       playerHealth;
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
    private float               timeSinceRoll = 0.0f;
    private float               delayToIdle = 0.0f;
    private Trajectory          trajectory;

    private float inputX;
    private bool attack;
    private bool jump;
    private bool roll;
    private bool blockDown;
    private bool blockUp;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        body2d = GetComponent<Rigidbody2D>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        playerHealth = GetComponent<Player_Health>();
        input = GetComponent<PlayerInput>();
        groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    // Use this for initialization
    void Start ()
    {
        audioManager = FindObjectOfType<AudioManager>();
        contactFilter2d = new ContactFilter2D();
        contactFilter2d.SetLayerMask(enemyLayer);
        Physics2D.IgnoreLayerCollision(9, 10);  //Ignore collisions between dead enemies and player
        Physics2D.IgnoreLayerCollision(8, 10);  //Ignore collisions between enemies and player
        Physics2D.IgnoreLayerCollision(8, 9);  //Ignore collisions between enemies and dead enemies
        Physics2D.IgnoreLayerCollision(8, 8);  //Ignore collisions between enemies
        Physics2D.IgnoreLayerCollision(9, 9);  //Ignore collisions between dead enemies
        Physics2D.IgnoreLayerCollision(12, 12);  //Ignore collisions between hitboxes

        audioManager.Play("Level Music");
        audioManager.Stop("Menu Music");

        player.stamina = player.maxStamina;
    }

    // Update is called once per frame
    void Update ()
    {
        if(transform.position.x > mapData.mapSections[player.currentRoom].basePosition.x + mapData.mapSections[player.currentRoom].width + 2)
        {
            player.currentRoom += 2;
        }

        input.enabled = animator.GetBool("DEBUG_canMove");
        // Increase timer that controls attack combo
        timeSinceAttack += Time.deltaTime;
        timeSinceRoll += Time.deltaTime;

        if (!player.isDead)
        {
            if(rolling && timeSinceRoll > 0.57f)
            {
                rolling = false;
                player.rolling = false;
                if(!playerHealth.InvinciblePowerUpActive)
                    player.invincible = false;
            }

            if (player.stamina < player.maxStamina && timeSinceRoll > 1.5f)
            {
                player.stamina += Time.deltaTime * player.staminaRecoverySpeed;
            }

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
            if ((facingDirection == 1 && attackHitbox.transform.localPosition.x < 0) || (facingDirection == -1 && attackHitbox.transform.localPosition.x > 0))
            {
                attackHitbox.transform.localPosition = new Vector2(attackHitbox.transform.localPosition.x * -1, attackHitbox.transform.localPosition.y);
                boxCollider2d.offset = new Vector2(-boxCollider2d.offset.x, boxCollider2d.offset.y);
            }

            // Move
            if (!rolling && timeSinceAttack > 0.5f && player.canMove)
            {
                if(inputX > 0)
                    body2d.velocity = new Vector2(1 * player.speed, body2d.velocity.y);
                else if (inputX < 0)
                    body2d.velocity = new Vector2(-1 * player.speed, body2d.velocity.y);
                else
                    body2d.velocity = new Vector2(0, body2d.velocity.y);
            }

            //Set AirSpeed in animator
            animator.SetFloat("AirSpeedY", body2d.velocity.y);

            // -- Handle Animations --
            //Wall Slide
            animator.SetBool("WallSlide", (wallSensorR1.State() && wallSensorR2.State()) || (wallSensorL1.State() && wallSensorL2.State()));

            //Attack
            if (attack && timeSinceAttack > 0.3f && grounded && !rolling)
            {
                currentAttack++;
                attack = false;

                body2d.velocity = new Vector2(0, body2d.velocity.y);

                List<Collider2D> hitColliders = new List<Collider2D>();
                attackHitbox.GetComponent<Collider2D>().OverlapCollider(contactFilter2d, hitColliders);
                
                foreach (Collider2D enemy in hitColliders)
                {
                    if (enemy.GetComponent<IDamagable>() != null)
                    {
                        //Debug.Log("We hit enemy: " + enemy.name);
                        audioManager.Play("Hit");
                        enemy.GetComponent<IDamagable>().TakeDamage(player.attackDamage);
                    }
                    else if (enemy.GetComponentInParent<IDamagable>() != null)
                    {
                        //Debug.Log("We hit enemy: " + enemy.name);
                        audioManager.Play("Hit");
                        enemy.GetComponentInParent<IDamagable>().TakeDamage(player.attackDamage);
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
                //Play attack audio
                audioManager.Play("swing" + currentAttack.ToString());
                // Reset timer
                timeSinceAttack = 0.0f;
            }

            // Block
            else if (blockDown && !rolling)
            {
                animator.SetTrigger("Block");
                animator.SetBool("IdleBlock", true);
                body2d.velocity = new Vector2(0, body2d.velocity.y);
                player.canMove = false;
            }

            else if (blockUp && !rolling)
            {
                animator.SetBool("IdleBlock", false);
                player.canMove = true;
                blockUp = false;
            }

            // Roll
            else if (roll && !rolling && grounded && player.stamina > 33)
            {
                roll = false;
                player.invincible = true;
                timeSinceRoll = 0f;
                rolling = true;
                player.rolling = true;
                animator.SetTrigger("Roll");
                body2d.velocity = new Vector2(facingDirection * player.rollSpeed, body2d.velocity.y);
                player.stamina -= 33;
            }

            //Jump
            else if (jump && grounded && !rolling)
            {
                if (facingDirection > 0)
                    trajectory = new Trajectory(body2d.transform.position, player.speed, player.jumpSpeed, player.fallMultiplier, true);
                else
                    trajectory = new Trajectory(body2d.transform.position, -player.speed, player.jumpSpeed, player.fallMultiplier, true);
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
                if (delayToIdle < 0)
                    animator.SetInteger("AnimState", 0);
            }

            if (body2d.velocity.y < 0)
            {
                body2d.velocity += Vector2.up * Physics2D.gravity.y * (player.fallMultiplier - 1) * Time.deltaTime;
            }

            else if (body2d.velocity.y > 0 && !jump)
            {
                body2d.velocity += Vector2.up * Physics2D.gravity.y * (player.variableJumpMultiplier) * Time.deltaTime;
            }

        }
    }

    // Animation Events
    // Called in end of roll animation.
    void AE_ResetRoll()
    {
        rolling = false;
        player.rolling = false;
        if(!playerHealth.InvinciblePowerUpActive)
            player.invincible = false;
    }

    void AE_HurtStart()
    {
        animator.SetBool("isHurting", true);
        body2d.velocity = new Vector2(0f, body2d.velocity.y);
        player.canMove = false;
    }

    void AE_HurtEnd()
    {
        animator.SetBool("isHurting", false);
        player.canMove = true;
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

    /*
    void DisableMovement()
    {
        body2d.velocity = new Vector2(0f, body2d.velocity.y);
        player.canMove = false;
    }

    void EnableMovement()
    {
        player.canMove = true;
    }
    */

    private void OnDrawGizmos()
    {
        if (totalJumps > 0)
        {
            for (int i = 0; i < trajectory.numPoints - 1; i++)
            {
                if (i % 2 == 0) Gizmos.color = Color.blue;
                else Gizmos.color = Color.green;

                Gizmos.DrawLine(trajectory.points[i], trajectory.points[i + 1]);
            }
        }
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        float val = value.ReadValue<float>();
        inputX = val;
    }

    public void OnAttack(InputAction.CallbackContext value)
    {
        if (value.started)
            attack = true;
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        if (value.started)
            jump = true;
        else if (value.canceled)
            jump = false;
    }

    public void OnRoll(InputAction.CallbackContext value)
    {
        if (value.started)
            roll = true;
        else if (value.canceled)
            roll = false;
    }

    public void OnBlock(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            blockDown = true;
            blockUp = false;
        }
            
        else if (value.canceled)
        {
            blockDown = false;
            blockUp = true;
        }
    }
}
