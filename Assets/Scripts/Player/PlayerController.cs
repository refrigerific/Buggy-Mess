using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Controller Options")]
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float sprintMultiplier = 1.5f; // Sprint speed multiplier
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float cameraFov = 60.0f;
    [SerializeField] private float sprintFov = 70.0f; // FOV during sprint
    [SerializeField] private float acceleration = 10.0f; // Controls acceleration and deceleration
    [SerializeField] private float airControlFactor = 0.5f; // Allows control while in the air
    [SerializeField] private float coyoteTime = 0.2f; // Allow some leniency for jumps
    [SerializeField] private bool enableHeadBobbing = true; // Toggle head bobbing
    [SerializeField] private float headBobFrequency = 2.0f;
    [SerializeField] private float headBobAmplitude = 0.1f;
    [SerializeField] private float sprintHeadBobFrequency = 4.0f; // Increased frequency when sprinting
    [SerializeField] private float sprintHeadBobAmplitude = 0.2f; // Increased amplitude when sprinting

    [Header("Components")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private Camera playerCamera;

    private float verticalSpeed = 0.0f;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 currentVelocity = Vector3.zero;
    private float xRotation = 0f;
    private Transform cameraTransform;
    private float lastGroundedTime = 0f;
    private float headBobTimer = 0f;

    private bool isSprinting = false;
    private bool isMoving = false;

    private void Start()
    {
        cameraTransform = playerCamera.transform;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovementInput();
        MouseLook();
        Move();
        CameraOptions();
        if (enableHeadBobbing)
        {
            HeadBob();
        }
    }

    private void HandleMovementInput()
    {
        // Determine if the player is moving
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        isMoving = moveHorizontal != 0 || moveVertical != 0;

        // Check if sprint key is pressed and the player is moving
        isSprinting = isMoving && Input.GetKey(KeyCode.LeftShift);
    }

    private void Move()
    {
        if (playerController.isGrounded)
        {
            lastGroundedTime = Time.time; // Update grounded time for coyote jump
            verticalSpeed = -gravity * Time.deltaTime; // Stick to ground slightly
        }
        else
        {
            verticalSpeed -= gravity * Time.deltaTime;
        }

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calculate the desired move direction based on input
        Vector3 targetMoveDirection = transform.right * moveHorizontal + transform.forward * moveVertical;

        // Apply sprint multiplier if sprinting
        float currentSpeed = isSprinting ? speed * sprintMultiplier : speed;
        targetMoveDirection *= currentSpeed;

        // Apply acceleration and air control
        if (playerController.isGrounded)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, targetMoveDirection, acceleration * Time.deltaTime);
        }
        else
        {
            currentVelocity = Vector3.Lerp(currentVelocity, targetMoveDirection, airControlFactor * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && (playerController.isGrounded || Time.time - lastGroundedTime <= coyoteTime))
        {
            verticalSpeed = jumpForce;
        }

        Vector3 movement = currentVelocity + Vector3.up * verticalSpeed;
        playerController.Move(movement * Time.deltaTime);
    }

    private void MouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void CameraOptions()
    {
        // Adjust FOV for sprinting
        float targetFov = isSprinting ? sprintFov : cameraFov;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFov, Time.deltaTime * 5f);
    }

    private void HeadBob()
    {
        // Adjust head bobbing frequency and amplitude based on sprinting
        float frequency = isSprinting ? sprintHeadBobFrequency : headBobFrequency;
        float amplitude = isSprinting ? sprintHeadBobAmplitude : headBobAmplitude;

        // Check if the player is grounded and moving
        if (playerController.isGrounded && playerController.velocity.magnitude > 0.1f)
        {
            headBobTimer += Time.deltaTime * frequency;
            float headBobOffset = Mathf.Sin(headBobTimer) * amplitude;
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, new Vector3(0, 1 + headBobOffset, 0), Time.deltaTime * 10f);
        }
        else
        {
            headBobTimer = 0f;
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, new Vector3(0, 1, 0), Time.deltaTime * 10f);
        }
    }
}
