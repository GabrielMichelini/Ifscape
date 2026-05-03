using UnityEngine;

public class GiraMoeda : MonoBehaviour
{
    public float velocidadeGiro = 150f;

    void Start()
    {
        // Destrói a moeda automaticamente após 10 segundos para não pesar o jogo
        Destroy(gameObject, 10f); 
    }

    void Update()
    {
        transform.Rotate(0f, velocidadeGiro * Time.deltaTime, 0f);
    }
}