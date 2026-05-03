using System.Collections;
using UnityEngine;

public class GeradorDeOnibus : MonoBehaviour
{
    [Header("Objetos")]
    public GameObject onibusPrefab;
    public GameObject avisoPrefab;
    
    [Header("Configurações")]
    public float laneDistance = 3f; 
    public float tempoParaOPrimeiro = 20f; 
    public float intervaloEntreEles = 15f; 
    public float tempoDeAviso = 2f; 

    [Header("Ajustes Visuais")]
    public float alturaOnibus = 1.5f; 
    public float alturaAviso = 2f; 

    private Transform playerTransform;

    void Start()
    {
        playerTransform = FindObjectOfType<PlayerController>().transform;
        InvokeRepeating("IniciarAtaque", tempoParaOPrimeiro, intervaloEntreEles);
    }

    void IniciarAtaque()
    {
        int pistaSorteada = Random.Range(0, 3);
        StartCoroutine(SequenciaDoOnibus(pistaSorteada));
    }

    IEnumerator SequenciaDoOnibus(int pista)
    {
        float targetX = (pista - 1) * laneDistance;

        // 1. CRIA O AVISO NA PISTA SORTEADA
        Vector3 posicaoAviso = new Vector3(targetX, alturaAviso, playerTransform.position.z + 15f);
        GameObject aviso = Instantiate(avisoPrefab, posicaoAviso, avisoPrefab.transform.rotation);
        
        yield return new WaitForSeconds(tempoDeAviso);

        // 2. DESTROI O AVISO E CHAMA O ÔNIBUS
        if (aviso != null) Destroy(aviso);

        Vector3 posicaoOnibus = new Vector3(targetX, alturaOnibus, playerTransform.position.z - 40f);
        Instantiate(onibusPrefab, posicaoOnibus, onibusPrefab.transform.rotation);
    }
}