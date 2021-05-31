using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Goblin : Enemy_Handler
{
    // Start is called before the first frame update
    private void Start()
    {
        InitEnemy();
    }
        
    // Update is called once per frame
    void Update()
    {
        Movement();
        CheckForDeath();
    }
}
