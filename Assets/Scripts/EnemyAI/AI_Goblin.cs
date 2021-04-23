using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Goblin : MonoBehaviour
{
    private Rigidbody2D m_body2d;
    private Animator m_animator;
    [SerializeField] private Map_Data mapData;
    private int room;

    // Start is called before the first frame update
    void Start()
    {
        m_body2d = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        room = mapData.currentRoom;
    }

    void Movement()
    {
        //if(mapData.mapSections[room].GetTile()
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }
}
