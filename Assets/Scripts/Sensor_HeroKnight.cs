using UnityEngine;
using System.Collections;

public class Sensor_HeroKnight : MonoBehaviour {

    //private int ColCount = 0;

    private float DisableTimer;

    private bool isColliding = false;
    private Collider2D collider2d;

    private void Start()
    {
        collider2d = GetComponent<Collider2D>();
    }

    public bool State()
    {
        if (DisableTimer > 0)
            return false;
        return isColliding;
        //return ColCount > 0;
    }

    /*
    private void OnEnable()
    {
        ColCount = 0;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 11)
        {
            ColCount++;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 11)
        {
            ColCount--;
        }
    }
    */

    void Update()
    {
        DisableTimer -= Time.deltaTime;
        isColliding = collider2d.IsTouchingLayers(LayerMask.GetMask("Ground"));
    }

    public void Disable(float duration)
    {
        DisableTimer = duration;
    }
}
