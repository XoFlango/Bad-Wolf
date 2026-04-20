using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Configurações Base")]
    public float speed = 3f;
    public float stoppingDistance = 0.8f;

    [Header("Sistema de Escudo (IA)")]
    public LayerMask obstacleLayer;
    public float detectionDistance = 1.8f;
    public float whiskerAngle = 35f;
    [Tooltip("Deve ser uns 20% maior que o seu Collider real")]
    public float whiskerRadius = 0.6f;

    private Transform player;
    private Rigidbody2D rb;
    private Vector2 currentDirection;

    public bool isAIActive = true;

    // Linhas visuais (Lasers)
    private LineRenderer lineFront, lineLeft, lineRight;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        currentDirection = transform.up;
        lineFront = CriarLinhaVisual("L_F");
        lineLeft = CriarLinhaVisual("L_E");
        lineRight = CriarLinhaVisual("L_D");
    }

    void FixedUpdate()
    {
        if (!isAIActive || player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        MoveWithPrioritySteering();
    }

    void MoveWithPrioritySteering()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= stoppingDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 dirPlayer = (player.position - transform.position).normalized;

        // --- SENSORES ---
        Vector2 vLeft = Quaternion.Euler(0, 0, whiskerAngle) * dirPlayer;
        Vector2 vRight = Quaternion.Euler(0, 0, -whiskerAngle) * dirPlayer;

        RaycastHit2D hitF = Physics2D.CircleCast(transform.position, whiskerRadius, dirPlayer, detectionDistance, obstacleLayer);
        RaycastHit2D hitL = Physics2D.CircleCast(transform.position, whiskerRadius, vLeft, detectionDistance, obstacleLayer);
        RaycastHit2D hitR = Physics2D.CircleCast(transform.position, whiskerRadius, vRight, detectionDistance, obstacleLayer);

        Vector2 targetDir;

        // --- LÓGICA DE PRIORIDADE ---
        // Se houver QUALQUER bloqueio, o modo é DESVIO TOTAL
        if (hitF || hitL || hitR)
        {
            // Calculamos um vetor de fuga baseado na 'normal' da parede batida
            // Usamos o hit mais próximo como referência
            RaycastHit2D closestHit = hitF;
            if (!hitF && hitL) closestHit = hitL;
            else if (!hitF && hitR) closestHit = hitR;

            // Criamos uma direção paralela à parede para contornar
            Vector2 avoidanceDir = Vector2.Perpendicular(closestHit.normal);

            // Garantimos que ele contorne pelo lado que não tem parede
            if (Vector2.Dot(avoidanceDir, dirPlayer) < 0) avoidanceDir = -avoidanceDir;

            targetDir = (avoidanceDir + closestHit.normal * 0.5f).normalized;
        }
        else
        {
            // CAMINHO LIVRE: Prioridade é caçar o player
            targetDir = dirPlayer;
        }

        // Suavização e Aplicação
        float tSpeed = (hitF || hitL || hitR) ? 15f : 5f;
        currentDirection = Vector2.Lerp(currentDirection, targetDir, Time.fixedDeltaTime * tSpeed).normalized;
        rb.linearVelocity = currentDirection * speed;

        // Visualização
        AtualizarLinha(lineFront, dirPlayer, hitF);
        AtualizarLinha(lineLeft, vLeft, hitL);
        AtualizarLinha(lineRight, vRight, hitR);
    }

    LineRenderer CriarLinhaVisual(string nome)
    {
        GameObject obj = new GameObject(nome);
        obj.transform.SetParent(this.transform);
        LineRenderer lr = obj.AddComponent<LineRenderer>();
        lr.positionCount = 2;

        // As linhas agora vão ter a espessura visual exata do raio de detecção!
        lr.startWidth = whiskerRadius * 2f;
        lr.endWidth = whiskerRadius * 2f;

        // Opacidade de 50% para você conseguir ver através da linha gordinha
        Color transparente = new Color(1, 1, 1, 0.3f);
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = transparente;
        lr.endColor = transparente;
        lr.sortingOrder = -1; // Desenha atrás do inimigo

        return lr;
    }

    void AtualizarLinha(LineRenderer lr, Vector2 dir, bool bateu)
    {
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + (Vector3)(dir * detectionDistance));

        // Cor mais transparente para os laseres gordinhos
        Color cor = bateu ? new Color(1, 0, 0, 0.4f) : new Color(0, 1, 0, 0.4f);
        lr.startColor = cor;
        lr.endColor = cor;
    }

    // Desenha o círculo no final da linha na aba Scene para ajudar a calibrar
    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return; // Só desenha no modo edição
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, whiskerRadius);
    }
}