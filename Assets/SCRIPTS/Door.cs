
using System.Collections;
using TMPro;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private int llavesNecesarias; 
    private bool estaDesbloqueada = false; 

    private InventoryHandler playerInventory; 

    void Start()
    {
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryHandler>();
    }


    void Update()
    {
        if (playerInventory._Inventario.Count >= llavesNecesarias) 
        {
            StartCoroutine(PuertaDesbloqueada()); 
        }
    }


    private IEnumerator PuertaDesbloqueada() 
    {
        if (!estaDesbloqueada) 
        {
            estaDesbloqueada = true;
            yield return new WaitForSeconds(1.5f); 
            transform.gameObject.SetActive(false); 
        }
    }
}
