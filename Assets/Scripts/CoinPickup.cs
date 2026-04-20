using UnityEngine;
using System.Collections; // Necessário para usar IEnumerator

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class CoinPickup : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Tempo em segundos antes do player conseguir pegar a moeda")]
    public float delayParaColeta = 0.1f;

    private bool podeColetar = false;

    void Start()
    {
        // Assim que a moeda nasce (sai do baú ou do inimigo), inicia o cronômetro
        StartCoroutine(AtivarColetaRoutine());
    }

    IEnumerator AtivarColetaRoutine()
    {
        // Espera o tempo definido (0.1 segundos)
        yield return new WaitForSeconds(delayParaColeta);

        // Libera a moeda para ser pega
        podeColetar = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. CHECAGEM DE SEGURANÇA: Se o tempo não passou, aborta a função aqui mesmo.
        if (!podeColetar) return;

        // 2. Lógica normal de coleta
        if (other.CompareTag("Player"))
        {
            PlayerCurrency wallet = other.GetComponent<PlayerCurrency>();

            if (wallet != null)
            {
                wallet.AddCoin(1);

                // Opcional: Aqui você pode colocar um som de "Plim" ou partícula de brilho antes de destruir

                Destroy(gameObject);
            }
        }
    }
}