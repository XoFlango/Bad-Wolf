using UnityEngine;

public class MerchantInteractor : MonoBehaviour
{
    [Header("Referências")]
    public DialogueParser dialogueParser; // O script que lê o seu .txt
    public DialogueUI dialogueUI;         // A interface de texto que criamos
    public GameObject promptUI;           // O texto "Pressione E"

    [Header("Configurações")]
    public string nomeDoComerciante = "Comerciante";

    private bool isPlayerNear = false;
    private bool isInConversation = false;

    void Start()
    {
        if (promptUI != null) promptUI.SetActive(false);
    }

    void Update()
    {
        // Se o player está perto, não está conversando e apertou E
        if (isPlayerNear && !isInConversation && Input.GetKeyDown(KeyCode.E))
        {
            StartInteraction();
        }
    }

    void StartInteraction()
    {
        isInConversation = true;
        if (promptUI != null) promptUI.SetActive(false);

        // Inicia o sistema de diálogo com as linhas lidas do .txt
        dialogueUI.IniciarDialogo(dialogueParser.dialogosLidos);

        // O sistema de diálogo deve avisar quando terminar.
        // Por enquanto, usaremos uma Invoke ou checagem simples.
        Debug.Log($"Iniciando conversa com {nomeDoComerciante}");
    }

    // --- LÓGICA DE COMÉRCIO (Pseudo-código) ---
    public void OnDialogueEnded()
    {
        isInConversation = false;
        OpenTradeWindow();
    }

    void OpenTradeWindow()
    {
        Debug.Log("Abrindo Janela de Comércio...");
        /* PSEUDO-CÓDIGO PARA O FUTURO:
           
           1. Parar o movimento do player (playerController.enabled = false)
           2. Ativar o painel de Loja (shopPanel.SetActive(true))
           3. Popular a loja com itens (shopManager.LoadItems(merchantInventory))
           4. Habilitar o cursor do mouse
        */
    }

    // --- DETECÇÃO DE TRIGGER ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (!isInConversation && promptUI != null) promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (promptUI != null) promptUI.SetActive(false);

            // Se o player sair correndo, fecha o diálogo à força
            // dialogueUI.FinalizarDialogo();
            isInConversation = false;
        }
    }
}