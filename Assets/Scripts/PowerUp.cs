using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.Universal;

public class PowerUp : MonoBehaviour, IPooledObject
{
    [SerializeField] private Player_Data playerData;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] public UnityEvent powerUp;
    [SerializeField] private SpriteRenderer sprite;

    private Player_Health playerHealth;
    private Light2D playerGlowTimer;
    private Light2D powerUpGlow;
    private Rigidbody2D body2d;
    private bool active = false;
    private bool waitingForPlayer = false;
    private float distanceFromPlayer;

    private float minRadius = 0.6f;
    private float maxRadius = 0.9f;
    private float minIntensity = 0.6f;
    private float maxIntensity = 1.1f;

    private float glowPulseDuration = 1f;
    private float time = 0f;
    private float startTime;

    public GameObject powerUpCageLight;

    private void Awake()
    {
        playerHealth = FindObjectOfType<Player_Health>();
        playerGlowTimer = playerHealth.gameObject.GetComponentInChildren<Light2D>();
        powerUpGlow = GetComponent<Light2D>();
        startTime = Time.time;
    }

    private void Start()
    {
        body2d = GetComponent<Rigidbody2D>();
    }

    public void OnObjectSpawn()
    {
        waitingForPlayer = true;
        active = false;
        sprite.enabled = false;
        powerUpGlow.intensity = 0f;
    }

    private void Update()
    {
        if (!active && !waitingForPlayer)
        {
            time += Time.deltaTime;
            if (time < glowPulseDuration)
            {
                powerUpGlow.pointLightOuterRadius = Mathf.SmoothStep(minRadius, maxRadius, time / glowPulseDuration);
                powerUpGlow.intensity = Mathf.SmoothStep(minRadius, maxRadius, time / glowPulseDuration);
            }
            else
            {
                float temp = maxRadius;
                maxRadius = minRadius;
                minRadius = temp;

                temp = maxIntensity;
                maxIntensity = minIntensity;
                minIntensity = temp;
                time = 0;
            }
        }

        else if (waitingForPlayer)
        {
            distanceFromPlayer = gameObject.transform.position.x - playerHealth.gameObject.transform.position.x;
            if (distanceFromPlayer < 20f)
            {
                //powerUpCageLight.
                sprite.enabled = true;
                body2d.velocity = new Vector2(0f, 3f);
                waitingForPlayer = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!active && 1 << collision.gameObject.layer == playerLayer)
        {
            active = true;
            sprite.enabled = false;
            powerUpGlow.enabled = false;
            powerUp.Invoke();
        }
    }

    public void PowerUpInvincible(float time)
    {
        playerData.invincible = true;
        StartCoroutine(PowerUpInvincibleRoutine(time));
        StartCoroutine(playerGlowRoutine(time));
    }

    private IEnumerator PowerUpInvincibleRoutine(float time)
    {
        Physics2D.IgnoreLayerCollision(10, 14, true);
        yield return new WaitForSeconds(time);
        Physics2D.IgnoreLayerCollision(10, 14, false);
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
        playerGlowTimer.intensity = 1f;
        playerGlowTimer.pointLightInnerAngle = 360f;
        playerGlowTimer.pointLightOuterAngle = 360f;
        float currentTime = 0;
        while (currentTime <= time)
        {
            yield return new WaitForEndOfFrame();
            playerGlowTimer.pointLightInnerAngle = 360 - currentTime * (360 / time);
            playerGlowTimer.pointLightOuterAngle = 360 - currentTime * (360 / time);
            currentTime += Time.deltaTime;
        }
        playerGlowTimer.intensity = 0f;
    }

}