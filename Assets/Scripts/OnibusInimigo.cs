using UnityEngine;

public class OnibusInimigo : MonoBehaviour
{
    public float velocidadeExtra = 15f; 
    public float raioDoRadar = 3f; 
    public GameObject particulaBatida; 

    [Header("Áudio")]
    public AudioClip somBuzina; // NOVO: Caixinha para o som do ônibus!

    private PlayerController player;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();

        // Toca a buzina assim que ele nasce na rua!
        if (somBuzina != null)
        {
            AudioSource.PlayClipAtPoint(somBuzina, Camera.main.transform.position, 1f);
        }

        Destroy(gameObject, 10f); 
    }

    void Update()
    {
        if (player != null && player.enabled)
        {
            float velocidadeTotal = player.forwardSpeed + velocidadeExtra;
            transform.Translate(Vector3.forward * velocidadeTotal * Time.deltaTime, Space.World);
            
            float distanciaZ = Mathf.Abs(transform.position.z - player.transform.position.z);
            float distanciaX = Mathf.Abs(transform.position.x - player.transform.position.x);

            if (distanciaZ < 2.5f && distanciaX < 1.0f)
            {
                FindObjectOfType<GameManager>().GameOver();
            }
        }

        Collider[] objetosNoRadar = Physics.OverlapSphere(transform.position, raioDoRadar);
        foreach (Collider obj in objetosNoRadar)
        {
            if (obj.CompareTag("Obstaculo"))
            {
                if (particulaBatida != null)
                {
                    Instantiate(particulaBatida, obj.transform.position, Quaternion.identity);
                }
                Destroy(obj.gameObject); 
            }
        }
    }
}