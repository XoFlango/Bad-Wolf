using UnityEngine;
using System.Collections;
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

    [Header("Precisão (Spread)")]
    [Tooltip("Graus de variação do tiro. 0 = laser perfeito. 5 = espalhamento leve.")]
    public float bulletSpread = 3f;

    [Header("Áudio")]
    public AudioClip shootSound;
    public AudioClip reloadSound;

    [Header("Munição e Recarga")]
    public int maxAmmo = 30;
    public int currentAmmo;
    public float reloadDuration = 2.0f;

    private bool isReloading = false;
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

    private bool IsAutomatic => weaponType == WeaponType.AssaultRifle;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        currentAmmo = maxAmmo;

        // Removemos a criação do AudioSource na arma! Agora a responsabilidade é do Player.
    }

    void Update()
    {
        isEquipped = transform.parent != null;
        if (!isEquipped) return;

        HandleShooting();
        HandleReload();
    }

    // --- NOVA FUNÇÃO: Busca o Áudio do Player ---
    private AudioSource GetPlayerAudio()
    {
        if (transform.parent == null) return null;

        // Procura o AudioSource no Player
        AudioSource playerAudio = transform.parent.GetComponent<AudioSource>();

        // Se o Player não tiver um, cria um silenciosamente
        if (playerAudio == null)
        {
            playerAudio = transform.parent.gameObject.AddComponent<AudioSource>();
            playerAudio.playOnAwake = false;
        }

        return playerAudio;
    }

    void HandleShooting()
    {
        if (isReloading) return;

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

        // --- MUDANÇA: Toca o som de tiro USANDO O PLAYER ---
        if (shootSound != null)
        {
            AudioSource pAudio = GetPlayerAudio();
            if (pAudio != null) pAudio.PlayOneShot(shootSound);
        }

        switch (weaponType)
        {
            case WeaponType.Pistol:
            case WeaponType.AssaultRifle:
                float randomSpread = Random.Range(-bulletSpread, bulletSpread);
                SpawnProjectile(Quaternion.Euler(0, 0, randomSpread));
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

    void HandleReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    IEnumerator ReloadRoutine()
    {
        if (transform.parent == null) yield break;
        if (playerAmmoInv == null) playerAmmoInv = transform.parent.GetComponent<PlayerAmmoInventory>();

        if (playerAmmoInv == null || currentAmmo == maxAmmo) yield break;

        int needed = maxAmmo - currentAmmo;
        if (playerAmmoInv.GetAmmoCount(weaponType) <= 0)
        {
            Debug.Log("Sem munição reserva no inventário!");
            yield break;
        }

        isReloading = true;

        // --- MUDANÇA: Toca o som de recarga USANDO O PLAYER ---
        if (reloadSound != null)
        {
            AudioSource pAudio = GetPlayerAudio();
            if (pAudio != null) pAudio.PlayOneShot(reloadSound);
        }

        yield return new WaitForSeconds(reloadDuration);

        int received = playerAmmoInv.TakeAmmo(weaponType, needed);
        if (received > 0) currentAmmo += received;

        isReloading = false;
    }

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
        StopAllCoroutines();
        isReloading = false;

        transform.parent = null;
        isEquipped = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0;
        col.isTrigger = false;
    }

    void OnDisable()
    {
        isReloading = false;
    }

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