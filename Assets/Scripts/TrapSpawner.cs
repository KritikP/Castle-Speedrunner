using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSpawner : MonoBehaviour
{
    [SerializeField] private GameObject shuriken;
    [SerializeField, Range(1, 100)] private float shurikenSpawnSpeed = 1f;
    [SerializeField, Range(1, 100)] private float shurikenSpeed = 10f;
    [SerializeField] private Map_Data mapData;
    [SerializeField] private bool firing = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnShurikenTrap(Vector3 position)
    {
        StartCoroutine(SpawnShuriken(position));
    }

    private IEnumerator SpawnShuriken(Vector3 position)
    {
        int trapRoom = mapData.currentRoom;

        while (trapRoom == mapData.currentRoom)
        {
            yield return new WaitUntil(isFiring);
            yield return new WaitForSeconds(shurikenSpawnSpeed);
            GameObject newShuriken = Instantiate(shuriken, position, Quaternion.identity);
            newShuriken.GetComponent<Rigidbody2D>().velocity = new Vector2(0, shurikenSpeed);
        }
        
    }

    private bool isFiring()
    {
        return firing;
    }
}
