using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 moveVector;
    public Animator animator;

    // Essa variável continua existindo, mas agora ela dita a "velocidade da esteira" para o resto do mundo!
    public float forwardSpeed = 15f; 
    public float laneDistance = 3f; 
    public float jumpForce = 8f;
    public float gravity = -20f;
    public float sideSpeed = 15f;

    private int desiredLane = 1; 
    private float verticalVelocity;

    private bool isRolling = false;
    private float originalHeight;
    private Vector3 originalCenter;
    public float rollHeight = 0.5f;
    public float rollDuration = 1.0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        originalHeight = controller.height;
        originalCenter = controller.center;
    }

    void Update()
    {
        // --- CONTROLES DE PISTA ---
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            desiredLane = Mathf.Clamp(desiredLane + 1, 0, 2);

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            desiredLane = Mathf.Clamp(desiredLane - 1, 0, 2);

        // Movimento lateral suave
        float targetX = (desiredLane - 1) * laneDistance;
        float newX = Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * sideSpeed);
        float xMovement = (newX - transform.position.x) / Time.deltaTime;

        // --- PULO E GRAVIDADE ---
        if (controller.isGrounded)
        {
            verticalVelocity = -1f; 
            if (animator != null) animator.SetBool("isGrounded", true);

            // Pulo
            if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && !isRolling)
            {
                verticalVelocity = jumpForce;
                if (animator != null) animator.SetTrigger("Jump");
            }
            // Rolada
            else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && !isRolling)
            {
                StartCoroutine(Roll());
            }
        }
        else // Se estiver no ar
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
        
        // A GRANDE MUDANÇA: O Z (frente) agora é ZERO. O AJ não corre mais para frente fisicamente.
        moveVector.z = 0f; 

        controller.Move(moveVector * Time.deltaTime);
    }

    // --- SISTEMA DE ROLADA ---
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

    // --- SISTEMA DE COLISÃO (MORTE) ---
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Obstaculo"))
        {
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.GameOver();
            }
        }
    }

    // --- SISTEMA DE COLETA (MOEDAS) ---
    void OnTriggerEnter(Collider outro)
    {
        if (outro.gameObject.CompareTag("Moeda"))
        {
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.AdicionarMoeda(1);
            }
            Destroy(outro.gameObject);
        }
    }
}