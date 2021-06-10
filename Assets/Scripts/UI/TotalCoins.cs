using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TotalCoins : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Player_Data playerData;

    void Update()
    {
        text.text = playerData.coins.ToString();
    }
}
