using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    [Header("Configuração")]
    public WeaponType ammoType; // Escolha no Inspector: AR, Shotgun, etc.
    public int ammoAmount = 30;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Tenta achar o bolso de munição do player
            PlayerAmmoInventory ammoInventory = collision.GetComponent<PlayerAmmoInventory>();

            if (ammoInventory != null)
            {
                ammoInventory.AddAmmo(ammoType, ammoAmount);

                // Som de coleta aqui (Ex: SoundManager.Play("AmmoPickup"))
                Destroy(gameObject);
            }
        }
    }
}