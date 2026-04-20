using UnityEngine;
using Pathfinding; // Isso é obrigatório para acessar os códigos do pacote!

public class EnemyPathfinder : MonoBehaviour
{
    private AIDestinationSetter destinationSetter;

    void Start()
    {
        // Puxa o componente do A* que você acabou de adicionar no inimigo
        destinationSetter = GetComponent<AIDestinationSetter>();

        // Procura o Player no mapa pela Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            // Diz para o A*: "O seu alvo é este cara aqui, vá pegá-lo!"
            destinationSetter.target = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player não encontrado! Verifique se a Tag está escrita como 'Player'.");
        }
    }
}