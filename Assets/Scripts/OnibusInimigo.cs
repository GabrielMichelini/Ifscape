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
        Destroy(gameObject, 10f);
    }

    void Update()
    {
        if (player != null && player.enabled)
        {
            float velocidadeTotal = player.forwardSpeed + velocidadeExtra;
            transform.Translate(Vector3.forward * velocidadeTotal * Time.deltaTime, Space.World);
            
            float distanciaProAJ = Vector3.Distance(transform.position, player.transform.position);
            if (distanciaProAJ < 3f)
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