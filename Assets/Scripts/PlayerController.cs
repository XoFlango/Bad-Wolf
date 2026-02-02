using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Referências Visuais")]
    // ARRASTE O OBJETO FILHO "VISUALS" PARA AQUI. 
    // Se deixar vazio, ele tenta usar o próprio objeto (mas pode bugar a colisão na parede).
    public Transform visualTransform;
    public Transform weaponHolder;

    [Header("Arremesso")]
    public float throwForce = 15f; // Força do arremesso

    [Header("Movimento")]
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;

    [Header("Idle (Respiração)")]
    public float breathSpeed = 2f; // Velocidade da respiração
    public float breathAmount = 0.05f; // O quanto ele estica (sutil)

    // Variável para lembrar o tamanho original do boneco
    private Vector3 defaultScale;

    [Header("Configurações do Flip & Waddle")]
    public float flipDuration = 0.15f;
    public float squashAmount = 0.1f;
    public float waddleFrequency = 10f; // Velocidade do balanço
    public float waddleAmplitude = 5f;  // Força do balanço (graus)

    private bool isFacingRight = true;
    private bool isFlipping = false;

    [Header("Interação")]
    private GameObject currentItem = null;
    private bool isCarrying = false;
    private GameObject interactableItem = null;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Segurança: Se você esqueceu de definir o visual, usa o transform raiz
        if (visualTransform == null)
            visualTransform = transform;

        defaultScale = visualTransform.localScale;
    }

    void Update()
    {
        // 1. Captura o input de movimento
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // 2. Impede o movimento na diagonal
        if (movement.x != 0)
        {
            movement.y = 0;
        }

        // 3. Lógica de Flip
        if (movement.x < 0 && isFacingRight)
        {
            Flip();
        }
        else if (movement.x > 0 && !isFacingRight)
        {
            Flip();
        }

        // 4. Lógica do Waddle (Balanço) - NOVO!
        HandleWaddle();

        // INTERAÇÃO / SOLTAR / ARREMESSAR

        // Espaço: Pega item (mantive igual)
        if (Input.GetKeyDown(KeyCode.Space) && !isCarrying && interactableItem != null)
        {
            PickUpItem();
        }

        if (isCarrying)
        {
            // Tecla G: Solta o item no chão (Drop Simples)
            if (Input.GetKeyDown(KeyCode.G))
            {
                DropItem(false); // False = Não é arremesso
            }
            // Botão Direito Mouse: Arremessa (Throw)
            else if (Input.GetMouseButtonDown(1))
            {
                DropItem(true); // True = É arremesso
            }
        }
    }

    void FixedUpdate()
    {
        // Aplica o movimento ao Rigidbody
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }

    // --- Lógica do Balanço (Waddle) ---
    void HandleWaddle()
    {
        // Se estiver "Flipping", não mexe na escala nem rotação para não brigar com a corrotina
        if (isFlipping) return;

        // ESTADO 1: ANDANDO (Waddle)
        if (movement.magnitude > 0.1f)
        {
            // Ginga (Rotação)
            float angle = Mathf.Sin(Time.time * waddleFrequency) * waddleAmplitude;
            visualTransform.localRotation = Quaternion.Euler(0, 0, angle);

            // Garante que a escala volte ao normal enquanto anda (sem respiração)
            // Mantendo a direção (Sinal do X) correta
            float currentSignX = Mathf.Sign(visualTransform.localScale.x);
            visualTransform.localScale = new Vector3(Mathf.Abs(defaultScale.x) * currentSignX, defaultScale.y, defaultScale.z);
        }
        // ESTADO 2: PARADO (Breathing)
        else
        {
            // Zera a rotação suavemente
            visualTransform.localRotation = Quaternion.Lerp(visualTransform.localRotation, Quaternion.identity, Time.deltaTime * 10f);

            // Respiração (Escala Y)
            // Cálculo: Seno varia de -1 a 1. Transformamos para algo como 0.95 a 1.05
            float breathFactor = Mathf.Sin(Time.time * breathSpeed) * breathAmount;

            // Mantém o lado que ele está olhando (Sinal do X)
            float currentSignX = Mathf.Sign(visualTransform.localScale.x);

            // Aplica a escala
            visualTransform.localScale = new Vector3(
                Mathf.Abs(defaultScale.x) * currentSignX, // Mantém largura original
                defaultScale.y + breathFactor,            // Respira na altura
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

        // Adaptado para usar visualTransform.localScale ao invés de transform.localScale
        float elapsedTime = 0f;
        Vector3 startScale = visualTransform.localScale;

        // Garante que o Squash mantenha o sinal correto
        float currentXSign = Mathf.Sign(startScale.x);
        Vector3 targetSquashScale = new Vector3(Mathf.Abs(startScale.x) * squashAmount * currentXSign, startScale.y, startScale.z);

        // 1. AMASSAR (Squash)
        while (elapsedTime < flipDuration / 2)
        {
            visualTransform.localScale = Vector3.Lerp(startScale, targetSquashScale, (elapsedTime / (flipDuration / 2)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 2. INVERTER A DIREÇÃO
        isFacingRight = !isFacingRight;
        Vector3 flippedScale = startScale;
        flippedScale.x *= -1;

        // 3. ESTICAR DE VOLTA (Stretch)
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

        // Configuração de Hierarquia
        currentItem.transform.SetParent(weaponHolder);
        currentItem.transform.localPosition = Vector3.zero;
        currentItem.transform.localRotation = Quaternion.identity;
        currentItem.transform.localScale = Vector3.one;

        // LÓGICA DE FÍSICA: Desliga o Rigidbody para a arma não cair da mão
        Rigidbody2D itemRb = currentItem.GetComponent<Rigidbody2D>();
        if (itemRb != null) itemRb.simulated = false; // Desativa a simulação física

        // Ativa a mira
        WeaponAim aimScript = currentItem.GetComponent<WeaponAim>();
        if (aimScript != null) aimScript.enabled = true;

        currentItem.GetComponent<Collider2D>().enabled = false;

        isCarrying = true;
        interactableItem = null;
    }

    private void DropItem(bool isThrowing)
    {
        // 1. Guarda a posição ATUAL do Player (pivô) antes de desconectar
        Vector3 playerPosition = transform.position;

        // 2. Tira do Pai (Desconecta da mão/WeaponHolder)
        currentItem.transform.SetParent(null);

        ThrowableWeapon throwable = currentItem.GetComponent<ThrowableWeapon>();

        if (throwable != null)
        {
            if (isThrowing) // ARREMESSO
            {
                // Calcula direção
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                Vector2 direction = (mousePos - transform.position).normalized;

                // A arma sai da posição atual da mão (WeaponHolder) para parecer natural
                throwable.PrepareThrow(direction, throwForce);
            }
            else // DROP (SOLTAR NO CHÃO)
            {
                // A arma é teleportada para o pé do player (playerPosition)
                throwable.PrepareDrop(playerPosition);
            }
        }

        // Limpeza
        currentItem = null;
        isCarrying = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item")) interactableItem = other.gameObject;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == interactableItem) interactableItem = null;
    }
}