using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    [Header("Referências Visuais")]
    // ARRASTE O OBJETO FILHO "VISUALS" PARA AQUI. 
    public Transform visualTransform;
    public Transform weaponHolder;

    [Header("Arremesso")]
    // Nota: A força agora é definida na Arma, mas mantemos aqui caso use para itens genéricos
    public float throwForce = 15f;

    [Header("Movimento")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;

    [Header("Recuo (Knockback)")]
    private Vector2 knockbackVelocity;
    public float knockbackFriction = 10f; // Controla quão rápido o empurrão para
    // ------------------------------------------

    [Header("Idle (Respiração)")]
    public float breathSpeed = 2f;
    public float breathAmount = 0.05f;
    private Vector3 defaultScale;

    [Header("Configurações do Flip & Waddle")]
    public float flipDuration = 0.15f;
    public float squashAmount = 0.1f;
    public float waddleFrequency = 10f;
    public float waddleAmplitude = 5f;

    private bool isFacingRight = true;
    private bool isFlipping = false;

    [Header("Interação")]
    private GameObject currentItem = null;
    private bool isCarrying = false;
    private GameObject interactableItem = null;

    [Header("Referência da Câmera")]
    public CinemachineCamera activeCam;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (visualTransform == null)
            visualTransform = transform;

        defaultScale = visualTransform.localScale;
    }

    void Update()
    {
        // 1. Captura o input de movimento
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Teleporte (Debug)
        if (Input.GetKeyDown(KeyCode.C))
        {
            TeleportPlayer(transform.position + new Vector3(5, 5, 0));
        }

        // 2. Impede movimento diagonal puro (opcional do seu design)
        if (movement.x != 0) movement.y = 0;

        // 3. Lógica de Flip
        if (movement.x < 0 && isFacingRight) Flip();
        else if (movement.x > 0 && !isFacingRight) Flip();

        // 4. Lógica do Waddle
        HandleWaddle();

        // 5. Interação
        if (Input.GetKeyDown(KeyCode.Space) && !isCarrying && interactableItem != null)
        {
            PickUpItem();
        }

        if (isCarrying)
        {
            if (Input.GetKeyDown(KeyCode.G)) DropItem(false); // Drop
            else if (Input.GetMouseButtonDown(1)) DropItem(true); // Throw
        }
    }

    // --- AQUI ESTÁ A MUDANÇA PRINCIPAL (FÍSICA) ---
    void FixedUpdate()
    {
        if (rb != null)
        {
            // 1. Calcula movimento normal (Teclado)
            Vector2 normalMove = movement.normalized * moveSpeed;

            // 2. Soma com o Recuo (Shotgun)
            // Se knockbackVelocity for (0,0), ele anda normal. Se tiver valor, ele soma.
            Vector2 finalVelocity = normalMove + knockbackVelocity;

            // 3. Aplica o movimento combinado
            rb.MovePosition(rb.position + finalVelocity * Time.fixedDeltaTime);

            // 4. Reduz o recuo suavemente (Atrito)
            if (knockbackVelocity.magnitude > 0.1f)
            {
                knockbackVelocity = Vector2.Lerp(knockbackVelocity, Vector2.zero, knockbackFriction * Time.fixedDeltaTime);
            }
            else
            {
                knockbackVelocity = Vector2.zero;
            }
        }
    }

    // --- NOVO MÉTODO: Chamado pelo WeaponController ---
    public void ApplyKnockback(Vector2 direction, float force)
    {
        // Adiciona força instantânea ao vetor de recuo
        knockbackVelocity += direction.normalized * force;
    }

    // --- Lógica do Balanço (Waddle) ---
    void HandleWaddle()
    {
        if (isFlipping) return;

        if (movement.magnitude > 0.1f)
        {
            float angle = Mathf.Sin(Time.time * waddleFrequency) * waddleAmplitude;
            visualTransform.localRotation = Quaternion.Euler(0, 0, angle);

            float currentSignX = Mathf.Sign(visualTransform.localScale.x);
            visualTransform.localScale = new Vector3(Mathf.Abs(defaultScale.x) * currentSignX, defaultScale.y, defaultScale.z);
        }
        else
        {
            visualTransform.localRotation = Quaternion.Lerp(visualTransform.localRotation, Quaternion.identity, Time.deltaTime * 10f);
            float breathFactor = Mathf.Sin(Time.time * breathSpeed) * breathAmount;
            float currentSignX = Mathf.Sign(visualTransform.localScale.x);

            visualTransform.localScale = new Vector3(
                Mathf.Abs(defaultScale.x) * currentSignX,
                defaultScale.y + breathFactor,
                defaultScale.z
            );
        }
    }

    void Flip()
    {
        if (isFlipping) return;
        StartCoroutine(FlipRoutine());
    }

    IEnumerator FlipRoutine()
    {
        isFlipping = true;
        float elapsedTime = 0f;
        Vector3 startScale = visualTransform.localScale;

        float currentXSign = Mathf.Sign(startScale.x);
        Vector3 targetSquashScale = new Vector3(Mathf.Abs(startScale.x) * squashAmount * currentXSign, startScale.y, startScale.z);

        // 1. Squash
        while (elapsedTime < flipDuration / 2)
        {
            visualTransform.localScale = Vector3.Lerp(startScale, targetSquashScale, (elapsedTime / (flipDuration / 2)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 2. Inverter
        isFacingRight = !isFacingRight;
        Vector3 flippedScale = startScale;
        flippedScale.x *= -1;

        // 3. Stretch
        elapsedTime = 0f;
        while (elapsedTime < flipDuration / 2)
        {
            visualTransform.localScale = Vector3.Lerp(targetSquashScale, flippedScale, (elapsedTime / (flipDuration / 2)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        visualTransform.localScale = flippedScale;
        isFlipping = false;
    }

    // --- Interação ---

    private void PickUpItem()
    {
        currentItem = interactableItem;

        currentItem.transform.SetParent(weaponHolder);
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;
        currentItem.transform.localScale = Vector3.one;

        Rigidbody2D itemRb = currentItem.GetComponent<Rigidbody2D>();
        if (itemRb != null) itemRb.simulated = false;

        WeaponAim aimScript = currentItem.GetComponent<WeaponAim>();
        if (aimScript != null) aimScript.enabled = true;

        currentItem.GetComponent<Collider2D>().enabled = false;

        // --- NOVO: Coleta no inventário ---
        WeaponController weapon = currentItem.GetComponent<WeaponController>();
        WeaponInventory inventory = GetComponent<WeaponInventory>();
        if (weapon != null && inventory != null)
        {
            inventory.CollectWeapon(weapon);
        }
        // ----------------------------------

        isCarrying = true;
        interactableItem = null;
    }

    private void DropItem(bool isThrowing)
    {
        if (currentItem == null) return;

        Vector3 playerPos = transform.position;
        WeaponController weapon = currentItem.GetComponent<WeaponController>();
        WeaponInventory inventory = GetComponent<WeaponInventory>();

        if (weapon != null && inventory != null)
        {
            inventory.RemoveWeaponFromInventory(weapon);
        }

        if (weapon != null)
        {
            if (isThrowing)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                Vector2 direction = (mousePos - transform.position).normalized;
                weapon.PerformThrow(direction);
            }
            else
            {
                weapon.PerformDrop(playerPos);
            }
        }
        else
        {
            currentItem.transform.SetParent(null);
        }

        currentItem = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item")) interactableItem = other.gameObject;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == interactableItem) interactableItem = null;
    }

    public void TeleportPlayer(Vector3 newPosition)
    {
        Vector3 positionDelta = newPosition - transform.position;
        transform.position = newPosition;
        if (activeCam != null)
        {
            activeCam.OnTargetObjectWarped(transform, positionDelta);
        }
    }
}