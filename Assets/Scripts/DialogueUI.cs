using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueUI : MonoBehaviour
{
    // VARIÁVEL GLOBAL: Qualquer script do jogo pode ler isso sem precisar de referência!
    public static bool isDialogueActive = false;

    [Header("Referências UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;

    [Header("Configurações")]
    public float typingSpeed = 0.05f;

    private List<DialogueParser.LinhaDeDialogo> linhasAtuais;
    private int indiceAtual = 0;
    private bool estaDigitando = false;
    private Coroutine corrotinaDigitacao;

    public static DialogueUI instance;

    void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }
    }
    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    void Update()
    {
        // Se o diálogo não estiver rolando, o script ignora as teclas abaixo
        if (!isDialogueActive) return;

        // AVANÇAR: Tecla E ou Espaço
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
        {
            AoClicarParaProsseguir();
        }

        // CANCELAR: Tecla ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelarDialogo();
        }
    }

    public void IniciarDialogo(List<DialogueParser.LinhaDeDialogo> novasLinhas)
    {
        isDialogueActive = true; // Avisa o jogo inteiro que o diálogo começou
        linhasAtuais = novasLinhas;
        indiceAtual = 0;
        dialoguePanel.SetActive(true);
        ExibirProximaLinha();
    }

    public void AoClicarParaProsseguir()
    {
        if (estaDigitando)
        {
            PularDigitacao();
        }
        else
        {
            indiceAtual++;
            ExibirProximaLinha();
        }
    }

    void ExibirProximaLinha()
    {
        if (indiceAtual < linhasAtuais.Count)
        {
            if (corrotinaDigitacao != null) StopCoroutine(corrotinaDigitacao);
            nameText.text = linhasAtuais[indiceAtual].nomePersonagem;
            corrotinaDigitacao = StartCoroutine(DigitarTexto(linhasAtuais[indiceAtual].textoDialogo));
        }
        else
        {
            FinalizarDialogo();
        }
    }

    IEnumerator DigitarTexto(string texto)
    {
        estaDigitando = true;
        dialogueText.text = "";

        foreach (char letra in texto.ToCharArray())
        {
            dialogueText.text += letra;
            yield return new WaitForSeconds(typingSpeed);
        }
        estaDigitando = false;
        ///buacar
    }

    void PularDigitacao()
    {
        StopCoroutine(corrotinaDigitacao);
        dialogueText.text = linhasAtuais[indiceAtual].textoDialogo;
        estaDigitando = false;
    }

    // Função nova para o botão ESC
    void CancelarDialogo()
    {
        if (corrotinaDigitacao != null) StopCoroutine(corrotinaDigitacao);
        FinalizarDialogo();
    }

    public void FinalizarDialogo()
    {
        isDialogueActive = false; // Libera o jogo!
        dialoguePanel.SetActive(false);
        Debug.Log("Fim da conversa.");
    }
}