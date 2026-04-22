using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Configurações Base")]
    public float speed = 3f;
    public float stoppingDistance = 0.8f;

    [Header("Sistema de Colisão (IA)")]
    public LayerMask obstacleLayer;
    public float colisorRaio = 0.5f;
    public float bumpDistancia = 0.15f;

    private Transform player;
    private Rigidbody2D rb;
    public bool isAIActive = true;

    // Memória
    private bool isSliding = false;
    private float slideWinding = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void FixedUpdate()
    {
        if (!isAIActive || player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        MoveAndSlideWithMemory();
    }

    void MoveAndSlideWithMemory()
    {
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer <= stoppingDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 dirToPlayer = (player.position - transform.position).normalized;

        // ================================================================
        // A SOLUÇÃO ANTI-QUINA: Trocamos Raycast por CircleCast
        // Agora ele projeta a largura do próprio corpo (colisorRaio) para frente.
        // Ele só vai tentar ir para o Player quando tiver espaço para passar sem raspar!
        // ================================================================
        RaycastHit2D wallBetweenUs = Physics2D.CircleCast(transform.position, colisorRaio, dirToPlayer, distToPlayer, obstacleLayer);

        if (wallBetweenUs.collider == null)
        {
            // O caminho está 100% livre para a LARGURA do inimigo! Esquece a parede e vai reto.
            isSliding = false;
            rb.linearVelocity = dirToPlayer * speed;
            return;
        }

        // 2. Batida Mínima (A Aura)
        RaycastHit2D bumpHit = Physics2D.CircleCast(transform.position, colisorRaio + bumpDistancia, dirToPlayer, 0.05f, obstacleLayer);

        if (bumpHit.collider != null)
        {
            Vector2 slideDirection = Vector2.Perpendicular(bumpHit.normal);

            // 3. O MOMENTO DA DECISÃO
            if (!isSliding)
            {
                float dot = Vector2.Dot(dirToPlayer, slideDirection);

                // O Radar de Empate
                if (Mathf.Abs(dot) < 0.1f)
                {
                    float distanciaLadoA = ScanWallLength(slideDirection, bumpHit.normal);
                    float distanciaLadoB = ScanWallLength(-slideDirection, bumpHit.normal);
                    slideWinding = (distanciaLadoA <= distanciaLadoB) ? 1f : -1f;
                }
                else
                {
                    slideWinding = (dot > 0) ? 1f : -1f;
                }

                isSliding = true;
            }

            // 4. APLICAÇÃO DO DESLIZAMENTO COM AFASTAMENTO DE QUINA
            Vector2 finalMove = slideDirection * slideWinding;

            // Aqui damos um pequeno "chega pra lá" na parede (0.15f) para garantir que 
            // a barriga do inimigo não raspe no Tilemap enquanto ele escorrega.
            finalMove = (finalMove - bumpHit.normal * 0.15f).normalized;

            rb.linearVelocity = finalMove * speed;
        }
        else
        {
            isSliding = false;
            rb.linearVelocity = dirToPlayer * speed;
        }
    }

    // ================================================================
    // O RADAR: Pula de metro em metro "cutucando" o Tilemap
    // ================================================================
    float ScanWallLength(Vector2 testDirection, Vector2 wallNormal)
    {
        float step = 1f; // Pula de 1 em 1 unidade
        float maxScan = 25f; // Limite de tamanho do muro (previne lag infinito)

        for (float d = step; d <= maxScan; d += step)
        {
            // Move um ponto imaginário paralelamente à parede
            Vector2 checkPos = (Vector2)transform.position + (testDirection * d);

            // Atira um raio DE VOLTA para a parede
            RaycastHit2D hit = Physics2D.Raycast(checkPos, -wallNormal, bumpDistancia * 3f, obstacleLayer);

            // Se o raio não bater em nada, a parede acabou ali!
            if (hit.collider == null)
            {
                return d;
            }
        }
        return maxScan; // Se não achou o fim, assume que a parede é gigante
    }

    private void OnDrawGizmos()
    {
        if (player == null) return;
        Vector2 dirToPlayer = (player.position - transform.position).normalized;
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, dirToPlayer * distToPlayer);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + (dirToPlayer * 0.05f), colisorRaio + bumpDistancia);
    }
}