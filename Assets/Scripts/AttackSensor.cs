using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackSensor : CollisionSensor
{
    public UnityEvent attackEvent;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == layerMask)
        {
            collisionCount = true;
            attackEvent.Invoke();
        }
    }
}
