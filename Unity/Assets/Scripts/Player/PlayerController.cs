using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    //* THIS SCRIPT CONTROLS THE PLAYER MOVEMENT AND LOOKING

    // PUBLICS
    [Header("Objects")]

    public Transform Camera;

    [SerializeField] private GameObject SlopeCheck;
    [SerializeField] private GameObject GroundCheck;
    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private PlayerLoader PlayerLoader;

  

    [Header("Movement & Cameras")]
    [SerializeField] private float MovementSpeed = 7f;
    [SerializeField] private float GroundDrag = 10f;
    [SerializeField] private float JumpForce = 300f;
    [SerializeField] private float CameraSensitivity = 1.5f;

    [Header("Camera Settings")]
    [SerializeField] private float SlopeMultiplier = 1.05f;
    public bool canUse {get; set;} = true;



    // PRIVATES
    private new Rigidbody rigidbody;
    private Vector2 currentCameraRotation;
    private RaycastHit SlopeHit;
    private Vector3 SlopeMoveDirection;
    private Vector3 MoveDirection;
    private bool grounded;



    // Start is called before the first frame update
    [Client]
    void Start()
    {
        if (!isLocalPlayer) return;

        // Load player objects.
        PlayerLoader.PlayerLoaded();
        
        // Set up the rigidbody and remove the cursor.
        rigidbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Jumping
        InputHandler.input.Game.Jump.performed += (InputAction.CallbackContext context)=> {if (context.performed && IsGrounded() && isLocalPlayer) {UserData.data.Coins += 0.23f; CoinUIUpdate.updateCoin.Invoke(); rigidbody.AddForce(Vector3.up * JumpForce);}};
    }

   
    // Update is called once per frame for physics.
    [Client]
    void FixedUpdate()
    {
        // Check user
        if (!isLocalPlayer) return;

        if (Input.GetKey(KeyCode.Escape))
        {
            if (isServer) 
            {
                Debug.Log("<color=#00FF00>[CLIENT] : Stopping as host.</color>");
                CustomNetworkManager.instance.gracefulDisconnect  = true;
                CustomNetworkManager.instance.StopHost();      
            }
            else
            {
                Debug.Log("<color=#00FF00>[CLIENT] : Stopping as client.</color>");
                CustomNetworkManager.instance.gracefulDisconnect  = true;
                CustomNetworkManager.instance.StopClient();
            }
        }

        // Check if we can use it.
        if (canUse)
        {
            // Check if grounded.
            grounded = IsGrounded();

            // Set grounded.
            if (grounded)
            {
                rigidbody.drag = GroundDrag;
            }
            else
            {
                rigidbody.drag = 0;
            }

            // Call main method.
            HandleMovement(); 
        }
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

        // Determine the forward and right directions based on the camera's orientation
        Vector3 forwardDirection = Camera.transform.forward; // Get camera's forward direction
        forwardDirection.y = 0; // Prevent upward movement
        forwardDirection.Normalize(); // Normalize to ensure consistent speed

        Vector3 rightDirection = Camera.transform.right; // Get camera's right direction
        rightDirection.y = 0; // Prevent upward movement
        rightDirection.Normalize(); // Normalize to ensure consistent speed

        // Set move directions.
        MoveDirection = ((forwardDirection * Vertical + rightDirection * Horizontal).normalized) * movementForce * (OnSlope() ? SlopeMultiplier : 1f);
        SlopeMoveDirection = Vector3.ProjectOnPlane(MoveDirection, SlopeHit.normal);

        // Check if we are grounded.
        if (grounded)
        {
            // Apply the force if we are on slope.
            if (OnSlope())
            {
                rigidbody.AddForce(movementForce * SlopeMoveDirection, ForceMode.Force);
            }
            else
            {
                rigidbody.AddForce(movementForce * MoveDirection, ForceMode.Force);
            }
        }

        if (stickDirection.magnitude > 0.1f)
        {
             // Calculate the target rotation based on movement direction
            Quaternion targetRotation = Quaternion.LookRotation(MoveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * CameraSensitivity);
        }

        // Get flat vel.
        Vector3 flatVel = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);

        // Limit velocity.
        if (flatVel.magnitude > MovementSpeed)
        {
            Vector3 limitedVelocity = flatVel.normalized * MovementSpeed;
            rigidbody.velocity = new Vector3(limitedVelocity.x, rigidbody.velocity.y, limitedVelocity.z);
        }
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