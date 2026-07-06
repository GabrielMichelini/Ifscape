using System.Collections.Generic;
using UnityEngine;

public class GeradorObstaculos : MonoBehaviour
{
    [Header("Prefabs dos Objetos")]
    public GameObject prefabArmario; 
    public GameObject prefabBarreiraBaixa; 
    public GameObject prefabMoeda; 
    public GameObject prefabPowerUp; 

    [Header("Frequência e Dificuldade")]
    [Range(0f, 1f)]
    public float chanceBaseObstaculo = 0.75f; 

    [Header("Ajustes de Rotação Inicial")]
    public float rotacaoYArmario = 0f; 
    public Vector3 rotacaoPowerUp = new Vector3(0, 0, 0); 

    [Header("Configuração de Spawn de Moedas")]
    public int moedasPorSequencia = 4; 
    public float espacamentoEntreMoedas = 2f; 

    private List<Transform> pontosDeSpawn = new List<Transform>(); 
    private List<GameObject> objetosInstanciados = new List<GameObject>(); 

    void Awake()
    {
        ConfigurarPontos();
    }

    void ConfigurarPontos()
    {
        pontosDeSpawn.Clear();
        foreach (Transform filho in transform)
        {
            if (filho.name.StartsWith("Ponto")) pontosDeSpawn.Add(filho);
        }
    }

    public void GerarNovoObstaculo()
    {
        LimparObstaculos();

        if (pontosDeSpawn.Count == 0) ConfigurarPontos();
        if (pontosDeSpawn.Count == 0) return;

        // 1. Escolhe a primeira pista bloqueada
        List<int> pistasBloqueadas = new List<int>();
        int pista1 = Random.Range(0, pontosDeSpawn.Count);
        pistasBloqueadas.Add(pista1);
        
        // --- NOVO: A DIFICULDADE CRUEL ---
        // Se a dificuldade passar de 1.5, tem 35% de chance de fechar DUAS pistas!
        if (GameManager.instance.dificuldade > 1.5f && Random.value < 0.35f)
        {
            int pista2 = Random.Range(0, pontosDeSpawn.Count);
            while (pista2 == pista1) // Garante que a pista 2 seja diferente da pista 1
            {
                pista2 = Random.Range(0, pontosDeSpawn.Count);
            }
            pistasBloqueadas.Add(pista2);
        }

        float chanceDoObstaculo = chanceBaseObstaculo + (GameManager.instance.dificuldade * 0.05f);

        // 2. GERA OS OBSTÁCULOS
        if (Random.value < chanceDoObstaculo)
        {
            foreach (int p in pistasBloqueadas)
            {
                if (pontosDeSpawn[p] != null)
                {
                    GameObject obstaculoSorteado = prefabArmario;
                    if (prefabBarreiraBaixa != null && Random.value > 0.5f) obstaculoSorteado = prefabBarreiraBaixa;

                    GameObject obstaculo = Instantiate(obstaculoSorteado, pontosDeSpawn[p].position, Quaternion.Euler(0, rotacaoYArmario, 0));
                    obstaculo.transform.SetParent(this.transform);
                    objetosInstanciados.Add(obstaculo);
                }
            }
        }

        bool jaSpawnouPoderAqui = false; 

        // 3. GERA OS ITENS NAS PISTAS LIVRES
        for (int i = 0; i < pontosDeSpawn.Count; i++)
        {
            // Verifica se a pista atual NÃO está na lista de bloqueadas
            if (!pistasBloqueadas.Contains(i) && pontosDeSpawn[i] != null)
            {
                if (prefabPowerUp != null && GameManager.instance.deveSpawnarPowerUp && !jaSpawnouPoderAqui) 
                {
                    GameObject powerUp = Instantiate(prefabPowerUp, pontosDeSpawn[i].position, Quaternion.Euler(rotacaoPowerUp));
                    powerUp.transform.SetParent(this.transform);
                    objetosInstanciados.Add(powerUp);
                    
                    GameManager.instance.deveSpawnarPowerUp = false; 
                    jaSpawnouPoderAqui = true;
                }
                else if (Random.value < 0.6f) 
                {
                    if (prefabMoeda != null) 
                    {
                        for (int j = 0; j < moedasPorSequencia; j++)
                        {
                            Vector3 posicaoMoeda = pontosDeSpawn[i].position + new Vector3(0, 0, j * espacamentoEntreMoedas);
                            GameObject moeda = Instantiate(prefabMoeda, posicaoMoeda, Quaternion.identity);
                            moeda.transform.SetParent(this.transform);
                            objetosInstanciados.Add(moeda);
                        }
                    }
                }
            }
        }
    }

    public void LimparObstaculos()
    {
        foreach (GameObject obj in objetosInstanciados)
        {
            if (obj != null) Destroy(obj);
        }
        objetosInstanciados.Clear();
    }
}