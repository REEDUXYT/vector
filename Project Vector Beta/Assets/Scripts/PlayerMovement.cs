using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float playerHeight = 2f;

    [SerializeField] Transform orientation;
    [Header("Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] private float fov;
    [SerializeField] private float fovSprint;
    [SerializeField] private float sprintFovTime;

    [Header("Movement")]
    public float moveSpeed = 6f;
    float movementMultiplier = 10f;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float airMultiplier = 0.2f;

    [Header("Sprinting")]
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float sprintSpeed = 6f;
    //[SerializeField] float crouchSpeed = 2f;

    [Header("Jumping")]
    public float jumpForce = 5f;
    public float gravityDownForce = 20f;

    public float jumpCounter = 0f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    //[SerializeField] KeyCode crouchKey = KeyCode.LeftCtrl;

    [Header("Drag")]
    [SerializeField] float groundDrag = 6f;
    [SerializeField] float airDrag = 2f;

    float horizontalMovement;
    float verticalMovement;

    [Header("Ground Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask wallMask;
    bool isGrounded;
    bool isWalled;
    float groundDistance = 0.4f;
    float wallDistance = 0.6f;

    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    Rigidbody rb;

    RaycastHit slopeHit;

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        isWalled = Physics.CheckSphere(wallCheck.position, wallDistance, wallMask);

        if (isGrounded)
        {
            print("The Player is Grounded.");
        }
        else if (isWalled)
        {
            print("The Player is touching a Wall.");
        }

        MyInput();
        ControlDrag();
        ControlSpeed();
        Jump();

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

    void Jump()
    {
        if (Input.GetKeyDown(jumpKey) && (isGrounded) && jumpCounter == 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
        else if (Input.GetKeyDown(jumpKey) && (isGrounded) && jumpCounter == 1)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            jumpCounter = 2f;
        }
        else if (Input.GetKeyDown(jumpKey) && (isGrounded) && jumpCounter == 2)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
        else if (Input.GetKeyDown(jumpKey) && (!isGrounded) && (!isWalled) && jumpCounter == 2)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            jumpCounter = 1f;
        }
    }

    void ControlSpeed()
    {
        if (Input.GetKey(sprintKey) && Input.GetKey(KeyCode.W) && (isGrounded))
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fovSprint, sprintFovTime * Time.deltaTime);
        }
        else if (Input.GetKey(sprintKey) && Input.GetKey(KeyCode.W) && (isWalled))
        
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, sprintFovTime * Time.deltaTime);
        }
    }


    void ControlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
            rb.useGravity = false;
        }
        else if (!isGrounded && isWalled && Input.GetKey(sprintKey))
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
            rb.velocity += Vector3.down * gravityDownForce * Time.deltaTime;
        }
    }
}
