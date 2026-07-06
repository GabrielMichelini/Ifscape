using UnityEngine;

public class SistemaDeMoeda : MonoBehaviour
{
    [Header("Configurações da Moeda")]
    public int valor = 1;
    public float velocidadeGiro = 120f; 
    
    [Header("Áudio")]
    public AudioClip somColeta; // NOVO: Caixinha para o som da moeda!
    
    private bool jaColetada = false; 

    void Update()
    {
        transform.Rotate(Vector3.up * velocidadeGiro * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider outro)
    {
        if (outro.CompareTag("Player") && !jaColetada)
        {
            jaColetada = true; 
            GameManager.instance.AdicionarMoeda(valor);

            // Toca o som de coleta diretamente na posição da câmera!
            if (somColeta != null)
            {
                AudioSource.PlayClipAtPoint(somColeta, Camera.main.transform.position, 0.8f);
            }

            Destroy(gameObject);
        }
    }
}