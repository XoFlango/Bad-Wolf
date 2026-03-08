using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Tipo de Objeto")]
    public bool isPlayer = false; // MARQUE ISSO APENAS NO PERSONAGEM!

    [Header("Status")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Invencibilidade (iFrames)")]
    public float invulnerabilityDuration = 1.0f; // 1 segundo como pediu
    private bool isInvulnerable = false;         // Trava interna

    [Header("Referências")]
    public HealthBar healthBar;
    public GameObject deathPrefab;    // Para o Player: Caixão. Para Spawner: Explosão/Fumaça.
    public SpriteRenderer spriteRenderer; // Para piscar quando tomar dano

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null) healthBar.UpdateHealthUI(currentHealth, maxHealth);
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>(); // Tenta achar no visual
    }

    public void TakeDamage(int damage)
    {
        // Se for Player invencível, ignora
        if (isPlayer && isInvulnerable) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} tomou {damage} de dano. Vida: {currentHealth}");

        // Atualiza a UI se existir
        if (healthBar != null)
        {
            int healthForUI = Mathf.Clamp(currentHealth, 0, maxHealth);
            healthBar.UpdateHealthUI(healthForUI, maxHealth);
        }

        // Efeito de Piscar (Flash) ao tomar dano
        if (spriteRenderer != null) StartCoroutine(FlashRoutine());

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (isPlayer)
        {
            // Só o Player fica invencível após o hit
            StartCoroutine(InvulnerabilityRoutine());
        }

    }

    IEnumerator FlashRoutine()
    {
        Color original = spriteRenderer.color;
        spriteRenderer.color = Color.red; // Fica vermelho no hit
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = original;
    }

    IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;

        // Configuração do Piscar
        float blinkInterval = 0.1f; // Velocidade do piscar (0.1s invisivel, 0.1s visivel)

        Color originalColor = spriteRenderer.color; // Guarda a cor original (Alpha 1)
        Color flashColor = originalColor;
        flashColor.a = 0f; // Alpha 0 = Totalmente Invisível (Ou use 0.2f para semi-transparente)

        // Calcula quando o efeito deve parar
        float endTime = Time.time + invulnerabilityDuration;

        // Enquanto não der o tempo final...
        while (Time.time < endTime)
        {
            // 1. Fica invisível
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(blinkInterval);

            // 2. Fica visível
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(blinkInterval);
        }

        // SEGURANÇA: Garante que o player volte ao normal no final
        spriteRenderer.color = originalColor;
        isInvulnerable = false;
    }

    void Die()
    {
        // 1. Cria o efeito de morte (Caixão ou Explosão)
        if (deathPrefab != null)
        {
            Instantiate(deathPrefab, transform.position, Quaternion.identity);
        }


        // 2. Comportamento Específico
        if (isPlayer)
        {
            Debug.Log("Game Over!");
            GameMenuController menuManager = Object.FindFirstObjectByType<GameMenuController>();
            if (menuManager != null) menuManager.GameOver();
        }
        else
        {
            Debug.Log("Alvo Destruído!");
            // Se for um inimigo ou spawner, pode dar pontos aqui futuramente
        }

        // 3. Destrói o objeto
        Destroy(gameObject);
    }
}