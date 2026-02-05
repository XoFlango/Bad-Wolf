using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Configuração")]
    public int ammoAmount = 7; // Quantidade de balas que essa caixa dá (1 pente?)
    public string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se foi o Player que tocou
        if (collision.CompareTag(playerTag))
        {
            // Tenta encontrar o script da arma nos filhos do Player (já que a arma está na mão)
            WeaponShooter weapon = collision.GetComponentInChildren<WeaponShooter>();

            if (weapon != null)
            {
                // Adiciona a munição na reserva
                weapon.AddAmmoToReserve(ammoAmount);

                // Efeito sonoro ou visual aqui (SoundManager.Play...)

                // Destrói a caixa
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("O Player pegou a caixa, mas não está segurando nenhuma arma!");
            }
        }
    }
}