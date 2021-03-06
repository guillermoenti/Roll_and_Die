using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerScript : MonoBehaviour
{
    private CharacterController controller;
    private InputManager inputManager;
    private Transform cameraTransform;

    [SerializeField] private Weapon weapon;

    private Vector3 playerVelocity;
    private bool groundedPlayer;

    //private int speedHash;

    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;

    [SerializeField] GameObject model;

    [SerializeField] LayerMask interactLayerMask;

    [SerializeField] PlayerAudio aud;

    float timer = 0;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        cameraTransform = Camera.main.transform;
        //animator = GetComponent<Animator>();
    }


    void Update()
    {
        Movement();

        if (inputManager.HasPlayerInteractedThisFrame())
        {
            Interact();
        }
    }

    private void Movement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movement = inputManager.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0;
        controller.Move(move * Time.deltaTime * playerSpeed);

        if ((movement.x > 0 || movement.y > 0) && groundedPlayer && timer > 0.5f)
        {
            aud.PlayFootstep();
            timer = 0;
        }
        timer += Time.deltaTime;

        controller.transform.forward = new Vector3(cameraTransform.forward.x, controller.transform.forward.y, cameraTransform.forward.z);
        //model.transform.forward = new Vector3(controller.transform.forward.x, cameraTransform.transform.forward.y, controller.transform.forward.z);

        // Changes the height position of the player..
        if (inputManager.HasPlayerJumpedThisFrame() && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }


    private void HandleAnimations()
    {
        //animator.SetFloat(speedHash, )
    }


    private void Interact()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(cameraTransform.position, cameraTransform.TransformDirection(Vector3.forward), out hit, 100f, interactLayerMask))
        {
            if (hit.collider)
            {
                hit.transform.TryGetComponent(out Interactable interactable);
                if (interactable)
                {
                    interactable.OnInteract();
                }
            }
        }
    }

}

