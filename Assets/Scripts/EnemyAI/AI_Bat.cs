using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Bat : Enemy_Handler
{
    // Start is called before the first frame update
    void Start()
    {
        InitEnemy();
        spriteData.flipX = !spriteData.flipX;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        CheckForDeath();
    }
}
