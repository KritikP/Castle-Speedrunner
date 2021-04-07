using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Goblin : MonoBehaviour
{
    Rigidbody2D m_body2d;
    Animator m_animator;

    // Start is called before the first frame update
    void Start()
    {
        m_body2d = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
