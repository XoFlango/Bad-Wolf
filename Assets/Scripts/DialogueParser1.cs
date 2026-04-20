using UnityEngine;
using System.Collections.Generic;

public class DialogueParser : MonoBehaviour
{
    // Criamos uma estrutura para guardar o nome e o texto separadinhos
    [System.Serializable]
    public struct LinhaDeDialogo
    {
        public string nomePersonagem;
        public string textoDialogo;
    }

    [Header("Arquivo .txt")]
    // TextAsset é como a Unity chama arquivos .txt importados
    public TextAsset arquivoDeTexto;

    [Header("Resultado (Visível no Inspector)")]
    public List<LinhaDeDialogo> dialogosLidos = new List<LinhaDeDialogo>();

    void Awake()
    {
        CarregarDialogos();
    }

    void CarregarDialogos()
    {
        if (arquivoDeTexto == null)
        {
            Debug.LogWarning("Nenhum arquivo .txt foi colocado no Inspector!");
            return;
        }

        // 1. Divide o texto inteiro em várias linhas isoladas
        string[] linhas = arquivoDeTexto.text.Split('\n');

        foreach (string linhaAtual in linhas)
        {
            // Ignora linhas em branco para não dar erro
            if (string.IsNullOrWhiteSpace(linhaAtual)) continue;

            // 2. O PULO DO GATO: Corta a linha usando o ':'
            string[] partes = linhaAtual.Split(':');

            // Se a linha tem pelo menos duas partes (Nome e Texto)
            if (partes.Length >= 2)
            {
                LinhaDeDialogo novaLinha = new LinhaDeDialogo();

                // O comando .Trim() limpa espaços vazios nas bordas (" Player " vira "Player")
                novaLinha.nomePersonagem = partes[0].Trim();
                novaLinha.textoDialogo = partes[1].Trim();

                // Adiciona na nossa lista pronta para ser usada pela UI
                dialogosLidos.Add(novaLinha);
            }
            else
            {
                Debug.LogWarning($"A linha não tem o separador ':'. Texto ignorado: {linhaAtual}");
            }
        }

        Debug.Log($"Leitura concluída! Carreguei {dialogosLidos.Count} linhas de diálogo.");
    }
}