using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHandler : MonoBehaviour
{
    [SerializeField] private List<Item> inventario;
    public List<Item> _Inventario { get => inventario; }

    public int indice;
    public int maxCapacity = 24;
    public int numero;

    [Header("Configuración de destrucción")]
    [SerializeField] private GameObject objetoADestruir;
    [SerializeField] private int itemsNecesarios = 8;

    private bool objetoDestruido = false;

    public int keyCount = 0;

    public void AddKey()
    {
        keyCount++;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            TirarObjeto();
        }

        if (inventario.Count >= itemsNecesarios && objetoADestruir != null && !objetoDestruido)
        {
            Destroy(objetoADestruir);
            objetoDestruido = true; // Marca como destruido
            Debug.Log("Objeto destruido por haber recolectado " + itemsNecesarios + " items");
        }
    }

    public void AgregarObjeto(Item item)
    {
        if (inventario.Count < maxCapacity)
        {
            inventario.Add(item);
            if (item.isKey) // Asumiendo que tienes una propiedad para identificar llaves
            {
                keyCount++;
            }
          
        }
        else
        {
            Debug.LogWarning("Inventario lleno!");
        }
    }

    public void TirarObjeto()
    {
        if (inventario.Count > 0 && indice >= 0 && indice < inventario.Count)
        {
            Instantiate(inventario[indice]._prefab, transform.position, transform.rotation);
            inventario.RemoveAt(indice);
            Debug.Log("Item tirado. Total: " + inventario.Count);
        }
    }
}
