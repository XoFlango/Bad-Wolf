using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Configurações")]
    public float speed = 3f;

    // NOVO: Distância para parar de andar e evitar tremedeira
    // Ajuste para um valor pequeno (ex: 0.8 ou 1.0)
    public float stoppingDistance = 0.8f;

    private Transform player;
    private Rigidbody2D rb;

    [Header("Debug / Testes")]
    public bool isAIActive = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (!isAIActive)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        // 1. Calcula a distância atual entre o Inimigo e o Player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 2. Lógica de Parada (Stopping Distance)
        // Se estiver LONGE, anda.
        if (distanceToPlayer > stoppingDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
        // Se estiver PERTO, freia totalmente.
        else
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f; // Garante que não gire
        }
    }
}