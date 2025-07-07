
using System.Collections.Generic;
using UnityEngine;

public class ROOMS : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject objetivo;
    [SerializeField] private int pool = 8;
    [Tooltip("Puntos donde apareceran los objetivos")]
    [SerializeField] private Transform[] apariciones;
    [Tooltip("Llaves que abrirra el cuarto final")]
    [SerializeField] private GameObject llave;

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
        for (int i = 0; i < pool; i++)
        {
            GameObject newTarget = Instantiate(objetivo, transform);
            newTarget.SetActive(false);
            var refScript = newTarget.AddComponent<TargetReference>();
            refScript.myRoom = this;
            poolObjetivos.Enqueue(newTarget);
        }
    }

    void SpawnTargets()
    {
        int i = 0;
        foreach (Transform spawnPoint in apariciones)
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

    public void OnTargetDestroyed(GameObject target)
    {
        target.SetActive(false);
        poolObjetivos.Enqueue(target);
        objetivosDestruidos++;
        

        if (objetivosDestruidos >= apariciones.Length && llave != null)
        {
            llave.SetActive(true);
           
        }
    }
}
public class AlreadyDestroyed : MonoBehaviour { } //esta clase es para evitar que el mismo objetivo sea destruido varias veces en bulletBehaviour

public class TargetReference : MonoBehaviour // Referencia al ROOMS desde BulletBehaviour
{
    public ROOMS myRoom;
}
