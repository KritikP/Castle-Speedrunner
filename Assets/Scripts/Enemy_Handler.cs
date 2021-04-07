using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Handler : MonoBehaviour
{
    private int health;
    private Animator m_animator;
    private SpriteRenderer m_renderer;
    private Collider2D[] m_colliders;
    private float fade = 1f;
    
    // Start is called before the first frame update
    void Start()
    {
        health = 60;
        m_animator = GetComponent<Animator>();
        m_renderer = GetComponent<SpriteRenderer>();
        m_colliders = GetComponents<Collider2D>();

    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        m_animator.SetTrigger("Hurt");
        
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            fade = fade - Time.deltaTime * 0.3f;
            m_animator.SetTrigger("Death");
            gameObject.layer = 9;   //Dead enemies layer

            //Fade and Destroy
            Color c = m_renderer.material.color;
            c.a = fade;
            m_renderer.material.color = c;
            if (fade <= 0f)
                Destroy(gameObject);
        }
    }

}
