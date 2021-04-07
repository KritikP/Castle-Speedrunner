using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player Data", order = 51)]
[SerializeField]
public class Player_Data : ScriptableObject
{
    [SerializeField] public int     maxHealth = 100;
    [SerializeField] public int     health;
    [SerializeField] public bool    invincible = false;
    [SerializeField] public float   invincibilityTime = 1f;
    [SerializeField] public int     attackDamage;
    [SerializeField] public float   speed = 7f;
    [SerializeField] public float   jumpSpeed = 7.5f;
    [SerializeField] public float   rollSpeed = 10f;
    [SerializeField] public float   fallMultiplier = 1f;
    [SerializeField] public float   variableJumpMultiplier = 2f;
    public Transform                position;
    [SerializeField] public MapSection map1;
}