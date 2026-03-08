using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour
{
    [Header("Configuração de Cenas")]
    public string gameSceneName = "Lvl1";
    public string mainMenuSceneName = "MainMenu";

    [Header("Painéis (Arraste os objetos da UI aqui)")]
    public GameObject pausePanel;    // O painel que tem o fundo preto e botões de pause
    public GameObject gameOverPanel; // O painel de "Você Morreu" (se já tiver)

    [Header("Custom Cursor")]
    public Texture2D menuCursorTexture; // Arraste seu sprite aqui
    public Vector2 hotspot = Vector2.zero; // Onde é a "ponta" do clique (0,0 = canto superior esquerdo)

    // Estado interno
    private bool isPaused = false;
    private bool isMainMenu = false;

    void Start()
    {
        // Verifica se estamos na cena do Menu Principal
        // (Evita que o ESC funcione no menu principal)
        string currentScene = SceneManager.GetActiveScene().name;
        isMainMenu = (currentScene == mainMenuSceneName);

        // Garante que os painéis comecem escondidos no início da fase
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // Garante que o jogo comece rodando (caso tenha saído pausado antes)
        Time.timeScale = 1f;

        // --- LÓGICA INICIAL DO CURSOR ---
        if (isMainMenu || isPaused)
        {
            // Se for menu principal, MOSTRA o cursor personalizado
            SetCursorState(true);
        }
        else
        {
            // Se for fase do jogo, ESCONDE o cursor (pois você usa a mira da arma)
            SetCursorState(false);
        }
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
            // CursorMode.ForceSoftware garante que a imagem apareça mesmo em builds
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