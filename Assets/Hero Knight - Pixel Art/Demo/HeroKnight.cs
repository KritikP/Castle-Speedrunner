using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroKnight : MonoBehaviour {

    [SerializeField] Player_State m_player;
    [SerializeField] bool       m_noBlood = false;
    [SerializeField] GameObject m_slideDust;
    [SerializeField] GameObject m_attackHitbox;
    [SerializeField] LayerMask  m_enemyLayer;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Collider2D          m_collider2D;
    private Sensor_HeroKnight   m_groundSensor;
    private Sensor_HeroKnight   m_wallSensorR1;
    private Sensor_HeroKnight   m_wallSensorR2;
    private Sensor_HeroKnight   m_wallSensorL1;
    private Sensor_HeroKnight   m_wallSensorL2;
    private bool                m_grounded = false;
    private bool                m_rolling = false;
    private bool                m_jumping = false;
    private int                 m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private int                 totalJumps;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;
    private List<Trajectory>        m_trajectories;

    // Use this for initialization
    void Start ()
    {
        m_trajectories = new List<Trajectory>();
        totalJumps = 0;

        Physics2D.IgnoreLayerCollision(9, 10);  //Ignore collisions between player and dead enemies
        Physics2D.IgnoreLayerCollision(8, 10);  //Ignore collisions between player and dead enemies

        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    // Update is called once per frame
    void Update ()
    {
        m_player.position = transform;
        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0 && !m_rolling)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
            
        else if (inputX < 0 && !m_rolling)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        //Attack hitbox position
        if(m_facingDirection == 1 && m_attackHitbox.transform.localPosition.x < 0)
            m_attackHitbox.transform.localPosition = new Vector2(m_attackHitbox.transform.localPosition.x * -1, m_attackHitbox.transform.localPosition.y);
        else if(m_facingDirection == -1 && m_attackHitbox.transform.localPosition.x > 0)
            m_attackHitbox.transform.localPosition = new Vector2(m_attackHitbox.transform.localPosition.x * -1, m_attackHitbox.transform.localPosition.y);

        // Move
        if (!m_rolling && m_timeSinceAttack > 0.5f)
            m_body2d.velocity = new Vector2(inputX * m_player.speed, m_body2d.velocity.y);

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        // -- Handle Animations --
        //Wall Slide
        m_animator.SetBool("WallSlide", (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State()));

        //Death
        if (Input.GetKeyDown("e") && !m_rolling)
        {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        }
            
        //Hurt
        else if (Input.GetKeyDown("q") && !m_rolling)
            m_animator.SetTrigger("Hurt");

        //Attack
        else if(Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && m_grounded && !m_rolling)
        {
            m_currentAttack++;
            
            m_body2d.velocity = new Vector2(0, m_body2d.velocity.y);

            Collider2D[] attackCollisions = Physics2D.OverlapCircleAll(m_attackHitbox.transform.position, m_attackHitbox.GetComponent<CircleCollider2D>().radius, m_enemyLayer);

            foreach(Collider2D enemy in attackCollisions)
            {
                Debug.Log("We hit " + enemy.name);
                if (enemy.GetComponent<Enemy_Handler>())
                {
                    enemy.GetComponent<Enemy_Handler>().TakeDamage(m_player.attackDamage);
                }
            }

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            m_animator.SetTrigger("Attack" + m_currentAttack);

            // Reset timer
            m_timeSinceAttack = 0.0f;
        }

        // Block
        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }

        else if (Input.GetMouseButtonUp(1) && !m_rolling)
            m_animator.SetBool("IdleBlock", false);

        // Roll
        else if (Input.GetKeyDown("left shift") && !m_rolling && m_grounded)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_player.rollSpeed, m_body2d.velocity.y);
        }

        //Jump
        else if (Input.GetKeyDown("space") && m_grounded && !m_rolling)
        {
            if(m_facingDirection > 0)
                m_trajectories.Add(new Trajectory(m_body2d.transform.position, m_player.speed, m_player.jumpSpeed, m_player.fallMultiplier, true));
            else
                m_trajectories.Add(new Trajectory(m_body2d.transform.position, -m_player.speed, m_player.jumpSpeed, m_player.fallMultiplier, true));
            totalJumps++;
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_jumping = true;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_player.jumpSpeed);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
                if(m_delayToIdle < 0)
                    m_animator.SetInteger("AnimState", 0);
        }

        if(m_body2d.velocity.y < 0)
        {
            m_body2d.velocity += Vector2.up * Physics2D.gravity.y * (m_player.fallMultiplier - 1) * Time.deltaTime;
        }

        else if(m_body2d.velocity.y > 0 && !Input.GetKey("space"))
        {
            m_body2d.velocity += Vector2.up * Physics2D.gravity.y * (m_player.variableJumpMultiplier) * Time.deltaTime;
        }

        /*Debugging max height information
        if(m_body2d.velocity.y <= 0 && Input.GetKey("space"))
        {
            //Debug.Log("Max Height Calc " + (m_trajectories[0].startPos.y + (Mathf.Pow(m_player.jumpSpeed, 2) / (2 * -Physics2D.gravity.y)) ));
            //Debug.Log("Rigidbody pos " + m_body2d.position.y);
        }
        */

    }

    // Animation Events
    // Called in end of roll animation.
    void AE_ResetRoll()
    {
        m_rolling = false;
    }

    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }

    private void OnDrawGizmos()
    {
        if (totalJumps > 0)
        {
            for (int i = 0; i < m_trajectories[totalJumps - 1].numPoints - 1; i++)
            {
                if (i % 2 == 0) Gizmos.color = Color.blue;
                else Gizmos.color = Color.green;

                Gizmos.DrawLine(m_trajectories[totalJumps - 1].points[i], m_trajectories[totalJumps - 1].points[i + 1]);
            }
        }
    }
}
