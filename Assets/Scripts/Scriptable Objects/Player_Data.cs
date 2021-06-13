using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player Data", order = 51)]
public class Player_Data : ScriptableObject
{
    [SerializeField] public int     maxHealth = 100;
    [SerializeField] public int     health = 100;
    [SerializeField] public float   maxStamina = 100f;
    [SerializeField] public float   staminaRecoverySpeed = 35f;
    [SerializeField] public float   stamina = 100f;
    [SerializeField] public bool    invincible = false;
    [SerializeField] public bool    isBlocking = false;
    [SerializeField] public bool    rolling = false;
    [SerializeField] public bool    canMove = true;
    [SerializeField] public float   invincibilityTime = 1f;
    [SerializeField] public int     baseAttackDamage = 20;
    [SerializeField] public int     attackDamage = 20;
    [SerializeField] public float   baseSpeed = 7f;
    [SerializeField] public float   speed = 7f;
    [SerializeField] public float   jumpSpeed = 7.5f;
    [SerializeField] public float   rollSpeed = 10f;
    [SerializeField] public float   fallMultiplier = 1f;
    [SerializeField] public float   variableJumpMultiplier = 2f;
    [SerializeField] public LayerMask playerLayer;
    public bool                     isDead = false;

    public int                      coins = 0;

    [SerializeField] public ShopItem armorUpgrade;
    [SerializeField] public ShopItem attackUpgrade;

    public void Init()
    {
        attackDamage = baseAttackDamage + 5 * attackUpgrade.level;

        maxHealth = 100 + 25 * armorUpgrade.level;
        health = maxHealth;

        speed = baseSpeed;

        isDead = false;
        invincible = false;
        canMove = true;
    }

}