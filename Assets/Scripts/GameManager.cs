using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject telaGameOver;
    public PlayerController playerScript;

    // --- Sistema de Pontuação e Dificuldade ---
    public TextMeshProUGUI textoPlacar;
    private float pontuacao;
    public float dificuldade = 1f; 

    // --- Sistema de Moedas ---
    [Header("Sistema de Moedas")]
    public TextMeshProUGUI textoMoedas;
    private int moedasColetadas = 0;

    // --- SISTEMA DE RECORDE (NOVO) ---
    [Header("Leaderboard")]
    public GameObject painelNovoRecorde; // A tela que pede o nome
    public TMP_InputField campoDigitacaoNome; // Onde o jogador digita
    public TextMeshProUGUI textoExibicaoRecorde; // Mostra "Recorde: Nome - Pontos" na tela do jogo

    private bool jogoRodando = true;

    void Start()
    {
        telaGameOver.SetActive(false);
        if (painelNovoRecorde != null) painelNovoRecorde.SetActive(false); // Esconde a tela de recorde
        
        Time.timeScale = 1f; 
        pontuacao = 0;
        moedasColetadas = 0; 
        
        AtualizarTextoDoRecorde(); // Mostra o recorde salvo logo que o jogo começa
    }

    void Update()
    {
        if (jogoRodando)
        {
            pontuacao += playerScript.forwardSpeed * Time.deltaTime;
            
            if (textoPlacar != null)
                textoPlacar.text = Mathf.FloorToInt(pontuacao).ToString();

            dificuldade += 0.02f * Time.deltaTime;
        }
    }

    public void GameOver()
    {
        if (!jogoRodando) return; 

        jogoRodando = false;
        playerScript.forwardSpeed = 0;
        playerScript.enabled = false;

        Animator anim = playerScript.GetComponentInChildren<Animator>();
        if (anim != null) anim.SetTrigger("Die");

        // --- Lógica do Recorde ---
        // Puxa o recorde salvo (se não tiver nada salvo, é 0)
        float recordeAtual = PlayerPrefs.GetFloat("MaiorPontuacao", 0f);

        // Se a pontuação de agora for maior que o recorde antigo...
        if (pontuacao > recordeAtual)
        {
            Invoke("MostrarTelaNovoRecorde", 2f); // Pede o nome!
        }
        else
        {
            Invoke("MostrarTela", 2f); // Game Over normal
        }
    }

    void MostrarTela()
    {
        telaGameOver.SetActive(true);
    }

    void MostrarTelaNovoRecorde()
    {
        painelNovoRecorde.SetActive(true);
    }

    // --- FUNÇÃO PARA O BOTÃO "SALVAR" ---
    public void SalvarRecorde()
    {
        // Pega o que o jogador digitou
        string nome = campoDigitacaoNome.text;
        if (string.IsNullOrEmpty(nome)) nome = "Aluno Anônimo"; // Se não digitar nada

        // Salva as informações no "bloquinho de notas" do Unity
        PlayerPrefs.SetFloat("MaiorPontuacao", pontuacao);
        PlayerPrefs.SetString("NomeRecordista", nome);
        PlayerPrefs.Save();

        // Atualiza a tela, esconde o painel de digitar e mostra o Game Over
        AtualizarTextoDoRecorde();
        painelNovoRecorde.SetActive(false);
        MostrarTela();
    }

    void AtualizarTextoDoRecorde()
    {
        if (textoExibicaoRecorde != null)
        {
            float recorde = PlayerPrefs.GetFloat("MaiorPontuacao", 0f);
            string nome = PlayerPrefs.GetString("NomeRecordista", "Ninguém");
            textoExibicaoRecorde.text = $"Recorde: {nome} - {Mathf.FloorToInt(recorde)}";
        }
    }

    public void ReiniciarJogo()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AdicionarMoeda(int valor)
    {
        moedasColetadas += valor;
        if (textoMoedas != null) textoMoedas.text = "Moedas: " + moedasColetadas.ToString();
    }
}