using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Configurações de Armadura")]
    public int maxArmor = 0;      // Se for 0, é um inimigo sem armadura
    public int currentArmor;

    [Range(0f, 1f)]
    public float armorAbsorption = 0.7f; // Quanto % do dano a armadura "tanka" (Padrão 70%)

    [Header("Feedback Visual (Opcional)")]
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        currentArmor = maxArmor;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int rawDamage)
    {
        int damageToHealth = rawDamage;

        // --- LÓGICA DE ARMADURA ---
        if (currentArmor > 0)
        {
            // Calcula quanto a armadura vai absorver
            int damageAbsorbed = Mathf.RoundToInt(rawDamage * armorAbsorption);

            // O que sobra vai para a vida
            int damageLeak = rawDamage - damageAbsorbed;

            // Aplica dano na armadura
            currentArmor -= damageAbsorbed;

            // Feedback: Se a armadura quebrar nesse hit
            if (currentArmor <= 0)
            {
                currentArmor = 0;
                Debug.Log($"<color=orange>{gameObject.name}: ARMADURA QUEBRADA!</color>");
            }
            else
            {
                Debug.Log($"<color=cyan>Armadura tankou: {damageAbsorbed}. Passou pra vida: {damageLeak}</color>");
            }

            // O dano final na vida é apenas o que "vazou"
            damageToHealth = damageLeak;
        }

        // --- APLICA DANO NA VIDA ---
        currentHealth -= damageToHealth;

        // Debug simples
        if (currentArmor > 0)
            Debug.Log($"HP: {currentHealth} | Armor: {currentArmor}");
        else
            Debug.Log($"HP: {currentHealth} (Vulnerável)");

        // MORTE
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Pequeno flash visual (Branco = Dano Vida, Azul = Dano Armadura)
            StartCoroutine(FlashColor(currentArmor > 0 ? Color.blue : Color.red));
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} morreu.");
        Destroy(gameObject);
    }

    // Feedback visual rápido
    System.Collections.IEnumerator FlashColor(Color color)
    {
        if (spriteRenderer)
        {
            Color original = spriteRenderer.color;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = original;
        }
    }
}