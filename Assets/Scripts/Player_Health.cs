using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Player_Health : MonoBehaviour, IDamagable
{
    [SerializeField] private Player_Data playerData;

    private SpriteRenderer spriteData;
    private Animator animator;
    private Rigidbody2D body2d;
    private Light2D playerGlowEffect;
    private float fade = 1f;
    public bool InvinciblePowerUpActive = false;

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
        playerGlowEffect = GetComponentInChildren<Light2D>();
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
                //Debug.Log("Took Damage, Health = " + playerData.health);
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

    public void PowerUpInvincible(float time)
    {
        StopCoroutine(InvincibilityFrames(playerData.invincibilityTime));
        spriteData.forceRenderingOff = false;
        InvinciblePowerUpActive = true;
        playerData.invincible = true;
        StartCoroutine(PowerUpInvincibleRoutine(time));
        StartCoroutine(playerGlowRoutine(time));
    }

    private IEnumerator PowerUpInvincibleRoutine(float time)
    {
        Physics2D.IgnoreLayerCollision(10, 14, true);
        yield return new WaitForSeconds(time);
        Physics2D.IgnoreLayerCollision(10, 14, false);
        InvinciblePowerUpActive = false;
        playerData.invincible = false;
    }

    public void PowerUpAttack(float time)
    {
        StartCoroutine(PowerUpAttackRoutine(time));
        StartCoroutine(playerGlowRoutine(time));
    }

    private IEnumerator PowerUpAttackRoutine(float time)
    {
        int damage = playerData.attackDamage;
        playerData.attackDamage *= 2;
        yield return new WaitForSeconds(time);
        playerData.attackDamage = damage;
    }

    public void PowerUpSpeed(float time)
    {
        StartCoroutine(PowerUpSpeedRoutine(time));
        StartCoroutine(playerGlowRoutine(time));
    }

    private IEnumerator PowerUpSpeedRoutine(float time)
    {
        float speed = playerData.speed;
        playerData.speed *= 1.3f;
        yield return new WaitForSeconds(time);
        playerData.speed = speed;
    }

    private IEnumerator playerGlowRoutine(float time)
    {
        playerGlowEffect.intensity = 4f;
        playerGlowEffect.pointLightInnerAngle = 360f;
        playerGlowEffect.pointLightOuterAngle = 360f;
        float currentTime = 0;
        while (currentTime <= time && !playerData.isDead)
        {
            yield return new WaitForEndOfFrame();
            playerGlowEffect.pointLightInnerAngle = 360 - currentTime * (360 / time);
            playerGlowEffect.pointLightOuterAngle = 360 - currentTime * (360 / time);
            currentTime += Time.deltaTime;
        }
        playerGlowEffect.intensity = 0f;
    }

}
