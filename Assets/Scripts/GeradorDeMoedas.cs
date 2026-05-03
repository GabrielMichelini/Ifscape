using UnityEngine;

public class GeradorDeMoedas : MonoBehaviour
{
    [Header("Configurações da Moeda")]
    public GameObject moedaPrefab;
    
    [Range(0, 100)]
    public int chanceDeNascer = 50; // 50% de chance de aparecer uma fileira neste pedaço de chão
    
    public int quantidadePorLinha = 5; // Quantas moedas seguidas vão aparecer
    public float distanciaEntreMoedas = 2f; // O espaço entre uma moeda e outra
    public float alturaDaMoeda = 1f; // Altura para não ficarem enterradas no asfalto

    [Header("Configurações da Pista")]
    public float laneDistance = 3f; // TEM que ser o mesmo valor que está no seu PlayerController!

    void Start()
    {
        // Só tenta gerar moedas se o prefab da moeda estiver configurado
        if (moedaPrefab == null) return;

        // Sorteia um número. Se cair dentro da chance, ele cria as moedas.
        if (Random.Range(0, 100) < chanceDeNascer)
        {
            // Sorteia a pista: 0 (Esquerda), 1 (Meio), 2 (Direita)
            int pistaSorteada = Random.Range(0, 3);
            CriarLinhaDeMoedas(pistaSorteada);
        }
    }

    void CriarLinhaDeMoedas(int pista)
    {
        // Usa a mesma matemática do seu jogador para achar a posição X exata da pista
        float targetX = (pista - 1) * laneDistance;

        // Cria a fileira de moedas
        for (int i = 0; i < quantidadePorLinha; i++)
        {
            // Calcula a posição (o Z vai aumentando para alinhar as moedas uma atrás da outra)
            Vector3 posicaoLocal = new Vector3(targetX, alturaDaMoeda, i * distanciaEntreMoedas);
            
            // Pega a posição do chão no mundo e soma com a posição da moeda
            Vector3 posicaoFinal = transform.position + posicaoLocal;

            // Cria a moeda na tela e coloca ela como filha deste pedaço de chão
            // (Assim, quando o chão for destruído lá atrás, as moedas que sobraram são destruídas junto!)
            Instantiate(moedaPrefab, posicaoFinal, Quaternion.identity);
        }
    }
}