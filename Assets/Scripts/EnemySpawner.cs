using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("O que Spawnar")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints; // Arraste os pontos aqui

    [Header("Tempo")]
    public float spawnInterval = 2f;

    void Start()
    {
        // Inicia o loop infinito de spawn
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || enemyPrefab == null) return;

        // 1. Escolhe um ponto aleatório
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform chosenPoint = spawnPoints[randomIndex];

        // 2. Cria o inimigo
        Instantiate(enemyPrefab, chosenPoint.position, Quaternion.identity);
    }
}