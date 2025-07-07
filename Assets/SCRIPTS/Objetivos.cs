using UnityEngine;

// Script para el Target
public class Objetivos : MonoBehaviour
{
    private ManejoDeCuartos manager;

    public void SetManager(ManejoDeCuartos manejo) //
    {
        manager = manejo;
    }

    // Llama esto cuando el target sea "destruido" 
    public void ObjetoDestruido()
    {
        if (manager != null)
        {
            manager.OnTargetDestroyed(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}

