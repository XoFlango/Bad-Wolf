using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour
{
    // --- O PADRÃO SINGLETON ---
    public static GameMenuController instance;

    [Header("Configuração de Cenas")]
    public string gameSceneName = "Lvl1";
    public string mainMenuSceneName = "MainMenu";

    [Header("Painéis (Arraste os objetos da UI aqui)")]
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("Custom Cursor")]
    public Texture2D menuCursorTexture;
    public Vector2 hotspot = Vector2.zero;

    private bool isPaused = false;
    private bool isMainMenu = false;

    void Awake()
    {
        // 1. Verifica se já existe um Gerenciador no jogo
        if (instance == null)
        {
            // Se não existe, este passa a ser o oficial
            instance = this;

            // 2. TORNA ESTE OBJETO IMORTAL
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Se já existe um, destrói a cópia
            Destroy(gameObject);
            return;
        }
    }

    // NOVO: Inscreve o script para ser avisado sempre que uma cena carregar
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // NOVO: Remove a inscrição se o objeto for destruído (evita vazamento de memória)
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // NOVO: Substitui o antigo Start(). Isso roda no Menu e roda novamente no Lvl1, Lvl2, etc.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isMainMenu = (scene.name == mainMenuSceneName);

        // 1. Resetar o estado global de diálogo
        DialogueUI.isDialogueActive = false;

        // 2. Tentar encontrar a UI de Diálogo e desligar o painel
        // (Isso garante que, se o diálogo estava aberto ao trocar de fase, ele feche)
        DialogueUI dUI = FindFirstObjectByType<DialogueUI>();
        if (dUI != null && dUI.dialoguePanel != null)
        {
            dUI.dialoguePanel.SetActive(false);
        }

        // 3. Desligar os painéis de Menu
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;

        if (isMainMenu) SetCursorState(true);
        else SetCursorState(false);
    }

    void Update()
    {
        // 1. Se estiver no Menu Principal, o script não faz mais nada no Update
        if (isMainMenu) return;

        // 2. Se apertar ESC ou P...
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            // 3. CHECAGEM DE DIÁLOGO: Se a UI de conversa estiver aberta, não pausa o jogo!
            if (DialogueUI.isDialogueActive) return;

            // 4. Se não tem diálogo, faz a troca normal de Pause/Play
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // --- FUNÇÕES DE CONTROLE DE FLUXO ---

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // CONGELA O JOGO

        if (pausePanel != null) pausePanel.SetActive(true); // Mostra o menu

        SetCursorState(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // DESCONGELA TUDO

        if (pausePanel != null) pausePanel.SetActive(false); // Esconde o menu

        SetCursorState(false); // ESCONDE O MOUSE PARA VOLTAR A JOGAR
    }

    public void GameOver()
    {
        // Pode ser chamado pelo script de Health do Player
        isPaused = true;
        Time.timeScale = 0f; // Congela para o player ver que morreu

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        SetCursorState(true); // Garante que o cursor apareça para ele clicar em restart
    }

    public void OpenConfigurations()
    {
        Debug.Log("Abrindo configurações... (Futuramente)");
        // Aqui você ativaria um painel de OptionsPanel.SetActive(true)
    }

    public void QuitToMainMenu()
    {
        // Importante: Despausar antes de trocar de cena, senão o menu principal fica travado!
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitDesktop()
    {
        Debug.Log("Saindo para o Desktop...");
        Application.Quit();
    }

    // --- MÉTODO GERENCIADOR DO CURSOR ---
    private void SetCursorState(bool visible)
    {
        if (visible)
        {
            // Mostra o cursor e aplica a textura personalizada
            Cursor.visible = true;
            Cursor.SetCursor(menuCursorTexture, hotspot, CursorMode.ForceSoftware);
        }
        else
        {
            // Esconde o cursor e reseta para o padrão (por segurança)
            Cursor.visible = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}