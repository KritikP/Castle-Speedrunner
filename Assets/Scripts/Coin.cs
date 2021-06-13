using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, IPooledObject
{
    [SerializeField] private Player_Data playerData;
    [SerializeField] private Rigidbody2D body2d;
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void SetBodyType(RigidbodyType2D type)
    {
        body2d.bodyType = type;
    }

    public void OnObjectSpawn()
    {
        body2d.bodyType = RigidbodyType2D.Static;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            playerData.coins++;
            audioManager.Play("Coin Pickup");
            gameObject.SetActive(false);
        }
    }
}
