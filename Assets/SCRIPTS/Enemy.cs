using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform thePlayer; 

    [Header("Explosión")]
    [SerializeField] private float radio = 3f;  
    public int damageToPlayer = 20;            
    public LayerMask playerMask;               
    public ParticleSystem explosionEffect;     

    [Header("Vida")]
    public int maxHealth = 100;                
    private int currentHealth;                 
    public float pushBackForce = 5f;           
       
    private bool hasExploded = false;
    public event Action<GameObject> OnDeath; 


    private void Start()
    {
        thePlayer = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (hasExploded) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, radio, playerMask);
        if (hits.Length > 0)
        {
            Explode();
        }
    }
        

    public void TakeDamage(int damage)
    {
        if (hasExploded) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            PushBack();
        }
    }

    private void PushBack()
    {
        Vector3 direction = transform.position - thePlayer.position;
        direction.y = 0;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(direction.normalized * pushBackForce, ForceMode.Impulse);
        }
    }

    private void Die()
    {
        if (OnDeath != null)
            OnDeath.Invoke(gameObject);
        Debug.Log("El enemigo ha muerto.");
        gameObject.SetActive(false); 
    }

    private void Explode()
    {
        hasExploded = true;

        
        //if (playerHealth != null)
        //{
        //    playerHealth.TakeDamage(damageToPlayer);

        //    Rigidbody playerRb = thePlayer.GetComponent<Rigidbody>();
        //    if (playerRb != null)
        //    {
        //        Vector3 direction = thePlayer.position - transform.position;
        //        direction.y = 0; // No lo empuja hacia arriba
        //        playerRb.AddForce(direction.normalized * pushBackForce, ForceMode.Impulse);
        //    }
        //}
            

        
        if (explosionEffect != null)
        {
            ParticleSystem explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            explosion.Play();
            Destroy(explosion.gameObject, 2f);
        }

        OnDeath?.Invoke(gameObject);

        Destroy(gameObject);
    }

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radio);
    }
}
