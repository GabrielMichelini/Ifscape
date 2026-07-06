using UnityEngine;

public class SistemaDePowerUp : MonoBehaviour
{
    [Header("Configurações do Power Up")]
    public float velocidadeGiro = 100f; 
    
    [Header("Áudio")]
    public AudioClip somPoder; // Som que toca quando pega o escudo/estrela
    
    private bool jaColetado = false; 

    void Update()
    {
        // Gira o item no ar (Pode apagar qualquer script antigo de girar que estiver nele)
        transform.Rotate(Vector3.up * velocidadeGiro * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider outro)
    {
        // Se quem encostou tem a tag Player e o poder ainda não foi pego...
        if (outro.CompareTag("Player") && !jaColetado)
        {
            jaColetado = true; // Trava para não pegar duas vezes

            // Toca o som do poder
            if (somPoder != null)
            {
                AudioSource.PlayClipAtPoint(somPoder, Camera.main.transform.position, 1f);
            }

            // Procura o script do AJ e manda ele ligar a invencibilidade
            PlayerController jogador = outro.GetComponent<PlayerController>();
            if (jogador != null)
            {
                jogador.AtivarPoder(); // Chama a nova função que criamos lá no AJ
            }

            // Destrói o item da pista
            Destroy(gameObject);
        }
    }
}