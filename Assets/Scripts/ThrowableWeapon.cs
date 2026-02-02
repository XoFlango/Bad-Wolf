using UnityEngine;

public class ThrowableWeapon : MonoBehaviour
{
    [Header("Configuração de Combate")]
    public int damage = 10;
    public bool isFlying = false;

    [Header("Física de Chão")]
    public float groundDrag = 5f;        // O quanto ela freia ao bater no chão (Quanto maior, mais rápido para)
    public float airDrag = 0.5f;         // Resistência do ar enquanto voa
    public float rotationDrag = 5f;      // O quanto ela para de girar ao bater no chão

    private Rigidbody2D rb;
    private WeaponAim aimScript;
    private Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        aimScript = GetComponent<WeaponAim>();
        col = GetComponent<Collider2D>();
    }

    // --- ARREMESSAR ---
    public void PrepareThrow(Vector2 direction, float force)
    {
        isFlying = true;

        SetupPhysics(true); // Ativa física dinâmica

        // Configuração de Voo: Pouco atrito para ir longe
        rb.linearDamping = airDrag; // (Nota: Unity 6 usa 'linearDamping'. Se der erro, use 'drag')
        rb.angularDamping = 0.5f;   // (Nota: Unity 6 usa 'angularDamping'. Se der erro, use 'angularDrag')

        // Desliga a mira
        if (aimScript) aimScript.enabled = false;

        // Aplica a força
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        // Efeito de girar a arma no ar
        rb.angularVelocity = -700f * Mathf.Sign(direction.x);
    }

    // --- SOLTAR NO PÉ (DROP) ---
    public void PrepareDrop(Vector3 dropPosition)
    {
        isFlying = false;

        // 1. Força a posição exata antes de ativar a física
        transform.position = dropPosition;
        transform.rotation = Quaternion.identity; // Reseta rotação (opcional)

        SetupPhysics(true);

        // Configuração de Drop: Muito atrito para não deslizar
        rb.linearDamping = groundDrag;
        rb.angularDamping = rotationDrag;

        if (aimScript) aimScript.enabled = false;

        // Pequeno empurrãozinho aleatório só pra não ficarem 10 armas empilhadas no mesmo pixel exato
        rb.AddForce(new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)), ForceMode2D.Impulse);
    }

    // Função auxiliar para ativar/desativar componentes
    private void SetupPhysics(bool active)
    {
        if (active)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
            col.enabled = true;
            col.isTrigger = false; // Bate nas paredes
        }
        else
        {
            rb.simulated = false;
            col.enabled = false;
        }
    }

    // --- COLISÃO ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Se bateu em algo, aumenta o atrito imediatamente para parar
        rb.linearDamping = groundDrag;
        rb.angularDamping = rotationDrag;

        if (!isFlying) return;

        // Lógica de Dano
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Dano no Inimigo: " + damage);
        }

        isFlying = false;
        col.isTrigger = true; // Vira trigger para ser pego de novo
    }
}