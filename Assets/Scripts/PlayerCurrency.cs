using UnityEngine;

public class PlayerCurrency : MonoBehaviour
{
    public int totalCoins = 0;

    public void AddCoin(int amount)
    {
        totalCoins += amount;
        Debug.Log($"Plim! Moedas totais: {totalCoins}");

        // Futuramente, chamamos o UIManager aqui para atualizar a tela
    }
}