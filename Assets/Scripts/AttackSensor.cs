using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackSensor : MonoBehaviour
{
    public UnityEvent attackEvent;
    public bool attacking = false;
    public LayerMask layerMask;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!attacking)
        {
            if (1 << other.gameObject.layer == layerMask)
            {
                attacking = true;
                attackEvent.Invoke();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!attacking)
        {
            if (1 << other.gameObject.layer == layerMask)
            {
                attacking = true;
                attackEvent.Invoke();
            }
        }

    }

    public void ResetAttackingState()
    {
        attacking = false;
    }

}