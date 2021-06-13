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
    private GameObject player;
    private ObjectPooler objectPooler;

    private float time;

    private void Awake()
    {
        player = FindObjectOfType<HeroKnight>().gameObject;
    }

    private void Start()
    {
        objectPooler = ObjectPooler.Instance;
    }

    public void SpawnShurikenTrap(Vector3 position)
    {
        StartCoroutine(SpawnShuriken(position));
    }

    private IEnumerator SpawnShuriken(Vector3 position)
    {
        int trapRoom = mapData.currentRoom;

        while (player.transform.position.x < (trapRoom - 1) * mapData.mapSections[0].width)
        {
            yield return new WaitUntil(isFiring);
            yield return new WaitForSeconds(shurikenSpawnSpeed);
            GameObject newShuriken = objectPooler.SpawnFromPool("Trap_Shuriken", position, Quaternion.identity);
            newShuriken.GetComponent<Rigidbody2D>().velocity = new Vector2(0, shurikenSpeed);
        }
        
    }

    private bool isFiring()
    {
        return firing;
    }
}
