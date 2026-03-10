using UnityEngine;

public class GeradorObstaculos : MonoBehaviour
{
    public GameObject obstaculoPrefab; 
    public Transform[] pontosDeSpawn;  
    public float zonaSegura = 40f; 

    private GameManager gm;

    void Start()
    {
        // Acha o GameManager para saber em qual dificuldade o jogo está
        gm = FindObjectOfType<GameManager>();

        if (transform.position.z < zonaSegura) return; 

        // 1. A CHANCE DE VAZIO DIMINUI COM O TEMPO
        // Começa em 40%, mas se a dificuldade for 2, cai para 20%, gerando muito mais armários.
        float chanceVazio = 40f / (gm != null ? gm.dificuldade : 1f);
        if (Random.Range(0, 100) < chanceVazio) return;

        // 2. QUANTOS ARMÁRIOS VÃO NASCER? (Bloquear 1 ou 2 pistas)
        int qtdObstaculos = 1;
        
        // Se a dificuldade já passou de 1.5, tem 30% de chance de nascer 2 armários lado a lado!
        if (gm != null && gm.dificuldade > 1.5f && Random.Range(0, 100) < 30) 
        {
            qtdObstaculos = 2; 
        }

        // Sorteia a primeira pista e cria o armário
        int pista1 = Random.Range(0, pontosDeSpawn.Length);
        CriarArmario(pista1);

        // Se o jogo decidiu que são 2 armários, cria o segundo em uma pista diferente
        if (qtdObstaculos == 2)
        {
            int pista2 = Random.Range(0, pontosDeSpawn.Length);
            while (pista2 == pista1) // Garante que não vai nascer um dentro do outro
            {
                pista2 = Random.Range(0, pontosDeSpawn.Length);
            }
            CriarArmario(pista2);
        }
    }

    // Função separada apenas para organizar a criação e impedir que o armário estique
    void CriarArmario(int indicePista)
    {
        GameObject novoObstaculo = Instantiate(obstaculoPrefab, pontosDeSpawn[indicePista].position, pontosDeSpawn[indicePista].rotation);
        novoObstaculo.transform.SetParent(transform, true);
    }
}