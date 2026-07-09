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
    public float velocidadeMaximaDaEsteira = 35f; 
    public float aceleracaoPorSegundo = 0.2f; 

    [Header("Frequência do Power Up")]
    [Tooltip("De quantos em quantos pontos o escudo vai aparecer? (Ex: 75, 100, 150)")]
    public float pontosParaPowerUp = 100f; 

    [HideInInspector] public bool deveSpawnarPowerUp = false;
    private float proximoMarcoPowerUp;

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

        deveSpawnarPowerUp = false;
        proximoMarcoPowerUp = pontosParaPowerUp; 
        
        AtualizarTextoDoRecorde();

        if (textoCountdown != null)
        {
            textoCountdown.gameObject.SetActive(true);
            textoCountdown.text = "PRESSIONE QUALQUER TECLA";
        }
    }

    void Update()
    {
        if (!jogoIniciou && Input.anyKeyDown)
        {
            jogoIniciou = true;
            StartCoroutine(ContagemRegressiva());
        }

        if (jogoRodando)
        {
            pontuacao += playerScript.forwardSpeed * Time.deltaTime;
            if (textoPlacar != null) textoPlacar.text = Mathf.FloorToInt(pontuacao).ToString();

            dificuldade += 0.02f * Time.deltaTime;

            if (playerScript != null && playerScript.forwardSpeed < velocidadeMaximaDaEsteira)
            {
                playerScript.forwardSpeed += aceleracaoPorSegundo * Time.deltaTime;
            }

            // Verifica se bateu o marco de pontos
            if (pontuacao >= proximoMarcoPowerUp)
            {
                deveSpawnarPowerUp = true;
                proximoMarcoPowerUp += pontosParaPowerUp; 
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
        GetComponent<AudioSource>().Stop();
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