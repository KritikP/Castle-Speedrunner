using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Player_Data playerData;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        text.text = "0";
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerData.isDead)
        {
            time += Time.smoothDeltaTime;
            text.text = time.ToString("F0");
        }
    }
}
