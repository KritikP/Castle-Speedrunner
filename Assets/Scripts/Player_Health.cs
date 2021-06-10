using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Health : MonoBehaviour, IDamagable
{
    [SerializeField] private Player_Data playerData;

    private SpriteRenderer spriteData;
    private Animator animator;
    private Rigidbody2D body2d;
    private float fade = 1f;

    private void Awake()
    {
        playerData.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteData = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        body2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckForDeath();
    }

    private void CheckForDeath()
    {
        if (playerData.health <= 0)
        {
            body2d.velocity = new Vector2(0f, body2d.velocity.y);
            if (!playerData.isDead)
            {
                FindObjectOfType<GameUIManager>().PlayerDeath();
                playerData.isDead = true;
                animator.SetBool("isDead", true);
                animator.SetTrigger("Death");
                gameObject.layer = 9;   //Dead enemies layer
            }

            fade = fade - Time.deltaTime * 0.3f;
            Color c = spriteData.color;
            c.a = fade;
            spriteData.color = c;
            if (fade <= 0f)
                Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        if (!playerData.invincible)
        {
            playerData.health -= damage;

            if (playerData.health > 0)
            {
                animator.SetTrigger("Hurt");
                playerData.invincible = true;
                Debug.Log("Took Damage, Health = " + playerData.health);
                body2d.velocity = new Vector2(0f, body2d.velocity.y);
                StartCoroutine(InvincibilityFrames(playerData.invincibilityTime));
            }
        }
        
    }

    public void TakeDamageRolling(int damage)
    {
        if (!playerData.invincible || playerData.rolling)
        {
            playerData.health -= damage;

            if (playerData.health > 0)
            {
                animator.SetTrigger("Hurt");
                playerData.invincible = true;
                body2d.velocity = new Vector2(0f, body2d.velocity.y);
                StartCoroutine(InvincibilityFrames(playerData.invincibilityTime));
            }
        }
    }

    public void InstaKill()
    {
        playerData.health = -100;
    }

    public IEnumerator InvincibilityFrames(float time)
    {
        float flashTime = 0f;
        while (flashTime < time)
        {
            spriteData.forceRenderingOff = true;
            yield return new WaitForSeconds(0.05f);
            spriteData.forceRenderingOff = false;
            yield return new WaitForSeconds(0.05f);
            flashTime += 0.1f;
        }
        playerData.invincible = false;
    }

}
