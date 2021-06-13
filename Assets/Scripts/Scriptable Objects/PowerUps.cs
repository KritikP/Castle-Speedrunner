using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUps", menuName = "Power Ups", order = 51)]
public class PowerUps : ScriptableObject
{
    [System.Serializable]
    public class PowerUpItem
    {
        public string name;
        public Sprite sprite;
        public PowerUpType type;
        public float time;
    }

    public PowerUpItem[] powerUpItems;
}

public enum PowerUpType
{
    Attack,
    Invincibility,
    Speed
}