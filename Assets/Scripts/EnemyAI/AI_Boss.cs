using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Boss : Enemy_Handler
{
    // Start is called before the first frame update
    void Start()
    {
        InitEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        CheckForDeath();
    }

    public override void StartAttack()
    {
        
    }

    public override void Attack()
    {
        
    }

    
}
