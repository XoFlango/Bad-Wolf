using UnityEngine;

public class WeaponShooter : MonoBehaviour
{
    [Header("Configurações de Tiro")]
    public GameObject projectilePrefab; // Arraste o prefab da bala aqui
    public Transform firePoint;         // O ponto vazio na ponta da arma (Muzzle)
    public float fireRate = 0.5f;       // Tempo entre tiros

    private float nextFireTime = 0f;
    private ThrowableWeapon throwable;  // Para checar se a arma não está voando

    void Start()
    {
        throwable = GetComponent<ThrowableWeapon>();
    }

    void Update()
    {
        // Se a arma estiver voando (arremessada), não pode atirar
        if (throwable != null && throwable.isFlying) return;

        // Input de Tiro (Botão Esquerdo)
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (firePoint == null || projectilePrefab == null)
        {
            Debug.LogWarning("Faltam referências no WeaponShooter!");
            return;
        }

        // Instancia a bala na posição e ROTAÇÃO da arma/firePoint
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }
}