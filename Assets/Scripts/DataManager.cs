using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DataManager : MonoBehaviour
{
    [SerializeField] private Player_Data playerData;
    [SerializeField] private TMP_Text resetText;

    public void ResetData()
    {
        playerData.armorUpgrade.level = 0;
        playerData.attackUpgrade.level = 0;
        playerData.coins = 0;
        resetText.text = "Reset!";
    }
}
