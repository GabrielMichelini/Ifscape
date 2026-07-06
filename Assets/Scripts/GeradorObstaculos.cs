using System.Collections.Generic;
using UnityEngine;

public class GeradorObstaculos : MonoBehaviour
{
    [Header("Prefabs dos Objetos")]
    public GameObject prefabArmario; 
    public GameObject prefabMoeda; 
    public GameObject prefabPowerUp; 

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
            if (filho.name.StartsWith("Ponto"))
            {
                pontosDeSpawn.Add(filho);
            }
        }
    }

    public void GerarNovoObstaculo()
    {
        LimparObstaculos();

        if (pontosDeSpawn.Count == 0) ConfigurarPontos();
        if (pontosDeSpawn.Count == 0) return;

        // 1. Sorteia o local do Armário
        int pistaDoArmario = Random.Range(0, pontosDeSpawn.Count);
        float chanceDoArmario = 0.4f + (GameManager.instance.dificuldade * 0.05f);

        if (Random.value < chanceDoArmario)
        {
            if (pontosDeSpawn[pistaDoArmario] != null)
            {
                GameObject armario = Instantiate(prefabArmario, pontosDeSpawn[pistaDoArmario].position, Quaternion.Euler(0, rotacaoYArmario, 0));
                armario.transform.SetParent(this.transform);
                objetosInstanciados.Add(armario);
            }
        }

        bool jaSpawnouPoderAqui = false; // Trava para não nascer 2 Power Ups lado a lado

        // 2. SISTEMA DE ITENS CORRIGIDO E ISOLADO
        for (int i = 0; i < pontosDeSpawn.Count; i++)
        {
            if (i != pistaDoArmario && pontosDeSpawn[i] != null)
            {
                // PRIORIDADE MÁXIMA: É a hora do Power Up? Ele nasce garantido!
                if (prefabPowerUp != null && GameManager.instance.deveSpawnarPowerUp && !jaSpawnouPoderAqui) 
                {
                    GameObject powerUp = Instantiate(prefabPowerUp, pontosDeSpawn[i].position, Quaternion.Euler(rotacaoPowerUp));
                    powerUp.transform.SetParent(this.transform);
                    objetosInstanciados.Add(powerUp);
                    
                    // Avisa o GameManager que o item já nasceu e trava para as outras pistas do mesmo chão
                    GameManager.instance.deveSpawnarPowerUp = false; 
                    jaSpawnouPoderAqui = true;

                    // ALARME NO CONSOLE PARA VOCÊ ACHAR O ITEM SE ELE ESTIVER INVISÍVEL
                    Debug.Log("🚨 O POWER UP NASCEU FISICAMENTE NA PISTA " + i);
                }
                // SE NÃO FOR A HORA DO POWER UP: Roda o sorteio normal de 60% para as moedas
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