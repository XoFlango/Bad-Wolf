using UnityEngine;

public class ChestController : MonoBehaviour
{
    [Header("Visuais do Baú")]
    public Sprite closedSprite;
    public Sprite openSprite;
    private SpriteRenderer sr;

    [Header("Configurações de Loot")]
    public GameObject coinPrefab;
    // O aviso "Pressione E para abrir" (pode ser o mesmo texto que usamos no Boss)
    public GameObject promptUI;

    private bool isPlayerNear = false;
    private bool isOpen = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = closedSprite; // Garante que começa fechado

        if (promptUI != null) promptUI.SetActive(false);
    }

    void Update()
    {
        // Se o player está perto, o baú tá fechado, e apertou E
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

        // Sorteia de 1 a 3 moedas
        int coinAmount = Random.Range(1, 4);

        for (int i = 0; i < coinAmount; i++)
        {
            SpawnJumpingCoin();
        }
    }

    void SpawnJumpingCoin()
    {
        GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);

        Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 1. Pega uma direção aleatória em 360 graus (um círculo ao redor do baú)
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            // 2. Define a força da explosão
            float scatterForce = Random.Range(4f, 8f);

            // 3. Aplica o empurrão
            rb.AddForce(randomDirection * scatterForce, ForceMode2D.Impulse);
        }
    }

    // --- DETECÇÃO DO PLAYER ---
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