using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.Universal;

public class PowerUp : MonoBehaviour, IPooledObject
{
    [SerializeField] private Player_Data playerData;
    [SerializeField] private PowerUps powerUps;
    [SerializeField] private PowerUpType type;

    private SpriteRenderer spriteData;
    private Collider2D collider2d;
    private Player_Health playerHealth;
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

    [HideInInspector] public GameObject powerUpCageLight;

    private void Awake()
    {
        playerHealth = FindObjectOfType<Player_Health>();
        powerUpGlow = GetComponent<Light2D>();
        body2d = GetComponent<Rigidbody2D>();
        spriteData = GetComponent<SpriteRenderer>();
        collider2d = GetComponent<Collider2D>();
    }

    public void OnObjectSpawn()
    {
        waitingForPlayer = true;
        active = false;
        spriteData.enabled = false;
        powerUpGlow.intensity = 0f;
    }

    public void SetPowerUpType(PowerUpType type)
    {
        this.type = type;
        spriteData.sprite = powerUps.powerUpItems[(int) type].sprite;
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
            if (distanceFromPlayer < 10f)
            {
                powerUpCageLight.GetComponent<Light2D>().intensity = 0f;
                spriteData.enabled = true;
                body2d.velocity = new Vector2(0f, 4f);
                waitingForPlayer = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!active && 1 << collision.gameObject.layer == playerData.playerLayer)
        {
            active = true;
            spriteData.enabled = false;
            powerUpGlow.enabled = false;
            if (type == PowerUpType.Attack)
            {
                playerHealth.PowerUpAttack(powerUps.powerUpItems[(int)type].time);
            }
            else if (type == PowerUpType.Invincibility)
            {
                playerHealth.PowerUpInvincible(powerUps.powerUpItems[(int)type].time);
            }
            else if (type == PowerUpType.Speed)
            {
                playerHealth.PowerUpSpeed(powerUps.powerUpItems[(int)type].time);
            }
            gameObject.SetActive(false);
        }
    }
}