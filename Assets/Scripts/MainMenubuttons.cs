using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    public void BotaoPlay()
    {
        // Avisa a instância global imortal para iniciar o jogo
        if (GameMenuController.instance != null)
        {
            GameMenuController.instance.PlayGame();
        }
    }

    public void BotaoSair()
    {
        if (GameMenuController.instance != null)
        {
            GameMenuController.instance.QuitDesktop();
        }
    }
}