using UnityEngine;
using System.Collections; // Necessário para IEnumerator

public class EnemyHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Configurações de Armadura")]
    public int maxArmor = 0;
    public int currentArmor;

    [Range(0f, 1f)]
    public float armorAbsorption = 0.7f;

    [Header("Feedback Visual (Opcional)")]
    public SpriteRenderer spriteRenderer;

    // --- CORREÇÃO: Variáveis para controlar o Flash sem bugar ---
    private Color defaultColor;
    private Coroutine currentFlashRoutine;
    // -----------------------------------------------------------

    void Start()
    {
        currentHealth = maxHealth;
        currentArmor = maxArmor;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // CORREÇÃO: Salva a cor original (Branca) UMA VEZ SÓ no início.
        // Assim, não importa se ele ficar vermelho, a gente sempre lembra que o original era branco.
        if (spriteRenderer != null)
        {
            defaultColor = spriteRenderer.color;
        }
    }

    public void TakeDamage(int rawDamage)
    {
        int damageToHealth = rawDamage;

        // --- LÓGICA DE ARMADURA ---
        if (currentArmor > 0)
        {
            int damageAbsorbed = Mathf.RoundToInt(rawDamage * armorAbsorption);
            int damageLeak = rawDamage - damageAbsorbed;

            currentArmor -= damageAbsorbed;

            if (currentArmor <= 0)
            {
                currentArmor = 0;
                Debug.Log($"<color=orange>{gameObject.name}: ARMADURA QUEBRADA!</color>");
            }
            else
            {
                Debug.Log($"<color=cyan>Armadura tankou: {damageAbsorbed}. Passou pra vida: {damageLeak}</color>");
            }

            damageToHealth = damageLeak;
        }

        // --- APLICA DANO NA VIDA ---
        currentHealth -= damageToHealth;

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
            // CORREÇÃO: Chama o novo método TriggerFlash em vez de StartCoroutine direto
            Color flashColor = (currentArmor > 0) ? Color.blue : Color.red;
            TriggerFlash(flashColor);
        }
    }

    void Die()
    {
        LootBag lootBag = GetComponent<LootBag>();
        if (lootBag != null)
        {
            lootBag.InstantiateLoot(transform.position);
        }

        Debug.Log($"{gameObject.name} morreu.");
        // Opcional: Efeito de morte/partículas aqui
        Destroy(gameObject);
    }

    // --- SISTEMA DE FLASH CORRIGIDO ---

    void TriggerFlash(Color targetColor)
    {
        if (spriteRenderer == null) return;

        // 1. Se já estiver piscando (por causa de um tiro anterior da shotgun), CANCELA o anterior.
        if (currentFlashRoutine != null)
        {
            StopCoroutine(currentFlashRoutine);
        }

        // 2. Garante que volta pra cor padrão antes de pintar de novo (opcional, mas evita mistura de cores)
        spriteRenderer.color = defaultColor;

        // 3. Inicia o novo flash
        currentFlashRoutine = StartCoroutine(FlashRoutine(targetColor));
    }

    IEnumerator FlashRoutine(Color color)
    {
        spriteRenderer.color = color; // Pinta (Vermelho ou Azul)

        yield return new WaitForSeconds(0.1f); // Espera

        spriteRenderer.color = defaultColor; // Volta para o BRANCO original (e não a cor anterior)

        currentFlashRoutine = null;
    }
}