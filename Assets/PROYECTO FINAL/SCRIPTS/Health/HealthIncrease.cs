using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthIncrease : MonoBehaviour
{
    public int healingAmount = 1; // Cantidad de curación

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Heatlh health = other.gameObject.GetComponent<Heatlh>();
            if (health != null)
            {
                health.Heal(healingAmount);
                Destroy(gameObject); // Destruir la poción después de usarla
            }
        }
    }
}