using UnityEngine;

public class MovimentoEsteira : MonoBehaviour
{
    private PlayerController player;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if (player != null && player.enabled)
        {
            transform.Translate(Vector3.back * player.forwardSpeed * Time.deltaTime, Space.World);
        }

        // Aumentamos o limite para -200! 
        // Assim ele limpa moedas velhas, mas nunca entra em conflito com o apagador de ruas.
        if (transform.position.z < -200f)
        {
            Destroy(gameObject);
        }
    }
}