using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    float playerHeight = 2f;

    [SerializeField] Transform orientation;
    [Header("Camera")]
    [SerializeField] private Camera cam;
    [SerializeField] Transform camHolder;
    [SerializeField] private float fov;
    [SerializeField] private float fovSprint;
    [SerializeField] private float sprintFovTime;
    [SerializeField] private float wallRunFov;
    [SerializeField] private float wallRunFovTime;
    [SerializeField] private float camTilt;
    [SerializeField] private float camTiltTime;
    public float tilt { get; private set; }

    [Header("Movement")]
    public float moveSpeed = 6f;
    float movementMultiplier = 10f;
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float sprintSpeed = 8f;
    [SerializeField] float crouchSpeed = 2f;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float airMultiplier = 0.4f;

    [Header("Jumping")]
    public float jumpForce = 20f;
    public float gravityDownForce = 35f;
    public float jumpCounter = 0f;

    [Header("Wall Running")]
    [SerializeField] private float wallRunGravity;
    [SerializeField] private float wallRunJumpForce;
    bool wallLeft = false;
    bool wallRight = false;
    RaycastHit leftWallHit;
    RaycastHit rightWallHit;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Drag")]
    [SerializeField] float groundDrag = 6f;
    [SerializeField] float airDrag = 2f;

    float horizontalMovement;
    float verticalMovement;

    [Header("Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask wallMask;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] private float wallDistance = 0.6f;
    [SerializeField] private float minimumJumpHeight = 1.5f;
    bool isGrounded;
    bool isWalled;

    //Combining PlayerLook for ease of Networking.
    [Header("Looking")]
    [SerializeField] private float sensX = 100f;
    [SerializeField] private float sensY = 100f;
    float mouseX;
    float mouseY;
    float multiplier = 0.01f;
    float xRotation;
    float yRotation;
    //May remove in the future. 

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

    bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        //Combining PlayerLook for ease of Networking. 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!isLocalPlayer)
        {
            cam.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        //If not local player, don't run these methods.
        if(!isLocalPlayer) 
        { 
            return;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        isWalled = Physics.CheckSphere(wallCheck.position, wallDistance, wallMask);

        if (isGrounded)
        {
            Debug.Log("The Player is Grounded.");
        }
        else if (isWalled)
        {
            Debug.Log("The Player is touching a Wall.");
        }

        if (CanWallRun() && isWalled)
        {
            if (wallLeft && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
            {
                StartWallRun();
                Debug.Log("Wall running on the Left.");
            }
            else if (wallRight && Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
            {
                StartWallRun();
                Debug.Log("Wall running on the Right.");
            }
            else
            {
                StopWallRun();
            }
        }
        else
        {
            StopWallRun();
        }

        MyInput();
        ControlDrag();
        ControlSpeed();
        Jump();
        CheckWall();

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

        //Combining PlayerLook for ease of Networking.
        camHolder.transform.rotation = Quaternion.Euler(xRotation, yRotation, tilt);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    void CheckWall()
    {
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallDistance);
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallDistance);
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;

        //Combining PlayerLook for ease of Networking.
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");
        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
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
        else if (Input.GetKey(crouchKey) && (isGrounded))
        {
            moveSpeed = Mathf.Lerp(moveSpeed, crouchSpeed, acceleration);
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

    void StartWallRun()
    {
        rb.useGravity = false;
        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, wallRunFov, wallRunFovTime * Time.deltaTime);

        if (wallLeft)
        {
            tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime);
        }
        else if (wallRight)
        {
            tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (wallLeft)
            {
                Vector3 wallRunJumpDirection = transform.up + leftWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
            }
            else if (wallRight)
            {
                Vector3 wallRunJumpDirection = transform.up + rightWallHit.normal;
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(wallRunJumpDirection * wallRunJumpForce * 100, ForceMode.Force);
            }
        }
    }

    void StopWallRun()
    {
        rb.useGravity = true;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, wallRunFovTime * Time.deltaTime);
        tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);
    }
}
