using System.Collections;
using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI do Jogo")]
    public GameObject telaGameOver;
    public TextMeshProUGUI textoPlacar;
    public TextMeshProUGUI textoMoedas;
    public TextMeshProUGUI textoCountdown;

    [Header("Leaderboard")]
    public GameObject painelNovoRecorde; 
    public TMP_InputField campoDigitacaoNome; 
    public TextMeshProUGUI textoExibicaoRecorde; 

    [Header("Configurações")]
    public PlayerController playerScript;
    public bool jogoRodando = false;
    public float dificuldade = 1f; 

    [Header("Ajuste de Dificuldade")]
    public float velocidadeMaximaDaEsteira = 35f; // Limite para o jogo não ficar impossível
    public float aceleracaoPorSegundo = 0.2f; // O quanto a esteira acelera a cada segundo

    private float pontuacao;
    private int moedasColetadas = 0;
    private bool jogoIniciou = false;

    void Awake() 
    { 
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        telaGameOver.SetActive(false);
        if (painelNovoRecorde != null) painelNovoRecorde.SetActive(false);
        
        Time.timeScale = 1f;
        dificuldade = 1f; 
        pontuacao = 0;
        moedasColetadas = 0;
        
        AtualizarTextoDoRecorde();

        if (textoCountdown != null)
        {
            textoCountdown.gameObject.SetActive(true);
            textoCountdown.text = "PRESSIONE QUALQUER TECLA";
        }
    }

    void Update()
    {
        // 1. Espera o jogador apertar um botão para iniciar a contagem
        if (!jogoIniciou && Input.anyKeyDown)
        {
            jogoIniciou = true;
            StartCoroutine(ContagemRegressiva());
        }

        // 2. O jogo rolando normalmente
        if (jogoRodando)
        {
            // Soma a pontuação baseada na velocidade atual
            pontuacao += playerScript.forwardSpeed * Time.deltaTime;
            if (textoPlacar != null) textoPlacar.text = Mathf.FloorToInt(pontuacao).ToString();

            // Cresce o multiplicador de dificuldade interno
            dificuldade += 0.02f * Time.deltaTime;

            // ACELERANDO A ESTEIRA DO JOGO AOS POUCOS
            if (playerScript != null && playerScript.forwardSpeed < velocidadeMaximaDaEsteira)
            {
                playerScript.forwardSpeed += aceleracaoPorSegundo * Time.deltaTime;
            }
        }
    }

    private IEnumerator ContagemRegressiva()
    {
        int tempo = 3;
        while (tempo > 0)
        {
            if (textoCountdown != null) textoCountdown.text = tempo.ToString();
            yield return new WaitForSeconds(1f);
            tempo--;
        }
        if (textoCountdown != null) textoCountdown.text = "VAI!";
        yield return new WaitForSeconds(0.5f);
        if (textoCountdown != null) textoCountdown.gameObject.SetActive(false);
        
        jogoRodando = true;
    }

    public void GameOver()
    {
        if (!jogoRodando) return;
        
        jogoRodando = false;
        Time.timeScale = 0f; 

        float recordeAtual = PlayerPrefs.GetFloat("MaiorPontuacao", 0f);
        if (pontuacao > recordeAtual) painelNovoRecorde.SetActive(true);
        else telaGameOver.SetActive(true);
    }

    public void SalvarRecorde()
    {
        string nome = campoDigitacaoNome.text;
        if (string.IsNullOrEmpty(nome)) nome = "Aluno";
        
        PlayerPrefs.SetFloat("MaiorPontuacao", pontuacao);
        PlayerPrefs.SetString("NomeRecordista", nome);
        PlayerPrefs.Save();
        
        painelNovoRecorde.SetActive(false);
        telaGameOver.SetActive(true);
    }

    void Ny() {} // Evita qualquer conflito

    void Ut() {} // Evita qualquer conflito

    void AtualizarTextoDoRecorde()
    {
        if (textoExibicaoRecorde != null)
        {
            float recorde = PlayerPrefs.GetFloat("MaiorPontuacao", 0f);
            string nome = PlayerPrefs.GetString("NomeRecordista", "---");
            textoExibicaoRecorde.text = $"Recorde: {nome} - {Mathf.FloorToInt(recorde)}";
        }
    }

    public void AdicionarMoeda(int v) 
    {
        moedasColetadas += v;
        if (textoMoedas != null) textoMoedas.text = "Moedas: " + moedasColetadas;
    }

    public void ReiniciarJogo() 
    { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
}