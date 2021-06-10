using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private Player_Data playerData;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] public UnityEvent powerUp;
    [SerializeField] private SpriteRenderer sprite;
    private Player_Health playerHealth;
    private bool active = false;

    private void Start()
    {
        playerHealth = FindObjectOfType<Player_Health>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!active && 1 << collision.gameObject.layer == playerLayer)
        {
            active = true;
            sprite.enabled = false;
            powerUp.Invoke();
        }
    }

    public void PowerUpInvincible(float time)
    {
        playerData.invincible = true;
        StartCoroutine(playerHealth.InvincibilityFrames(time));
        StartCoroutine(PowerUpInvincibleRoutine(time));
    }

    private IEnumerator PowerUpInvincibleRoutine(float time)
    {
        Physics2D.IgnoreLayerCollision(10, 14, true);
        yield return new WaitForSeconds(time + 0.1f);
        Physics2D.IgnoreLayerCollision(10, 14, false);
        Destroy(gameObject);
    }

    public void PowerUpAttack(float time)
    {
        StartCoroutine(PowerUpAttackRoutine(time));
    }

    private IEnumerator PowerUpAttackRoutine(float time)
    {
        int damage = playerData.attackDamage;
        playerData.attackDamage *= 2;
        yield return new WaitForSeconds(time);
        playerData.attackDamage = damage;
        Destroy(this);
    }

    public void PowerUpSpeed(float time)
    {
        StartCoroutine(PowerUpSpeedRoutine(time));
    }

    private IEnumerator PowerUpSpeedRoutine(float time)
    {
        float speed = playerData.speed;
        playerData.speed *= 1.3f;
        yield return new WaitForSeconds(time);
        playerData.speed = speed;
        Destroy(gameObject);
    }
}