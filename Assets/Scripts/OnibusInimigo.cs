using UnityEngine;

public class OnibusInimigo : MonoBehaviour
{
    public float velocidadeExtra = 15f; 
    private PlayerController player;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        // Aumentei o tempo de vida para garantir que ele ultrapasse o jogador antes de sumir
        Destroy(gameObject, 10f);
    }

    void Update()
    {
        if (player != null && player.enabled)
        {
            float velocidadeTotal = player.forwardSpeed + velocidadeExtra;
            // O Space.World é a mágica: obriga o ônibus a ir reto no eixo Z do mapa!
            transform.Translate(Vector3.forward * velocidadeTotal * Time.deltaTime, Space.World);
        }
    }

    void OnTriggerEnter(Collider outro)
    {
        if (outro.CompareTag("Player"))
        {
            FindObjectOfType<GameManager>().GameOver();
        }
    }
}