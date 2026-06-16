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
        // O mundo só corre se o GameManager autorizar
        if (player != null && GameManager.instance.jogoRodando)
        {
            transform.Translate(Vector3.back * player.forwardSpeed * Time.deltaTime, Space.World);
        }

        // Faxina automática para moedas soltas
        if (transform.position.z < -200f && !gameObject.CompareTag("Untagged"))
        {
            Destroy(gameObject);
        }
    }
}