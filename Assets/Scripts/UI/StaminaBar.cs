using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] Player_Data playerData;
    [SerializeField] Slider staminaFill;
    [SerializeField] Slider staminaBar;

    void Update()
    {
        staminaBar.value = playerData.maxStamina;
        staminaFill.value = playerData.stamina;
    }
}
