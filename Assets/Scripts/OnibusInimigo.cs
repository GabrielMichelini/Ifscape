using UnityEngine;

public class OnibusInimigo : MonoBehaviour
{
    public float velocidadeExtra = 15f; 
    public float raioDoRadar = 3f; 
    
    // --- NOVO: Espaço para a sua partícula ---
    public GameObject particulaBatida; 

    private PlayerController player;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        Destroy(gameObject, 10f); // Destrói o ônibus depois de 10 segundos para não pesar o jogo
    }

    void Update()
    {
        if (player != null && player.enabled)
        {
            float velocidadeTotal = player.forwardSpeed + velocidadeExtra;
            transform.Translate(Vector3.forward * velocidadeTotal * Time.deltaTime, Space.World);
            
            // --- A GRANDE CORREÇÃO ESTÁ AQUI ---
            // Separamos a matemática para ele entender o que é "Pista" e o que é "Frente/Trás"
            float distanciaZ = Mathf.Abs(transform.position.z - player.transform.position.z);
            float distanciaX = Mathf.Abs(transform.position.x - player.transform.position.x);

            // Só dá Game Over se o ônibus estiver muito perto no eixo Z (batida) 
            // E EXATAMENTE na mesma pista (distância X menor que 1 metro)
            if (distanciaZ < 2.5f && distanciaX < 1.0f)
            {
                FindObjectOfType<GameManager>().GameOver();
            }
        }

        Collider[] objetosNoRadar = Physics.OverlapSphere(transform.position, raioDoRadar);
        foreach (Collider obj in objetosNoRadar)
        {
            AlvoDoOnibus alvo = obj.GetComponentInParent<AlvoDoOnibus>();
            
            if (alvo != null)
            {
                // --- NOVO: Cria a explosão EXATAMENTE na posição do armário ---
                if (particulaBatida != null)
                {
                    Instantiate(particulaBatida, alvo.transform.position, Quaternion.identity);
                }

                // E depois destrói o armário
                Destroy(alvo.gameObject);
            }
        }
    }
}