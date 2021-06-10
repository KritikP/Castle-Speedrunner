using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Saw : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private int damage = 10;
    [SerializeField] private float spinSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, -spinSpeed);
        if (transform.position.y > 18)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (1 << other.gameObject.layer == playerMask && other.GetComponent<IDamagable>() != null)
        {
            other.GetComponent<Player_Health>().TakeDamageRolling(damage);
        }
    }
}
