using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private Image[] corazones;
    [SerializeField] private Respawn respawn; 

    private int currentHealth;
    private int maxHealth = 4;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHearts();
    }

    public void GetDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHearts();

        if (currentHealth <= 0)
        {
            RespawnPlayer();
        }
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < corazones.Length; i++)
        {
            corazones[i].gameObject.SetActive(i < currentHealth);
        }
    }

    private void RespawnPlayer()
    {
        currentHealth = maxHealth;
        UpdateHearts();

        respawn.TeleportPlayer(transform);
    }
}
