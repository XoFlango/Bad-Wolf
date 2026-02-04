using UnityEngine;
// Não precisa mais de SceneManagement aqui, pois o Caixão cuida disso

public class Health : MonoBehaviour
{
    [Header("Status")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Ao Morrer")]
    public GameObject coffinPrefab; // Arraste o PREFAB do caixão aqui

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        // Tecla de suicídio para teste
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(9999);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Trocando Player pelo Caixão...");

        // 1. Cria o caixão na mesma posição e rotação do player
        if (coffinPrefab != null)
        {
            Instantiate(coffinPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("ERRO: Você esqueceu de colocar o Prefab do Caixão no script Health!");
        }

        // 2. Destrói o Player (Adeus scripts, adeus bugs, adeus animator)
        Destroy(gameObject);
    }
}