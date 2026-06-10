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

    [Header("Áudio do Inimigo")] // --- NOVO CABEÇALHO PARA SONS ---
    [Tooltip("Som tocado quando o inimigo sobrevive ao impacto")]
    public AudioClip hitSound;
    [Tooltip("Som tocado quando a vida chega a zero")]
    public AudioClip deathSound;
    private AudioSource audioSource;

    [Header("Feedback Visual (Opcional)")]
    public SpriteRenderer spriteRenderer;

    // Variáveis para controlar o Flash sem bugar
    private Color defaultColor;
    private Coroutine currentFlashRoutine;

    void Start()
    {
        currentHealth = maxHealth;
        currentArmor = maxArmor;

        // --- CONFIGURAÇÃO AUTOMÁTICA DE ÁUDIO ---
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

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
            // --- TOCA O SOM DE IMPACTO ---
            if (hitSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(hitSound);
            }

            // Chama o método TriggerFlash em vez de StartCoroutine direto
            Color flashColor = (currentArmor > 0) ? Color.blue : Color.red;
            TriggerFlash(flashColor);
        }
    }

    void Die()
    {
        // --- TOCA O SOM DE MORTE INDEPENDENTE ---
        // Cria um ponto de áudio temporário na coordenada do mapa para o som
        // não ser cortado quando o GameObject deixar de existir.
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        LootBag lootBag = GetComponent<LootBag>();
        if (lootBag != null)
        {
            lootBag.InstantiateLoot(transform.position);
        }

        Debug.Log($"{gameObject.name} morreu.");

        Destroy(gameObject);
    }

    // --- SISTEMA DE FLASH CORRIGIDO ---
    void TriggerFlash(Color targetColor)
    {
        if (spriteRenderer == null) return;

        if (currentFlashRoutine != null)
        {
            StopCoroutine(currentFlashRoutine);
        }

        spriteRenderer.color = defaultColor;
        currentFlashRoutine = StartCoroutine(FlashRoutine(targetColor));
    }

    IEnumerator FlashRoutine(Color color)
    {
        spriteRenderer.color = color;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = defaultColor;
        currentFlashRoutine = null;
    }
}