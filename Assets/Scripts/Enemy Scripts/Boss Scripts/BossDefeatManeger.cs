using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para trocar de cena

public class BossDefeatManager : MonoBehaviour
{
    [Header("Observador")]
    [Tooltip("Arraste o objeto do Boss da sua cena para cá")]
    public GameObject bossObj;

    [Header("Interface (UI)")]
    [Tooltip("Arraste o Texto (Canvas) que diz 'Pressione E para prosseguir'")]
    public GameObject promptUI;

    [Header("Transição de Fase")]
    [Tooltip("O nome exato da próxima cena (ex: Level_02)")]
    public string nextSceneName = "Lvl2";

    // Variável interna para saber se já ganhamos
    private bool isBossDead = false;

    void Start()
    {
        // Garante que o texto comece invisível
        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }
    }

    void Update()
    {
        // 1. CHECAGEM DE MORTE
        // Na Unity, quando um objeto é destruído (Destroy), ele vira 'null'
        if (!isBossDead && bossObj == null)
        {
            isBossDead = true;
            Debug.Log("O Boss foi derrotado! Aguardando input do jogador...");

            // Liga o texto na tela
            if (promptUI != null) promptUI.SetActive(true);
        }

        // 2. TRANSIÇÃO DE FASE
        // Se o Boss estiver morto e o jogador apertar a tecla "E" (ou "Enter")
        if (isBossDead && Input.GetKeyDown(KeyCode.E))
        {
            GoToNextLevel();
        }
    }

    void GoToNextLevel()
    {
        // Carrega a próxima cena
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Nome da próxima cena não foi configurado!");
        }
    }
}