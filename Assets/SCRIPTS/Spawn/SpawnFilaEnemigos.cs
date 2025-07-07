
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnFilaEnemigos : MonoBehaviour
{
    [SerializeField] private int maxCharacterCountInScene = 6; // Máximo de enemigos activos
    [SerializeField] private int maxCharacterInstancesInQueue = 20; // Máximo de instancias en la pool
    [SerializeField] private float spawnRate = 1f; // Delay entre spawns
    [SerializeField] private GameObject characterToSpawn; // Prefab del enemigo
    [SerializeField] private Transform[] spawnPoints; // Puntos de spawn

    private Queue<GameObject> characterQueue;
    private int charactersInScene = 0; // Enemigos activos en escena
    private int totalToKillThisRound = 10; // Enemigos a matar en la ronda actual
    private int charactersKilled = 0; // Enemigos muertos en la ronda
    private int charactersSpawnedThisRound = 0; // Enemigos spawneados en la ronda

    private void Start()
    {
        // Inicializa variables y pool
        totalToKillThisRound = 10;
        charactersKilled = 0;
        charactersSpawnedThisRound = 0;
        charactersInScene = 0;
        characterQueue = new Queue<GameObject>();

        // Crea la pool de enemigos
        for (int i = 0; i < maxCharacterInstancesInQueue; i++)
        {
            GameObject instance = Instantiate(characterToSpawn);
            instance.SetActive(false);
            characterQueue.Enqueue(instance);
        }
        // Spawnea los primeros 6 enemigos
        StartCoroutine(SpawnInitialEnemies());
    }

    // Spawnea los primeros enemigos de la ronda
    private IEnumerator SpawnInitialEnemies()
    {
        // Reinicia contadores
        for (int i = 0; i < maxCharacterCountInScene && charactersSpawnedThisRound < totalToKillThisRound; i++)
        {
            // Spawnea un enemigo
            SpawnCharacter();
            yield return new WaitForSeconds(spawnRate);
        }
    }

    // Spawnea un enemigo si hay espacio y quedan por aparecer en la ronda
    private void SpawnCharacter()
    {
        // Verifica si se puede spawnear un nuevo enemigo
        if (charactersInScene >= maxCharacterCountInScene || characterQueue.Count == 0 || charactersSpawnedThisRound >= totalToKillThisRound) return;

        // Saca un enemigo de la pool y lo activa
        GameObject character = characterQueue.Dequeue();
        if (character == null) return;

        // activa el enemigo y lo posiciona en un punto de spawn aleatorio
        character.SetActive(true);
        int randomSpawn = Random.Range(0, spawnPoints.Length);
        character.transform.SetPositionAndRotation(spawnPoints[randomSpawn].position, spawnPoints[randomSpawn].rotation);

        // incrementa loa personajes en escena y los spawneados en la ronda
        charactersInScene++;
        charactersSpawnedThisRound++;
        Debug.Log("Personajes en escena: " + charactersInScene);

        //evento de muerte del enemigo
        Enemy enemy = character.GetComponent<Enemy>();
        if (enemy != null)
            enemy.OnDeath += OnCharacterKilled;
    }

    private void OnCharacterKilled(GameObject character)
    {
        // Se llama cuando un enemigo es asesinado
        charactersInScene--;

        // Incrementa el contador de enemigos muertoos
        charactersKilled++;
        Debug.Log("Personajes en escena: " + charactersInScene);
        Debug.Log("Personajes muertos: " + charactersKilled);

        // Desactiva el enemigo y lo devuelve a la pool
        if (character != null)
        {
            character.SetActive(false);
            characterQueue.Enqueue(character);
            Enemy enemy = character.GetComponent<Enemy>();
            if (enemy != null)
                enemy.OnDeath -= OnCharacterKilled;
        }

        // Si quedan enemigos por matar en la ronda, spawnea uno nuevo tras un delay
        if (charactersKilled < totalToKillThisRound)
        {
            StartCoroutine(SpawnWithDelay());
        }
        else
        {
            // Nueva ronda: suma 2 al total a matar y reinicia contadores
            Debug.Log("¡Fin de la ronda! Iniciando nueva ronda...");
            Debug.Log("Ronda anterior: " + totalToKillThisRound + " enemigos. Nueva ronda: " + (totalToKillThisRound + 2) + " enemigos.");

            // Suma 2 al total de enemigos a matar en la nueva ronda
            totalToKillThisRound += 2;

            // Reinicia contadores
            charactersKilled = 0;
            charactersSpawnedThisRound = 0;
            StartCoroutine(SpawnInitialEnemies());
        }
    }

    private IEnumerator SpawnWithDelay() //esperar antes de spawnear un nuevo enemigo
    {
        yield return new WaitForSeconds(spawnRate);
        SpawnCharacter();
    }
}
