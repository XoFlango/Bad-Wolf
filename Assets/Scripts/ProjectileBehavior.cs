using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    [Header("Configurações")]
    public float speed = 20f;
    public int damage = 10;
    public float lifeTime = 5f; // Tempo para destruir se não bater em nada

    [Header("Efeitos")]
    public GameObject impactEffect; // Opcional: Partícula ao bater

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // A bala já nasce voando para a direita (o WeaponController já rotacionou ela)
        rb.linearVelocity = transform.right * speed;

        // Segurança: Destrói depois de X segundos para não lagar o jogo
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // --- A LISTA BRANCA (WHITELIST) ---
        // Verificamos se o objeto tem uma das tags permitidas
        Debug.Log($"[BALA] Nasci e bati em: {hitInfo.name} (Tag: {hitInfo.tag})");
        bool isEnemy = hitInfo.CompareTag("Enemy");
        bool isProp = hitInfo.CompareTag("Prop");
        bool isWall = hitInfo.CompareTag("Wall"); // Paredes/Cenário

        // Se NÃO for nenhum desses três, ignora e deixa passar
        if (!isEnemy && !isProp && !isWall)
        {
            // Debug opcional para saber o que ignorou
            Debug.Log($"[Bala] Ignorou colisão com: {hitInfo.name}");
            return;
        }

        // --- A PARTIR DAQUI, É COLISÃO VÁLIDA ---

        // 1. Tenta causar dano (Só Inimigos e Props têm vida)
        // --- COLISÃO VÁLIDA ---

        // CASO 1: É INIMIGO? (Usa o script de vida do inimigo)
        if (isEnemy)
        {
            // Substitua 'EnemyHealth' pelo nome EXATO do script de vida dos seus inimigos
            EnemyHealth enemyHealth = hitInfo.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                // O script do inimigo deve ter o método TakeDamage igual ao Health
                enemyHealth.TakeDamage(damage);
            }
          
        }
        // CASO 2: É PROP/OBJETO? (Usa o script de vida genérico)
        else if (isProp)
        {
            Health propHealth = hitInfo.GetComponent<Health>();

            if (propHealth != null)
            {
                propHealth.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
}

