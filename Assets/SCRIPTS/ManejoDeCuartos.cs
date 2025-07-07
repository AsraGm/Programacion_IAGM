using System.Collections.Generic;
using UnityEngine;

public class ManejoDeCuartos : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject objetivoPrefab;
    [SerializeField] private int tamañoPool = 8;
    [SerializeField] private Transform[] puntosRespawnPrefab; // Asigna 8 posiciones en el inspector
    [SerializeField] private GameObject llave; // Asigna la llave de este cuarto

    private Queue<GameObject> poolObjetivos = new Queue<GameObject>();
    private int objetivosDestruidos = 0;

    void Awake()
    {
        InitializePool();
    }

    void Start()
    {
        SpawnTargets();
        if (llave != null)
            llave.SetActive(false);
    }

    void InitializePool()
    {
        for (int i = 0; i < tamañoPool; i++)
        {
            GameObject newTarget = Instantiate(objetivoPrefab, transform);
            newTarget.SetActive(false);
            // Asigna el manager a cada target para que pueda devolverlo al pool
            newTarget.GetComponent<Objetivos>().SetManager(this);
            poolObjetivos.Enqueue(newTarget);
        }
    }

    void SpawnTargets()
    {
        int i = 0;
        foreach (Transform spawnPoint in puntosRespawnPrefab)
        {
            if (poolObjetivos.Count > 0)
            {
                GameObject target = poolObjetivos.Dequeue();
                target.transform.position = spawnPoint.position;
                target.transform.rotation = spawnPoint.rotation;
                target.SetActive(true);
                i++;
            }
        }
        objetivosDestruidos = 0;
    }

    // Llama esto desde el Target cuando sea destruido
    public void OnTargetDestroyed(GameObject target)
    {
        target.SetActive(false);
        poolObjetivos.Enqueue(target);
        objetivosDestruidos++;

        if (objetivosDestruidos >= tamañoPool && llave != null)
        {
            llave.SetActive(true);
        }
    }
}
