using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    [Header("Configurações do Projétil")]
    public float speed = 20f;
    public float lifeTime = 3f; // Tempo para se autodestruir se não bater em nada
    public int damage = 1;

    [Header("Efeitos")]
    public GameObject hitEffectPrefab; // Opcional: Explosãozinha ao bater

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // DESTROY POR TEMPO: Garante que a bala suma depois de X segundos
        Destroy(gameObject, lifeTime);

        // MOVIMENTO: Impulso inicial na direção que o objeto está rotacionado (Eixo X Vermelho)
        rb.linearVelocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignora o próprio Player e outros projéteis para não explodir na saída
        if (other.CompareTag("Player") || other.CompareTag("Projectile")) return;

        // Lógica de Dano (Exemplo)
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Acertou inimigo!");
            // other.GetComponent<EnemyHealth>()?.TakeDamage(damage);
        }

        // Verifica se é uma parede ou objeto sólido (Camada "Ground" ou "Obstacle")
        // Ou simplesmente destrói em qualquer coisa que não seja o player
        HandleImpact();
        // Calcula o dano
        // Ignora player e outros projéteis
        if (other.CompareTag("Player") || other.CompareTag("Projectile")) return;

        // Tenta pegar o script de vida do objeto que foi atingido
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            // AQUI ESTÁ A MÁGICA:
            // 1. Pega o dano base do projétil
            // 2. Passa pela calculadora
            // 3. Aplica o resultado no inimigo
            int finalDamage = DamageCalculator.CalculateFinalDamage(damage);

            enemy.TakeDamage(finalDamage);
        }
        else
        {
            // Bateu na parede ou objeto indestrutível
            // Opcional: Tocar som de 'ricochete'
        }

        HandleImpact(); // Destrói a bala
    }

    void HandleImpact()
    {
        // Se tiver efeito visual, cria ele antes de destruir
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject); // Tchau, bala
    }
}