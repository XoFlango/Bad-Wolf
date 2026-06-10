using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Configurações Base")]
    public float baseSpeed = 3f;

    [Tooltip("Variação aleatória na velocidade. Ex: 0.5 faz a velocidade variar de 2.5 a 3.5.")]
    public float speedVariance = 0.5f;
    public float stoppingDistance = 0.8f;

    // Esta é a velocidade real e única que este inimigo sorteou para a vida dele
    private float currentSpeed;

    [Header("Campo de Visão")]
    public float visionRadius = 7f;
    public bool isAlert = false;

    [Header("Sistema de Colisão")]
    public LayerMask obstacleLayer;
    public float colisorRaio = 0.3f;

    private Transform player;
    private Rigidbody2D rb;
    public bool isAIActive = true;

    // Estados da Bússola
    private bool isAvoiding = false;
    private Vector2 lockedDirection;
    private Vector2[] compass = {
        Vector2.up, new Vector2(1,1).normalized, Vector2.right, new Vector2(1,-1).normalized,
        Vector2.down, new Vector2(-1,-1).normalized, Vector2.left, new Vector2(-1,1).normalized
    };

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // ================================================================
        // O SORTEIO DE VELOCIDADE
        // Cada inimigo calcula sua própria velocidade assim que nasce
        // ================================================================
        currentSpeed = baseSpeed + Random.Range(-speedVariance, speedVariance);

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

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer > visionRadius)
        {
            isAlert = false;
            isAvoiding = false;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        isAlert = true;

        ExecuteCompassMovement(distToPlayer);
    }

    void ExecuteCompassMovement(float distToPlayer)
    {
        if (distToPlayer <= stoppingDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 dirToPlayer = (player.position - transform.position).normalized;

        RaycastHit2D sightCheck = Physics2D.Raycast(transform.position, dirToPlayer, distToPlayer, obstacleLayer);

        if (sightCheck.collider == null)
        {
            RaycastHit2D faceCheck = Physics2D.CircleCast(transform.position, colisorRaio, dirToPlayer, 0.2f, obstacleLayer);

            if (faceCheck.collider == null)
            {
                isAvoiding = false;
                // Substituímos o "speed" antigo pela velocidade sorteada "currentSpeed"
                rb.linearVelocity = dirToPlayer * currentSpeed;
                return;
            }
        }

        if (!isAvoiding)
        {
            isAvoiding = true;
            lockedDirection = SpinCompass(dirToPlayer);
        }
        else
        {
            RaycastHit2D pathCheck = Physics2D.CircleCast(transform.position, colisorRaio, lockedDirection, 0.2f, obstacleLayer);
            if (pathCheck.collider != null)
            {
                lockedDirection = SpinCompass(lockedDirection);
            }
        }

        // Substituímos o "speed" antigo pela velocidade sorteada "currentSpeed"
        rb.linearVelocity = lockedDirection * currentSpeed;
    }

    Vector2 SpinCompass(Vector2 baseDir)
    {
        int bestIndex = 0;
        float maxDot = -Mathf.Infinity;
        for (int i = 0; i < 8; i++)
        {
            float dot = Vector2.Dot(baseDir, compass[i]);
            if (dot > maxDot) { maxDot = dot; bestIndex = i; }
        }

        int spin = Random.value > 0.5f ? 1 : -1;
        for (int i = 0; i < 8; i++)
        {
            int testIndex = (bestIndex + (i * spin) + 8) % 8;
            Vector2 testDir = compass[testIndex];
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, colisorRaio, testDir, 0.3f, obstacleLayer);
            if (hit.collider == null) return testDir;
        }
        return -baseDir;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isAlert ? Color.red : Color.blue;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        if (player != null && isAlert)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}