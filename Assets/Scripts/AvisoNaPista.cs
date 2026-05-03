using UnityEngine;

public class AvisoNaPista : MonoBehaviour
{
    private Transform player;
    private float meuX; // Guarda a pista sorteada

    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        // Salva o X exato da pista em que o Gerador mandou ele nascer
        meuX = transform.position.x; 
    }

    void Update()
    {
        if (player != null)
        {
            // Acompanha o jogador na distância Z, mas NUNCA sai da pista X sorteada
            transform.position = new Vector3(meuX, transform.position.y, player.position.z + 15f);
        }
    }
}