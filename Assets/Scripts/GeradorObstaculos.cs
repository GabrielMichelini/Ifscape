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

    // --- AGORA TEM CONTROLO TOTAL (X, Y e Z) SOBRE TUDO! ---
    [Header("Ajustes de Rotação Inicial (X, Y, Z)")]
    public Vector3 rotacaoArmario = new Vector3(0, 0, 0); 
    public Vector3 rotacaoBarreira = new Vector3(0, 0, 0); 
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

        List<int> pistasBloqueadas = new List<int>();
        int pista1 = Random.Range(0, pontosDeSpawn.Count);
        pistasBloqueadas.Add(pista1);
        
        if (GameManager.instance.dificuldade > 1.5f && Random.value < 0.35f)
        {
            int pista2 = Random.Range(0, pontosDeSpawn.Count);
            while (pista2 == pista1) 
            {
                pista2 = Random.Range(0, pontosDeSpawn.Count);
            }
            pistasBloqueadas.Add(pista2);
        }

        float chanceDoObstaculo = chanceBaseObstaculo + (GameManager.instance.dificuldade * 0.05f);

        if (Random.value < chanceDoObstaculo)
        {
            foreach (int p in pistasBloqueadas)
            {
                if (pontosDeSpawn[p] != null)
                {
                    GameObject obstaculoSorteado = prefabArmario;
                    Vector3 rotacaoSorteada = rotacaoArmario; // Usa o Vector3 completo

                    if (prefabBarreiraBaixa != null && Random.value > 0.5f) 
                    {
                        obstaculoSorteado = prefabBarreiraBaixa;
                        rotacaoSorteada = rotacaoBarreira;
                    }

                    // Aplica a rotação exata que definir no Inspector
                    GameObject obstaculo = Instantiate(obstaculoSorteado, pontosDeSpawn[p].position, Quaternion.Euler(rotacaoSorteada));
                    obstaculo.transform.SetParent(this.transform);
                    objetosInstanciados.Add(obstaculo);
                }
            }
        }

        bool jaSpawnouPoderAqui = false; 

        for (int i = 0; i < pontosDeSpawn.Count; i++)
        {
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