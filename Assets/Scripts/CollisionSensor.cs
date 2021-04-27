using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSensor : MonoBehaviour
{
    protected bool collisionCount;
    public int layerMask;

    private void Start()
    {
        collisionCount = false;
    }

    public bool isCollision()
    {
        return collisionCount;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == layerMask)
        {
            collisionCount = true;
            //Debug.Log("Collision = true");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == layerMask)
        {
            collisionCount = false;
            //Debug.Log("Collision = false");
        }
    }
}
