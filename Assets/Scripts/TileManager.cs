using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] modelosDeRua; 
    
    [Header("Configurações de Encaixe")]
    public float tamanhoDaRua = 30f; 
    public int quantidadeNaTela = 6; 

    private List<GameObject> ruasAtivas = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < quantidadeNaTela; i++)
        {
            if (modelosDeRua.Length == 0) return;

            int index = Random.Range(0, modelosDeRua.Length);
            GameObject novaRua = Instantiate(modelosDeRua[index], new Vector3(0, 0, i * tamanhoDaRua), Quaternion.identity);
            
            // Pega o gerador da rua
            GeradorObstaculos gerador = novaRua.GetComponent<GeradorObstaculos>();
            
            // As 2 primeiras ruas do jogo começam limpas (pro jogador respirar), as outras já começam com jogo acontecendo!
            if (i < 2)
            {
                if (gerador != null) gerador.LimparObstaculos();
            }
            else
            {
                if (gerador != null) gerador.GerarNovoObstaculo();
            }

            ruasAtivas.Add(novaRua);
        }
    }

    void Update()
    {
        if (ruasAtivas.Count == 0) return;

        // Recicla o chão quando ele sai totalmente da visão da câmera
        if (ruasAtivas[0].transform.position.z < -50f)
        {
            GameObject ruaParaMover = ruasAtivas[0];
            ruasAtivas.RemoveAt(0);

            float novaPosicaoZ = ruasAtivas[ruasAtivas.Count - 1].transform.position.z + tamanhoDaRua;
            ruaParaMover.transform.position = new Vector3(0, 0, novaPosicaoZ);

            // Sorteia tudo de novo (Armários e Moedas) lá na frente!
            GeradorObstaculos gerador = ruaParaMover.GetComponent<GeradorObstaculos>();
            if (gerador != null) gerador.GerarNovoObstaculo();

            ruasAtivas.Add(ruaParaMover);
        }
    }
}