using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSensor : MonoBehaviour
{
    protected int collisionCount = 0;
    [SerializeField] public LayerMask layerMask;

    private void Start()
    {
        collisionCount = 0;
    }

    public bool isCollision()
    {
        return collisionCount > 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (1 << other.gameObject.layer == layerMask)
        {
            collisionCount++;
            //Debug.Log("Collision enter");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (1 << other.gameObject.layer == layerMask)
        {
            collisionCount--;
            //Debug.Log("Collision exit");
        }
    }
}
