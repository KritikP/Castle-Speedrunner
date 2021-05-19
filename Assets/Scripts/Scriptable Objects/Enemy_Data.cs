using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy", order = 51)]
public class Enemy_Data : ScriptableObject
{
    public new string name;
    public int maxHealth;
    public int contactDamage;
    public int attackDamage;
    public float moveSpeed;
    public GameObject prefab;
}
