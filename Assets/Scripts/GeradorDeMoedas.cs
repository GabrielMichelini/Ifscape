using UnityEngine;

public class Moeda : MonoBehaviour
{
    // Coloque este script no seu Prefab da Moeda!
    void OnTriggerEnter(Collider outro)
    {
        // A moeda SÓ pode ser destruída se quem encostou nela foi o AJ (Player)
        if (outro.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.AdicionarMoeda(1);
            }
            Destroy(gameObject);
        }
    }
}