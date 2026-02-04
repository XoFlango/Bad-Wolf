using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    void Update()
    {
        // Como este script está no caixão, ele só roda quando o player já morreu
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}