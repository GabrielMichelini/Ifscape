using System.Collections.Generic;
using UnityEngine;

public class GeradorObstaculos : MonoBehaviour
{
    [Header("Prefabs dos Objetos")]
    public GameObject prefabArmario; 
    public GameObject prefabMoeda; 
    public GameObject prefabPowerUp; 

    [Header("Ajustes de Rotação Inicial")]
    [Tooltip("Se o armário nascer virado errado, mude aqui (ex: 0, 90, 180, 270)")]
    public float rotacaoYArmario = 0f; 
    [Tooltip("Se o Power Up nascer virado errado, mude aqui")]
    public float rotacaoYPowerUp = 0f; 

    [Header("Configuração da Sequência de Moedas")]
    public int moedasPorSequencia = 4; // Quantas moedas nascem em fila
    public float espacamentoEntreMoedas = 2f; // Distância (Z) entre uma moeda e outra na fileira

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

        // 1. Sorteia uma pista para o Obstáculo (Armário)
        int pistaDoArmario = Random.Range(0, pontosDeSpawn.Count);
        float chanceDoArmario = 0.4f + (GameManager.instance.dificuldade * 0.05f);

        if (Random.value < chanceDoArmario)
        {
            if (pontosDeSpawn[pistaDoArmario] != null)
            {
                // Cria o armário usando o ângulo corrigido
                GameObject armario = Instantiate(prefabArmario, pontosDeSpawn[pistaDoArmario].position, Quaternion.Euler(0, rotacaoYArmario, 0));
                armario.transform.SetParent(this.transform);
                objetosInstanciados.Add(armario);
            }
        }

        // 2. SISTEMA DE ITENS: Nas pistas livres, gera Sequências de Moedas ou o Power Up
        for (int i = 0; i < pontosDeSpawn.Count; i++)
        {
            if (i != pistaDoArmario && pontosDeSpawn[i] != null)
            {
                if (Random.value < 0.6f) // 60% de chance de popular a pista livre
                {
                    // 8% de chance de nascer o Power Up em vez de moedas
                    if (prefabPowerUp != null && Random.value < 0.08f) 
                    {
                        GameObject powerUp = Instantiate(prefabPowerUp, pontosDeSpawn[i].position, Quaternion.Euler(0, rotacaoYPowerUp, 0));
                        powerUp.transform.SetParent(this.transform);
                        objetosInstanciados.Add(powerUp);
                    }
                    else if (prefabMoeda != null) // Cria a trilha de moedas em linha reta (Z)
                    {
                        for (int j = 0; j < moedasPorSequencia; j++)
                        {
                            // Calcula a posição de cada moeda empurrando elas para frente no eixo Z
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