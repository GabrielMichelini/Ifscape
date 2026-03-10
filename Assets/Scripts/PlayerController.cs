using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 moveVector;
    public Animator animator;

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
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            desiredLane = Mathf.Clamp(desiredLane + 1, 0, 2);

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            desiredLane = Mathf.Clamp(desiredLane - 1, 0, 2);

        float targetX = (desiredLane - 1) * laneDistance;
        float newX = Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * sideSpeed);
        float xMovement = (newX - transform.position.x) / Time.deltaTime;

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

        moveVector.x = xMovement;
        moveVector.y = verticalVelocity;
        moveVector.z = forwardSpeed;

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
}