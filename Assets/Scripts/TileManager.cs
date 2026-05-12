using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] modelosDeRua; // Arraste seus Prefabs de rua aqui
    
    // O tamanho exato do seu modelo 3D da rua. Ajuste esse número se as ruas ficarem com buracos ou sobrepostas!
    public float tamanhoDaRua = 30f; 
    public int quantidadeNaTela = 5; // Quantas ruas ficam prontas rodando ao mesmo tempo

    private List<GameObject> ruasAtivas = new List<GameObject>();

    void Start()
    {
        // Cria as primeiras ruas para preencher a tela logo que o jogo começa
        for (int i = 0; i < quantidadeNaTela; i++)
        {
            // A primeira nasce no 0, a segunda no 30, a terceira no 60...
            SpawnRua(i * tamanhoDaRua);
        }
    }

    void Update()
    {
        // Verifica se a lista de ruas não está vazia e se a primeira rua ainda existe
        if (ruasAtivas.Count > 0 && ruasAtivas[0] != null)
        {
            // Se a primeira rua da fila foi muito para trás do AJ (passou da câmera)
            if (ruasAtivas[0].transform.position.z < -tamanhoDaRua)
            {
                // Pega a posição exata de onde a ÚLTIMA rua da esteira está agora
                float posicaoZDaUltimaRua = ruasAtivas[ruasAtivas.Count - 1].transform.position.z;
                
                // Cria uma rua nova exatamente colada no final dessa última
                SpawnRua(posicaoZDaUltimaRua + tamanhoDaRua);

                // Destrói a rua velha que ficou para trás e remove da lista
                Destroy(ruasAtivas[0]);
                ruasAtivas.RemoveAt(0);
            }
        }
    }

    void SpawnRua(float posicaoZ)
    {
        // Escolhe uma rua aleatória dos seus prefabs
        int index = Random.Range(0, modelosDeRua.Length);
        
        // Cria a rua no mundo
        GameObject novaRua = Instantiate(modelosDeRua[index], new Vector3(0, 0, posicaoZ), Quaternion.identity);
        
        // Adiciona ela na nossa lista de controle
        ruasAtivas.Add(novaRua);
    }
}