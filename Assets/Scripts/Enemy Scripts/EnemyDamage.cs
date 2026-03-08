using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [Header("Configuração de Dano")]
    public int minDamage = 5;
    public int maxDamage = 10;
    public float damageCooldown = 1.0f; // 1 segundo de intervalo entre hits
    private float nextDamageTime = 0f;

    [Header("Hitbox Quadrada (Box)")]
    // Vector2 define Largura (X) e Altura (Y) da caixa
    public Vector2 boxSize = new Vector2(1f, 1f);
    public LayerMask playerLayer;

    void Update()
    {
        // --- MUDANÇA AQUI: Usamos OverlapBox em vez de Circle ---
        // Parâmetros: Posição Central, Tamanho (Vector2), Ângulo (0f), Layer Alvo
        Collider2D playerCollider = Physics2D.OverlapBox(transform.position, boxSize, 0f, playerLayer);

        // Se o radar detectou o player E o cooldown permite atacar...
        if (playerCollider != null && Time.time >= nextDamageTime)
        {
            Health playerHealth = playerCollider.GetComponent<Health>();

            if (playerHealth != null)
            {
                int damage = Random.Range(minDamage, maxDamage + 1);
                playerHealth.TakeDamage(damage);

                // Reseta o cronômetro do inimigo
                nextDamageTime = Time.time + damageCooldown;
            }
        }
    }

    // --- FERRAMENTA VISUAL (Só aparece no Editor) ---
    // Desenha um cubo/quadrado para você ver o tamanho do Hitbox
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Desenha o aramado do cubo na posição do inimigo com o tamanho definido
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}