using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 moveVector;
    public Animator animator;

    [Header("Movimentação")]
    public float forwardSpeed = 15f; 
    public float laneDistance = 3f; 
    public float jumpForce = 8f;
    public float gravity = -20f;
    public float sideSpeed = 15f;

    [Header("Configuração de Rolagem")]
    public float rollHeight = 0.5f;
    public float rollDuration = 1.0f;
    private bool isRolling = false;
    private float originalHeight;
    private Vector3 originalCenter;

    [Header("Sensor de Dano")]
    public float distanciaDeteccao = 0.6f;

    [Header("Power Up (Invencibilidade)")]
    public float duracaoDoPoder = 5f; 
    private bool isInvencivel = false; 

    private int desiredLane = 1; 
    private float verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        originalHeight = controller.height;
        originalCenter = controller.center;
    }

    void Update()
    {
        if (!GameManager.instance.jogoRodando) return;

        // --- SENSOR A LASER CONTRA ARMÁRIOS ---
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.forward, out hit, distanciaDeteccao))
        {
            if (hit.collider.CompareTag("Obstaculo") || hit.collider.transform.root.CompareTag("Obstaculo"))
            {
                if (isInvencivel) Destroy(hit.collider.gameObject);
                else GameManager.instance.GameOver();
            }
        }

        // --- CONTROLES DE PISTA ---
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            desiredLane = Mathf.Clamp(desiredLane + 1, 0, 2);

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            desiredLane = Mathf.Clamp(desiredLane - 1, 0, 2);

        float targetX = (desiredLane - 1) * laneDistance;
        float newX = Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * sideSpeed);
        float xMovement = (newX - transform.position.x) / Time.deltaTime;

        // --- PULO E GRAVIDADE ---
        if (controller.isGrounded)
        {
            verticalVelocity = -1f; 
            if (animator != null) animator.SetBool("isGrounded", true);

            if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && !isRolling)
            {
                verticalVelocity = jumpForce;
                if (animator != null) animator.SetTrigger("Jump");
            }
            else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && !isRolling)
            {
                StartCoroutine(Roll());
            }
        }
        else 
        {
            verticalVelocity += gravity * Time.deltaTime;
            if (animator != null) animator.SetBool("isGrounded", false);

            if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && !isRolling)
            {
                verticalVelocity = -jumpForce; 
                StartCoroutine(Roll());
            }
        }

        // --- APLICA O MOVIMENTO ---
        moveVector.x = xMovement;
        moveVector.y = verticalVelocity;
        moveVector.z = 0f; 

        controller.Move(moveVector * Time.deltaTime);
    }

    private IEnumerator Roll()
    {
        isRolling = true;
        if (animator != null) animator.SetTrigger("Roll");

        controller.height = rollHeight;
        controller.center = new Vector3(originalCenter.x, rollHeight / 2f, originalCenter.z);

        yield return new WaitForSeconds(rollDuration);

        controller.height = originalHeight;
        controller.center = originalCenter;
        isRolling = false;
    }

    // --- FUNÇÃO PÚBLICA PARA O POWER UP ACESSAR ---
    public void AtivarPoder()
    {
        StartCoroutine(RotinaInvencibilidade());
    }

    // --- NOVA ROTINA: O EFEITO DE BRILHO (ESTRELA) ---
    private IEnumerator RotinaInvencibilidade()
    {
        isInvencivel = true;

        // Pega todas as partes do modelo 3D
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        
        // Dicionário para salvar as cores originais da roupa
        Dictionary<Material, Color> coresOriginais = new Dictionary<Material, Color>();

        // Salva as cores originais antes de começar a piscar
        foreach (Renderer r in renderers)
        {
            if (r.gameObject == this.gameObject) continue; // Pula o cubo fantasma
            
            foreach (Material mat in r.materials)
            {
                if (mat.HasProperty("_Color") && !coresOriginais.ContainsKey(mat))
                {
                    coresOriginais.Add(mat, mat.color);
                }
            }
        }

        float tempoPassado = 0f;
        float velocidadePiscar = 0.15f; 
        bool corAlternada = false;

        // Fica trocando de cor enquanto o tempo do poder não acabar
        while (tempoPassado < duracaoDoPoder)
        {
            foreach (Renderer r in renderers) 
            {
                if (r.gameObject == this.gameObject) continue;
                
                foreach (Material mat in r.materials)
                {
                    if (mat.HasProperty("_Color"))
                    {
                        // Se for a vez de alternar, pinta de Amarelo. Se não, volta pra cor original.
                        mat.color = corAlternada ? Color.yellow : coresOriginais[mat]; 
                    }
                }
            }
            
            corAlternada = !corAlternada; // Inverte para a próxima piscada
            yield return new WaitForSeconds(velocidadePiscar);
            tempoPassado += velocidadePiscar;
        }

        // Quando o poder acaba, garante que todas as cores voltem ao normal!
        foreach (Renderer r in renderers) 
        {
            if (r.gameObject == this.gameObject) continue;
            
            foreach (Material mat in r.materials)
            {
                if (mat.HasProperty("_Color") && coresOriginais.ContainsKey(mat))
                {
                    mat.color = coresOriginais[mat];
                }
            }
        }
        
        isInvencivel = false;
    }

    // --- SENSOR PARA BATER NOS OBSTÁCULOS ---
    void OnTriggerEnter(Collider outro)
    {
        if (outro.gameObject.CompareTag("Obstaculo"))
        {
            if (isInvencivel) Destroy(outro.gameObject);
            else GameManager.instance.GameOver();
        }
    }
}