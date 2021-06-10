using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackSensor : CollisionSensor
{
    public UnityEvent attackEvent;
    public bool attacking = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!attacking)
        {
            if (1 << other.gameObject.layer == layerMask)
            {
                attacking = true;
                Debug.Log("Attacking: " + other.name);
                attackEvent.Invoke();
            }
        }

    }
}