using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //* THIS SCRIPT CONTROLS THE PLAYER MOVEMENT AND LOOKING

    public static PlayerController instance {get; private set;}
    // PUBLICS
    [Header("Objects")]
    public GameObject CameraHolder;
    public Transform Camera;
    [SerializeField] private GameObject Orientation;
    [SerializeField] private GameObject CameraPosition;
    [SerializeField] private GameObject SlopeCheck;
    [SerializeField] private GameObject GroundCheck;
    [SerializeField] private LayerMask GroundLayer;

    [Header("Movement & Cameras")]
    [SerializeField] private float MovementSpeed = 27.5f;
    [SerializeField] private float MaxMovementVelocity = 27.5f;
    [SerializeField] private float JumpForce = 5f;
    [SerializeField] private float CameraSensitivity = 1.5f;

    [Header("Camera Settings")]
    [SerializeField] private float SlopeMultiplier = 1.1f;
    public bool canUse {get; set;} = true;



    // PRIVATES
    private new Rigidbody rigidbody;
    private Vector2 currentCameraRotation;
    private RaycastHit SlopeHit;
    private Vector3 SlopeMoveDirection;
    private Vector3 MoveDirection;


    void Awake()
    {
        // Create singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set up the rigidbody and remove the cursor.
        rigidbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Jumping
        InputHandler.input.Game.Jump.performed += (InputAction.CallbackContext context)=> {if (context.performed && IsGrounded()) {rigidbody.AddForce(Vector3.up * JumpForce);}};
    }


    // Update is called once per frame for physics.
    void FixedUpdate()
    {
        // Check if we can use it.
        if (canUse)
        {
            HandleMovement(); 
        }
    }

    void LateUpdate()
    {
        // Check if we can use it.
        if (canUse)
        {
            HandleCamera();
        }
    }


    /// <summary>
    /// Handles the camera, call in LateUpdate.
    /// </summary>
    void HandleCamera()
    {
        // Get the mouse inputs and add them to current rotation vector.
        currentCameraRotation.x -= Input.GetAxisRaw("Mouse Y") * CameraSensitivity;
        currentCameraRotation.y += Input.GetAxisRaw("Mouse X") * CameraSensitivity;
        currentCameraRotation.x = Mathf.Clamp(currentCameraRotation.x, -85, 85);

        // Set the camera position to the camera position gameobject.
        CameraHolder.transform.position = CameraPosition.transform.position;
        
        // Orientation object is set to the correct rotation, then the players rotation is set to it to prevent player model stuttering, but when setting the camera rotation it doesn't work.
        Orientation.transform.rotation = Quaternion.Euler(0, currentCameraRotation.y, 0);
        CameraHolder.transform.rotation = Quaternion.Euler(currentCameraRotation.x, currentCameraRotation.y, 0);
    }

    /// <summary>
    /// Handles the movement, call in Update.
    /// </summary>
    void HandleMovement()
    {
        // Get the movement inputs from the input handler.
        Vector2 stickDirection = InputHandler.input.Game.Movement.ReadValue<Vector2>();

        // Get the movement directions and multiply them by time.deltatime;
        float Horizontal = stickDirection.x * Time.deltaTime;
        float Vertical = stickDirection.y * Time.deltaTime;
        float movementForce = MovementSpeed;

        // Set move directions.
        MoveDirection = ((Orientation.transform.forward * Vertical + Orientation.transform.right * Horizontal).normalized) * movementForce * (OnSlope() ? SlopeMultiplier : 1f);
        SlopeMoveDirection = Vector3.ProjectOnPlane(MoveDirection, SlopeHit.normal);

        // Max force
        float maxForce = (Mathf.Max(0.0f, MaxMovementVelocity - rigidbody.velocity.magnitude) * rigidbody.mass) / Time.fixedDeltaTime;
   
        // Apply the force if we are on slope.
        if (OnSlope())
        {
            rigidbody.AddForce(Mathf.Min(maxForce, movementForce) * SlopeMoveDirection, ForceMode.Force);
        }
        else
        {
            rigidbody.AddForce(Mathf.Min(maxForce, movementForce) * MoveDirection, ForceMode.Force);
        }

        // Using gravity.
        rigidbody.useGravity = !OnSlope();
    }


    private bool OnSlope()
    {
        // Raycast to check the normal.
        if (Physics.Raycast(SlopeCheck.transform.position, Vector3.down, out SlopeHit, 2f))
        {
            if (SlopeHit.normal != Vector3.up)
            {
                return true;
            }
            {
                return false;
            }
        }
        return false;
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(GroundCheck.transform.position, 0.15f, GroundLayer);
    }
}