using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Controle Mestre")]
    public bool canSpawn = false; //controle para ativar/desativar spawn.

    [Header("Detecção do Player")]
    public float activationRadius = 8f; // Qual a distância para o Spawner acordar?
    private Transform player;

    [Header("Configurações")]
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public float spawnRadius = 3f;

    void Start()
    {

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        // Inicia o loop eterno de spawn
        StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        // Se o player morreu ou não foi encontrado, interrompe a checagem
        if (player == null) return;

        // Calcula a distância exata entre o Spawner e o Player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Se o player entrar no raio de ativação, liga a chave. Se sair, desliga.
        if (distanceToPlayer <= activationRadius)
        {
            canSpawn = true;
        }
        else
        {
            canSpawn = false;
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Espera o tempo definido
            yield return new WaitForSeconds(spawnInterval);

            // SÓ SPAWNA SE A CHAVE ESTIVER LIGADA
            if (canSpawn && enemyPrefab != null)
            {
                SpawnEnemy();
            }
        }
    }

    void SpawnEnemy()
    {
        // Gera uma posição aleatória dentro do círculo
        Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = transform.position + (Vector3)randomPoint;

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    // --- FUNÇÕES PÚBLICAS PARA OUTROS SCRIPTS CHAMAREM ---

    public void EnableSpawner()
    {
        canSpawn = true;
    }

    public void DisableSpawner()
    {
        canSpawn = false;
    }

    // Desenha a área no editor para você ver onde eles vão nascer
    private void OnDrawGizmos()
    {
        // Círculo Amarelo: Onde os inimigos nascem
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        // Círculo Verde: A área de detecção do Player
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }
}