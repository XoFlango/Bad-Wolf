using UnityEngine;
using System.Collections.Generic; // Necessário para usar a 'List'

// --- NOVA CLASSE DE LOOT ---
// [System.Serializable] faz essa classe aparecer bonitinha no Inspector da Unity
[System.Serializable]
public class LootItem
{
    public string itemName = "Novo Item"; // Só para ajudar na organização visual no Inspector
    public GameObject itemPrefab;

    [Tooltip("Chance de 0 a 100 de este item dropar")]
    [Range(0f, 100f)]
    public float dropChance = 50f;

    public int minAmount = 1;
    public int maxAmount = 1;
}

public class ChestController : MonoBehaviour
{
    [Header("Visuais do Baú")]
    public Sprite closedSprite;
    public Sprite openSprite;
    private SpriteRenderer sr;

    [Header("Áudio")]
    [Tooltip("Som tocado quando o baú é aberto")]
    public AudioClip openSound;
    private AudioSource audioSource;

    [Header("Configurações de Loot")]
    [Tooltip("Adicione aqui todos os itens que este baú pode cuspir!")]
    public List<LootItem> possibleLoot; // Substitui o antigo 'coinPrefab'

    public GameObject promptUI;

    private bool isPlayerNear = false;
    private bool isOpen = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = closedSprite;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;

        if (promptUI != null) promptUI.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNear && !isOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        isOpen = true;
        sr.sprite = openSprite; // Troca a imagem

        if (promptUI != null) promptUI.SetActive(false); // Esconde o aviso

        // Toca o som
        if (openSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(openSound);
        }

        // Passa por cada item da lista
        foreach (LootItem loot in possibleLoot)
        {
            if (loot.itemPrefab == null) continue;

            float roll = Random.Range(0f, 100f);

            if (roll <= loot.dropChance)
            {
                int amountToDrop = Random.Range(loot.minAmount, loot.maxAmount + 1);

                // --- O SEU DEBUG LOG AQUI ---
                Debug.Log($"[BAÚ] Dropou: {amountToDrop}x {loot.itemName}");

                for (int i = 0; i < amountToDrop; i++)
                {
                    SpawnJumpingItem(loot.itemPrefab);
                }
            }
        }
    }

    // --- FUNÇÃO DE SPAWN ATUALIZADA ---
    // Agora pede um 'prefab' como argumento, para servir pra qualquer coisa
    void SpawnJumpingItem(GameObject prefab)
    {
        GameObject item = Instantiate(prefab, transform.position, Quaternion.identity);

        Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            float scatterForce = Random.Range(4f, 8f);

            rb.AddForce(randomDirection * scatterForce, ForceMode2D.Impulse);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            isPlayerNear = true;
            if (promptUI != null) promptUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (promptUI != null) promptUI.SetActive(false);
        }
    }
}