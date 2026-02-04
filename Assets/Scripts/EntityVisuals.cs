using UnityEngine;
using System.Collections;

public class EntityVisuals : MonoBehaviour
{
    [Header("Configurações do Waddle (Andar)")]
    public float waddleFrequency = 15f; // Quão rápido ele balança
    public float waddleAmplitude = 5f;  // Quantos graus ele inclina

    [Header("Configurações do Stretch (Esticar)")]
    public Vector2 stretchAmount = new Vector2(0.8f, 1.2f); // X esmaga, Y estica
    public float stretchDuration = 0.15f; // Tempo do efeito

    private Rigidbody2D rb;
    private bool isFacingRight = true;
    private Vector3 originalScale;
    private bool isStretching = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (rb == null) return;

        // Verifica se está se movendo (magnitude da velocidade > 0.1)
        bool isMoving = rb.linearVelocity.magnitude > 0.1f;

        HandleWaddle(isMoving);
        HandleFlip();
    }

    void HandleWaddle(bool isMoving)
    {
        if (isMoving)
        {
            // Calcula a rotação senoidal baseada no tempo
            float zRotation = Mathf.Sin(Time.time * waddleFrequency) * waddleAmplitude;
            transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
        }
        else
        {
            // Se parar, volta suavemente a rotação para 0
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * 10f);
        }
    }

    void HandleFlip()
    {
        // Se a velocidade X for significativa, verificamos o lado
        float velocityX = rb.linearVelocity.x;

        if (Mathf.Abs(velocityX) > 0.1f)
        {
            // Se move para direita e olha para esquerda -> Vira
            if (velocityX > 0 && !isFacingRight)
            {
                Flip();
            }
            // Se move para esquerda e olha para direita -> Vira
            else if (velocityX < 0 && isFacingRight)
            {
                Flip();
            }
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        // Inverte o sinal do X da escala original
        // Usamos originalScale para garantir que não acumule erros de float ou stretch
        float newScaleX = originalScale.x * (isFacingRight ? 1 : -1);

        // Aplica o Flip imediatamente
        Vector3 targetScale = new Vector3(newScaleX, originalScale.y, originalScale.z);
        transform.localScale = targetScale;

        // Inicia o efeito de Stretch (Gelatina)
        if (!isStretching) StartCoroutine(DoStretchEffect(targetScale));
    }

    IEnumerator DoStretchEffect(Vector3 targetBaseScale)
    {
        isStretching = true;

        float timer = 0f;

        // 1. Estica (Vai para o formato "Alongado")
        Vector3 stretchedScale = new Vector3(
            targetBaseScale.x * stretchAmount.x, // Esmaga X
            targetBaseScale.y * stretchAmount.y, // Estica Y
            targetBaseScale.z
        );

        while (timer < stretchDuration)
        {
            timer += Time.deltaTime;
            // Lerp simples para ir e voltar poderia ser complexo, 
            // aqui vamos fazer um "ping-pong" rápido: vai para o stretch e volta.

            float progress = timer / stretchDuration;

            // Curva de sino (Sobe e desce): Mathf.Sin(progress * PI)
            // Quando progress é 0 ou 1, o valor é 0 (Escala Normal). 
            // Quando progress é 0.5, o valor é 1 (Escala Esticada).
            float curve = Mathf.Sin(progress * Mathf.PI);

            transform.localScale = Vector3.Lerp(targetBaseScale, stretchedScale, curve);

            yield return null;
        }

        // Garante que voltou ao tamanho exato no final
        transform.localScale = targetBaseScale;
        isStretching = false;
    }
}