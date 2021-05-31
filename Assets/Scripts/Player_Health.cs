using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Health : MonoBehaviour, IDamagable
{
    [SerializeField] private Player_Data player;
    private SpriteRenderer spriteData;
    private Animator animator;
    private Rigidbody2D body2d;
    private float fade = 1f;

    // Start is called before the first frame update
    void Start()
    {
        spriteData = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        body2d = GetComponent<Rigidbody2D>();
        player.health = player.maxHealth;
    }

    private void Update()
    {
        CheckForDeath();
    }

    private void CheckForDeath()
    {
        if (player.health <= 0)
        {
            body2d.velocity = new Vector2(0f, body2d.velocity.y);
            if (!player.isDead)
            {
                player.isDead = true;
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
        if (!player.invincible)
        {
            player.health -= damage;

            if (player.health > 0)
            {
                animator.SetTrigger("Hurt");
                player.invincible = true;
                Debug.Log("Took Damage, Health = " + player.health);
                body2d.velocity = new Vector2(0f, body2d.velocity.y);
                StartCoroutine(InvincibilityFrames());
            }
            else
            {
                //Dying
                Debug.Log("Dying, Health = " + player.health);
            }
            
        }
        
    }

    private IEnumerator InvincibilityFrames()
    {
        float flashTime = 0f;
        while (flashTime < player.invincibilityTime)
        {
            spriteData.forceRenderingOff = true;
            yield return new WaitForSeconds(0.05f);
            spriteData.forceRenderingOff = false;
            yield return new WaitForSeconds(0.05f);
            flashTime += 0.1f;
        }
        player.invincible = false;
    }

}
