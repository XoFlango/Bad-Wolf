using UnityEngine;
using UnityEngine.UI; // Necessário para mexer com Image

public class HealthBar : MonoBehaviour
{
    [Header("Referências")]
    public Image healthBarFill; // Arraste a imagem VERDE aqui

    [Header("Cores")]
    public Color healthyColor = Color.green;  // Acima de 60%
    public Color warningColor = Color.yellow; // Entre 25% e 60%
    public Color criticalColor = Color.red;   // Abaixo de 25%

    public void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        // 1. Calcula a porcentagem (0 a 1)
        // O (float) é obrigatório, senão a divisão de inteiros retorna 0
        float percentage = (float)currentHealth / maxHealth;

        // 2. Atualiza o tamanho da barra (Fill Amount)
        healthBarFill.fillAmount = percentage;

        // 3. Lógica das Cores
        if (percentage > 0.6f) // Acima de 60%
        {
            healthBarFill.color = healthyColor;
        }
        else if (percentage > 0.25f) // Entre 25% e 60%
        {
            healthBarFill.color = warningColor;
        }
        else // Abaixo de 25%
        {
            healthBarFill.color = criticalColor;
        }
    }
}