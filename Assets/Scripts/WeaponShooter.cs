using UnityEngine;

public class WeaponShooter : MonoBehaviour
{
    [Header("Configurações de Tiro")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;

    [Header("Munição")]
    public int maxMagSize = 7;      // Capacidade da pistola (7)
    public int currentAmmo;         // Balas atuais na arma
    public int reserveAmmo = 0;     // Balas no bolso (total acumulado)

    private float nextFireTime = 0f;
  //  private ThrowableWeapon throwable;

    // Propriedade para checar se está na mão do player
    private bool IsEquipped => transform.parent != null;

    void Start()
    {
     //   throwable = GetComponent<ThrowableWeapon>();

        // A arma começa carregada (Regra que você pediu)
        currentAmmo = maxMagSize;

        // Opcional: Começar com 0 de reserva ou algum valor
        reserveAmmo = 0;
    }

    void Update()
    {
        if (!IsEquipped) return;
        //if (throwable != null && throwable.isFlying) return;

        // Recarregar (Tecla R)
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        // Atirar
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            if (currentAmmo > 0)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
            else
            {
                Debug.Log("Clic! Sem munição. Aperte R.");
                // Aqui entraria um som de "clique" seco
            }
        }
    }

    void Shoot()
    {
        if (firePoint == null || projectilePrefab == null) return;

        currentAmmo--; // Gasta uma bala
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Debug para você acompanhar
        Debug.Log($"Pew! Munição: {currentAmmo}/{maxMagSize} | Reserva: {reserveAmmo}");
    }

    // Método chamado pelo script da Caixa de Munição
    public void AddAmmoToReserve(int amount)
    {
        reserveAmmo += amount;
        Debug.Log($"Pegou munição! Total na reserva: {reserveAmmo}");

        // Opcional: Se a arma estiver vazia, recarrega automaticamente?
        // if (currentAmmo == 0) Reload();
    }

    void Reload()
    {
        // Se já está cheia ou não tem reserva, não faz nada
        if (currentAmmo == maxMagSize || reserveAmmo <= 0) return;

        // Calcula quantas balas faltam para encher o pente
        int bulletsNeeded = maxMagSize - currentAmmo;

        // Verifica se tem balas suficientes na reserva
        if (reserveAmmo >= bulletsNeeded)
        {
            currentAmmo += bulletsNeeded;
            reserveAmmo -= bulletsNeeded;
        }
        else
        {
            // Se tiver menos na reserva do que o necessário, pega tudo que tem
            currentAmmo += reserveAmmo;
            reserveAmmo = 0;
        }

        Debug.Log("Recarregando... Tchack-Tchack!");
    }
}