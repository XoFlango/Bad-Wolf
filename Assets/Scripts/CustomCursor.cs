using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [Header("Configurações")]
    public SpriteRenderer cursorSprite; // O desenho da mira
    public bool isGameActive = true;   // Se True = Mira do jogo. Se False = Mouse normal (menus).

    private void Start()
    {
        // Se esqueceu de arrastar o sprite, tenta pegar do próprio objeto
        if (cursorSprite == null)
            cursorSprite = GetComponent<SpriteRenderer>();

        // Esconde o mouse do sistema operacional logo no início
        UpdateCursorState();
    }

    private void Update()
    {
        // Se estamos no modo de Jogo, a mira segue o mouse
        if (isGameActive)
        {
            MoveCursor();

            // BÔNUS: Rotação lenta para ficar estiloso
            transform.Rotate(Vector3.forward * 30 * Time.deltaTime);
        }

        // Se você quiser testar a troca de estado, aperte ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isGameActive = !isGameActive;
            UpdateCursorState();
        }
    }

    void MoveCursor()
    {
        // 1. Pega a posição do mouse na tela (Pixels)
        Vector3 mousePos = Input.mousePosition;

        // 2. Converte para o Mundo do Jogo (Unidades Unity)
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        // 3. Importante: Zera o Z para garantir que o sprite seja visível em 2D
        mousePos.z = 0f;

        // 4. Aplica a posição
        transform.position = mousePos;
    }

    // Função pública para você chamar quando abrir/fechar menus no futuro
    public void SetGameActive(bool active)
    {
        isGameActive = active;
        UpdateCursorState();
    }

    private void UpdateCursorState()
    {
        if (isGameActive)
        {
            Cursor.visible = false; // Esconde a seta do Windows
            cursorSprite.enabled = true; // Mostra nossa mira
        }
        else
        {
            Cursor.visible = true; // Mostra a seta do Windows
            cursorSprite.enabled = false; // Esconde nossa mira
        }
    }
}