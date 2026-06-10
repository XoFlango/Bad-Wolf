using UnityEngine;
using Unity.Cinemachine;

public enum WeaponType { Pistol = 0, AssaultRifle = 1, Shotgun = 2 }

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class WeaponController : MonoBehaviour
{
    [Header("Interface")]
    public Sprite iconUI;

    [Header("Configuração Geral")]
    public WeaponType weaponType;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Combate")]
    public float fireRate = 0.5f;
    public int damage = 10;

    [Header("Áudio")] // --- NOVO CABEÇALHO PARA O SOM ---
    public AudioClip shootSound;

    [Header("Munição")]
    public int maxAmmo = 30;
    public int currentAmmo;
    private PlayerAmmoInventory playerAmmoInv;

    [Header("Shotgun")]
    public int pellets = 3;
    public float spreadAngle = 15f;
    public float kickbackForce = 5f;

    [Header("Arremesso (Throw)")]
    public float throwForce = 15f;
    public float throwSpin = 720f;
    public int throwDamage = 5;

    // Estados Internos
    private float nextFireTime = 0f;
    private bool isEquipped = false;
    private Rigidbody2D rb;
    private Collider2D col;
    private CinemachineImpulseSource impulseSource;
    private AudioSource audioSource; // --- NOVO COMPONENTE DE ÁUDIO ---

    private bool IsAutomatic => weaponType == WeaponType.AssaultRifle;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        currentAmmo = maxAmmo;

        // --- CONFIGURAÇÃO DO ÁUDIO ---
        // Pega o AudioSource se já existir, ou cria um automaticamente na arma
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false; // Garante que a arma não atire sozinha ao nascer
    }

    void Update()
    {
        isEquipped = transform.parent != null;
        if (!isEquipped) return;

        HandleShooting();
        HandleReload();
    }

    // --- TIRO ---
    void HandleShooting()
    {
        bool triggerPulled = IsAutomatic ? Input.GetButton("Fire1") : Input.GetButtonDown("Fire1");

        if (triggerPulled && Time.time >= nextFireTime)
        {
            if (currentAmmo > 0)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
            else
            {
                Debug.Log("Clic! Sem munição. R para recarregar.");
            }
        }
    }

    void Shoot()
    {
        currentAmmo--;

        int reserve = (playerAmmoInv != null) ? playerAmmoInv.GetAmmoCount(weaponType) : 0;
        Debug.Log($"[ARMA] {weaponType} | Pente: {currentAmmo}/{maxAmmo} | Bolso: {reserve}");

        // --- TOCA O SOM DO TIRO AQUI ---
        if (shootSound != null && audioSource != null)
        {
            // O PlayOneShot permite que os sons se sobreponham, ideal para armas automáticas ou shotgun!
            audioSource.PlayOneShot(shootSound);
        }

        switch (weaponType)
        {
            case WeaponType.Pistol:
            case WeaponType.AssaultRifle:
                SpawnProjectile(Quaternion.identity);
                break;
            case WeaponType.Shotgun:
                FireShotgun();
                break;
        }
    }

    void SpawnProjectile(Quaternion rotationOffset)
    {
        if (projectilePrefab && firePoint)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation * rotationOffset);
        }
    }

    void FireShotgun()
    {
        for (int i = 0; i < pellets; i++)
        {
            float angleStep = spreadAngle / (pellets - 1);
            float currentAngle = -spreadAngle / 2 + (angleStep * i);
            SpawnProjectile(Quaternion.Euler(0, 0, currentAngle));
        }
        if (impulseSource) impulseSource.GenerateImpulse();

        if (transform.parent && transform.parent.TryGetComponent(out Rigidbody2D pRb))
        {
            pRb.AddForce(-firePoint.right * kickbackForce, ForceMode2D.Impulse);
        }

        // --- RECUO ---
        if (transform.parent != null)
        {
            PlayerController player = transform.parent.GetComponent<PlayerController>();

            if (player != null)
            {
                Vector2 recoilDir = -firePoint.right;
                player.ApplyKnockback(recoilDir, kickbackForce);
            }
        }
    }

    // --- RECARGA ---
    void HandleReload()
    {
        if (Input.GetKeyDown(KeyCode.R)) Reload();
    }

    void Reload()
    {
        if (transform.parent == null) return;
        if (playerAmmoInv == null) playerAmmoInv = transform.parent.GetComponent<PlayerAmmoInventory>();
        if (playerAmmoInv == null || currentAmmo == maxAmmo) return;

        int needed = maxAmmo - currentAmmo;
        int received = playerAmmoInv.TakeAmmo(weaponType, needed);

        if (received > 0) currentAmmo += received;
    }

    // --- AÇÕES PÚBLICAS (CHAMADAS PELO PLAYER) ---
    public void PerformThrow(Vector2 direction)
    {
        Disconnect();
        rb.linearVelocity = direction * throwForce;
        rb.angularVelocity = -throwSpin;
    }

    public void PerformDrop(Vector3 dropPosition)
    {
        Disconnect();
        transform.position = dropPosition;
        rb.linearVelocity = new Vector2(Random.Range(-0.5f, 0.5f), 0);
    }

    void Disconnect()
    {
        transform.parent = null;
        isEquipped = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        col.isTrigger = false;
    }

    // --- COLISÃO / COLETA ---
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb.linearVelocity.magnitude > 2f && collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.TryGetComponent(out Health enemy))
            {
                enemy.TakeDamage(throwDamage);
            }
        }
        rb.linearVelocity *= 0.3f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isEquipped && other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out WeaponInventory inventory))
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0;
                col.isTrigger = true;

                inventory.CollectWeapon(this);
                isEquipped = true;

                playerAmmoInv = other.GetComponent<PlayerAmmoInventory>();
            }
        }
    }
}