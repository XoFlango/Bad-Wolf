using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Configurações")]
    public float speed = 3f;

    private Transform player;
    private Rigidbody2D rb;

    [Header("Debug / Testes")]
    public bool isAIActive = true; // Desmarque isso no Inspector para ele parar

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Encontra o jogador automaticamente pela Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void FixedUpdate()
    {

        // 1. CHECAGEM DE TRAVA (NOVO)
        // Se a IA estiver desligada, zera a velocidade e para o código aqui.
        if (!isAIActive)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        // Se o player morreu ou não existe, o inimigo para
        if (player == null)
        {
            rb.linearVelocity = Vector2.zero; // Unity 6 usa linearVelocity
            return;
        }

        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        // 1. Calcula a direção (Player - Eu)
        Vector2 direction = (player.position - transform.position).normalized;

        // 2. Move usando física (respeita paredes)
        rb.linearVelocity = direction * speed;

    }
}