using UnityEngine;

public static class DamageCalculator
{
    // Configurações de Probabilidade (0.0 a 1.0)
    private const float CRIT_CHANCE = 0.10f; // 10% de chance de Crítico
    private const float VARIANCE_CHANCE = 0.30f; // 30% de chance de Dano Variável

    public static int CalculateFinalDamage(int baseDamage)
    {
        float rng = Random.value; // Gera um número entre 0.0 e 1.0

        // 1. CHECAGEM DE CRÍTICO (Prioridade máxima)
        if (rng <= CRIT_CHANCE)
        {
            Debug.Log($"<color=red>CRÍTICO! (2x)</color>");
            return baseDamage * 2;
        }

        // 2. CHECAGEM DE DANO VARIÁVEL (Se não foi crítico, tenta variar)
        // Ajustamos o range para considerar os 10% que já passaram
        else if (rng <= (CRIT_CHANCE + VARIANCE_CHANCE))
        {
            float multiplier = Random.Range(1.1f, 1.5f);
            int finalDamage = Mathf.RoundToInt(baseDamage * multiplier);
            Debug.Log($"<color=yellow>Dano Aumentado! ({multiplier:F1}x)</color>");
            return finalDamage;
        }

        // 3. DANO BASE (Normal)
        else
        {
            // Debug.Log("Dano Normal (1x)");
            return baseDamage;
        }
    }
}