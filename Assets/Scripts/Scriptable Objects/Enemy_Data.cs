using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy", order = 51)]
public class Enemy_Data : ScriptableObject
{
    public new string name;
    public float maxHealth;
    public float contactDamage;

}
