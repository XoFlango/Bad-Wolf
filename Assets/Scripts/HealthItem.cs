using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class HealthItem : MonoBehaviour
{
    [Header("Configurações de Cura")]
    public int healAmount = 25;

    [Header("Áudio")]
    public AudioClip healSound;


    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Testa se o item sentiu a colisão com qualquer coisa
        Debug.Log($"[ITEM CURA] O item encostou em: {other.gameObject.name}");

        // 2. Testa se o objeto tem a Tag certa
        if (other.CompareTag("Player"))
        {
            Debug.Log("[ITEM CURA] O objeto é o Player! Procurando o script de vida...");

            // MUDANÇA AQUI: Usamos GetComponentInParent por segurança.
            // Se o colisor do seu Player for um objeto "filho" (Hitbox), ele acha o script no "pai".
            Health playerHealth = other.GetComponentInParent<Health>();

            // 3. Testa se achou o script
            if (playerHealth != null)
            {
                Debug.Log("[ITEM CURA] Script 'Health' encontrado! Tentando curar...");

                bool wasHealed = playerHealth.Heal(healAmount);

                // 4. Testa se a vida já não estava cheia
                if (wasHealed)
                {
                    Debug.Log("[ITEM CURA] Player curado com sucesso! Destruindo o item.");

                    if (healSound != null)
                    {
                        AudioSource.PlayClipAtPoint(healSound, transform.position);
                    }
                    Destroy(gameObject);
                }
                else
                {
                    Debug.LogWarning("[ITEM CURA] O item não foi destruído porque a vida do Player já está 100% cheia!");
                }
            }
            else
            {
                Debug.LogError("[ITEM CURA] O objeto tem a tag 'Player', mas o script 'Health' não foi encontrado nele!");
            }
        }
    }
}