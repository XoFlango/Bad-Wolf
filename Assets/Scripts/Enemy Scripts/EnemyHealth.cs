using UnityEngine;
using System.Collections;

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

    [Header("Áudio do Inimigo")]
    public AudioClip hitSound;
    private AudioSource audioSource;

    [Header("Áudio de morte")]
    public AudioClip deathSound;

    [Header("Efeito de Morte (Cadáver)")] // --- NOVA SEÇÃO ---
    [Tooltip("Arraste o Prefab do cadáver/explosão aqui")]
    public GameObject corpsePrefab;
    [Tooltip("Tempo em segundos que o corpo fica no chão antes de sumir")]
    public float corpseLifetime = 5f;

    [Header("Feedback Visual")]
    public SpriteRenderer spriteRenderer;

    private Color defaultColor;
    private Coroutine currentFlashRoutine;

    void Start()
    {
        currentHealth = maxHealth;
        currentArmor = maxArmor;

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

            damageToHealth = damageLeak;
        }

        currentHealth -= damageToHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (hitSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(hitSound);
            }

            Color flashColor = (currentArmor > 0) ? Color.blue : Color.red;
            TriggerFlash(flashColor);
        }
    }

    void Die()
    {
        if (corpsePrefab != null)
        {
            // 1. Cria o sprite do cadáver na mesma posição e rotação do inimigo
            GameObject corpse = Instantiate(corpsePrefab, transform.position, transform.rotation);

            // --- 2. O SOM DE MORTE NO CADÁVER ---
            if (deathSound != null)
            {
                // Tenta pegar o AudioSource do cadáver. Se ele não tiver, cria um na hora!
                AudioSource corpseAudio = corpse.GetComponent<AudioSource>();
                if (corpseAudio == null)
                {
                    corpseAudio = corpse.AddComponent<AudioSource>();
                }

                // Toca o som a partir do corpo caído
                corpseAudio.PlayOneShot(deathSound);
            }

            // 3. Destrói o cadáver após X segundos
            Destroy(corpse, corpseLifetime);
        }
        else
        {
            Debug.LogWarning("Você esqueceu de colocar o Prefab do Cadáver no Inspector!");
        }

        // Dropa o loot
        LootBag lootBag = GetComponent<LootBag>();
        if (lootBag != null)
        {
            lootBag.InstantiateLoot(transform.position);
        }

        Debug.Log($"{gameObject.name} morreu.");

        // Destrói o inimigo original
        Destroy(gameObject);
    }

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