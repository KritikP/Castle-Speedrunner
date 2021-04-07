using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Health : MonoBehaviour
{
    [SerializeField] private Player_Data player;
    private SpriteRenderer m_spriteRenderer;
    private Collider2D m_collider2D;

    // Start is called before the first frame update
    void Start()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        player.health = player.maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (!player.invincible)
        {
            player.health -= damage;
        }
        player.invincible = true;
        Debug.Log("Took Damage, Health = " + player.health);
        StartCoroutine(InvincibilityFrames());
    }

    private IEnumerator InvincibilityFrames()
    {
        float flashTime = 0f;
        while (flashTime < player.invincibilityTime)
        {
            m_spriteRenderer.forceRenderingOff = true;
            yield return new WaitForSeconds(0.05f);
            m_spriteRenderer.forceRenderingOff = false;
            yield return new WaitForSeconds(0.05f);
            flashTime += 0.1f;
        }
        player.invincible = false;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer.CompareTo(8) == 0)
        {
            TakeDamage(20);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
