using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public Item item;

    private InventoryHandler inventory;

    private void Start()
    {
        inventory = FindObjectOfType<InventoryHandler>();
        if (inventory == null)
        {
            Debug.LogError("No se encontró InventoryHandler en la escena.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (inventory != null)
            {
                inventory.AgregarObjeto(item);
                Debug.Log(item.name + " añadida al inventario");
                Destroy(gameObject); // Destruye el objeto para que no se pueda recoger de nuevo
            }
            else
            {
                Debug.LogWarning("InventoryHandler no asignado.");
            }
        }
    }
}
