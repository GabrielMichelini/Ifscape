using System.Collections.Generic;
using UnityEngine;

public class GeradorObstaculos : MonoBehaviour
{
    [Header("Prefabs dos Objetos")]
    public GameObject prefabArmario; 
    public GameObject prefabMoeda; // NOVO: Arraste o Prefab da sua moeda aqui!

    [Header("Ajuste de Rotação do Armário")]
    [Tooltip("Se o armário nascer de costas ou de lado, mude esse número no Inspector (ex: 0, 90, 180, 270)")]
    public float rotacaoYArmario = 0f; 

    private List<Transform> pontosDeSpawn = new List<Transform>(); 
    private List<GameObject> objetosInstanciados = new List<GameObject>(); // Guarda tudo para limpar depois

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

        // Garante que os pontos estão carregados
        if (pontosDeSpawn.Count == 0) ConfigurarPontos();
        if (pontosDeSpawn.Count == 0) return;

        // 1. Sorteia uma pista para ser a do Obstáculo (Armário)
        int pistaDoArmario = Random.Range(0, pontosDeSpawn.Count);

        // Calcula a chance do armário aparecer com base na dificuldade
        float chanceDoArmario = 0.4f + (GameManager.instance.dificuldade * 0.05f);

        if (Random.value < chanceDoArmario)
        {
            if (pontosDeSpawn[pistaDoArmario] != null)
            {
                // Cria o armário usando o ângulo customizado que você escolheu no Inspector
                GameObject armario = Instantiate(prefabArmario, pontosDeSpawn[pistaDoArmario].position, Quaternion.Euler(0, rotacaoYArmario, 0));
                armario.transform.SetParent(this.transform);
                objetosInstanciados.Add(armario);
            }
        }

        // 2. SISTEMA DE MOEDAS: Nas pistas que não têm armário, tenta gerar moedas!
        for (int i = 0; i < pontosDeSpawn.Count; i++)
        {
            // Se essa pista não for a pista que escolhemos para o armário...
            if (i != pistaDoArmario && pontosDeSpawn[i] != null)
            {
                // 60% de chance de nascer uma moeda nas pistas livres
                if (Random.value < 0.6f && prefabMoeda != null)
                {
                    GameObject moeda = Instantiate(prefabMoeda, pontosDeSpawn[i].position, Quaternion.identity);
                    moeda.transform.SetParent(this.transform);
                    objetosInstanciados.Add(moeda);
                }
            }
        }
    }

    public void LimparObstaculos()
    {
        // Destrói todos os armários e moedas antigos antes de criar novos
        foreach (GameObject obj in objetosInstanciados)
        {
            if (obj != null) Destroy(obj);
        }
        objetosInstanciados.Clear();
    }
}