using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heatlh : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHearts = 4; // Número máximo de corazones
    private int currentHearts; // Corazones actuales

    [Header("UI Settings")]
    public Image[] heartImages; // Array de imágenes de corazones en el Canvas

    [Header("Respawn Settings")]
    public GameObject respawnPoint; // Punto de respawn

    private void Start()
    {
        currentHearts = maxHearts;
        UpdateHeartsUI();
    }

    public void GetDamage(int damage)
    {
        currentHearts -= damage;
        if (currentHearts < 0) currentHearts = 0;

        UpdateHeartsUI();

        if (currentHearts == 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHearts += amount;
        if (currentHearts > maxHearts) currentHearts = maxHearts;

        UpdateHeartsUI();
    }

    private void Die()
    {
        RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.transform.position;
            currentHearts = maxHearts; // Reinicia la salud
            UpdateHeartsUI();
        }
    }
    

    private void UpdateHeartsUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentHearts)
            {
                heartImages[i].enabled = true; // Muestra el corazón lleno
            }
            else
            {
                heartImages[i].enabled = false; // Oculta el corazón vacío
            }
        }
    }
}

