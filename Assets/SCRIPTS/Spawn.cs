using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    [Header("Configuración de Rondas")]
    [Tooltip("personajes por ronda")]
    [SerializeField] private int charactersPerRound = 6;
    [Tooltip("Muertos")]
    [SerializeField] private int charactersKilled = 0;

    [Header("Spawning")]
    [Tooltip("Donde spawnearan en formato lista")]
    [SerializeField] private List<GameObject> enemyPrefabs; 
    [SerializeField] private Transform[] spawnPoints;
    [Tooltip("max de personajes en escena")]
    [SerializeField] private int maxCharacterCountInScene = 3;
    [Tooltip("Cuantos instanciaran max")]
    [SerializeField] private int maxCharacterInstancesinQueue = 20;
    [Tooltip("temporalizador de spwawn")]
    [SerializeField] private float spawnRate = 2f;

    private int charactersSpawnedThisRound = 0;
    private int charactersAliveInScene = 0;
    private float spawnTimer = 0f;

    private void Update()
    {
        if (enemyPrefabs.Count == 0) return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnRate)
        {
            spawnTimer = 0f;
            TrySpawnCharacter();
        }
    }

    private void TrySpawnCharacter()
    {
        if (charactersSpawnedThisRound >= charactersPerRound) return;
        if (charactersAliveInScene >= maxCharacterCountInScene) return;
        if (maxCharacterInstancesinQueue <= 0) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        enemy.SetActive(true); 

        charactersSpawnedThisRound++;
        charactersAliveInScene++;
        maxCharacterInstancesinQueue--;

        EnemyNotifier notifier = enemy.GetComponent<EnemyNotifier>();
        if (notifier == null) notifier = enemy.AddComponent<EnemyNotifier>();
        notifier.SetSpawner(this);
    }

    public void NotifyEnemyKilled()
    {
        charactersKilled++;
        charactersAliveInScene--;

        if (charactersKilled >= charactersPerRound)
        {
            StartNextRound();
        }
    }

    private void StartNextRound()
    {
        charactersPerRound += 2;
        charactersKilled = 0;
        charactersSpawnedThisRound = 0;
        Debug.Log($"Nueva ronda: {charactersPerRound} enemigos");
    }
}