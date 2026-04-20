using UnityEngine;
using System.Collections.Generic;

public class LootBag : MonoBehaviour
{
    // Classe interna para definir cada item na lista do Inspector
    [System.Serializable]
    public class LootItem
    {
        public string nome; // Só pra você se organizar no Inspector
        public GameObject itemPrefab;
        [Range(0, 100)] public float dropChance; // Chance em %
    }

    [Header("Tabela de Loot")]
    public List<LootItem> lootList = new List<LootItem>();

    // Função chamada pelo script de Vida quando o dono morre
    public void InstantiateLoot(Vector3 spawnPosition)
    {
        // 1. Roda o dado de 0 a 100
        float randomNumber = Random.Range(0f, 100f);

        float currentProbabilitySum = 0f;

        foreach (LootItem item in lootList)
        {
            // Soma a chance do item atual ao total verificado
            // Ex: Item A (30%) -> Verifica se o dado é <= 30
            // Ex: Item B (10%) -> Verifica se o dado é <= 40 (30+10)
            if (randomNumber <= currentProbabilitySum + item.dropChance)
            {
                if (item.itemPrefab != null)
                {
                    Instantiate(item.itemPrefab, spawnPosition, Quaternion.identity);
                    Debug.Log($"Sorte! Dropou: {item.nome}");
                }
                return; // Já dropou, encerra a função (para dropar apenas 1 item)
            }

            // Se não caiu nesse item, soma a chance dele e passa para o próximo
            currentProbabilitySum += item.dropChance;
        }

        // Se o loop terminar e nada foi escolhido, significa que caiu na chance de "Nada"
        Debug.Log("Azar! O inimigo não dropou nada.");
    }
}