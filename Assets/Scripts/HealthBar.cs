using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Player_Data playerData;

    private void Start()
    {
        slider.maxValue = playerData.maxHealth;
    }

    private void Update()
    {
        slider.value = playerData.health;
    }
}
