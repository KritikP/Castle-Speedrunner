using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Saw : MonoBehaviour, IPooledObject
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private int damage = 10;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > 18)
            gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (1 << other.gameObject.layer == playerMask && other.GetComponent<IDamagable>() != null)
        {
            other.GetComponent<Player_Health>().TakeDamageRolling(damage);
        }
    }

    public void OnObjectSpawn()
    {

    }
}
