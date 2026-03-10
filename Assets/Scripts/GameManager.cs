using UnityEngine;
using TMPro; // Necessário para mexer no texto do placar
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject telaGameOver;
    public PlayerController playerScript;

    // --- Sistema de Pontuação e Dificuldade ---
    public TextMeshProUGUI textoPlacar;
    private float pontuacao;
    public float dificuldade = 1f; // O jogo começa na dificuldade 1

    private bool jogoRodando = true;

    void Start()
    {
        telaGameOver.SetActive(false);
        Time.timeScale = 1f; 
        pontuacao = 0;
    }

    void Update()
    {
        if (jogoRodando)
        {
            // 1. Os pontos aumentam com base na velocidade do personagem
            pontuacao += playerScript.forwardSpeed * Time.deltaTime;
            
            // Atualiza o texto na tela (Mathf.FloorToInt arredonda os números quebrados)
            if (textoPlacar != null)
            {
                textoPlacar.text = Mathf.FloorToInt(pontuacao).ToString();
            }

            // 2. A dificuldade sobe lentamente com o tempo (ex: +0.02 por segundo)
            dificuldade += 0.02f * Time.deltaTime;
        }
    }

    public void GameOver()
    {
        jogoRodando = false;
        telaGameOver.SetActive(true);
        playerScript.forwardSpeed = 0;
        playerScript.enabled = false;
    }

    public void ReiniciarJogo()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}