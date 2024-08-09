using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header ("Controller options")]
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float cameraFov = 60;
    [Header ("Components")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private Camera playerCamera;
    private float verticalSpeed = 0.0f;
    private Vector3 moveDirection = Vector3.zero;
    private Transform cameraTransform;

    private float xRotation = 0f;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Move();
        MouseLook();
        CameraOption();
    }

    private void Move()
    {
        if (playerController.isGrounded)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            moveDirection = transform.right * moveHorizontal+ transform.forward * moveVertical;
            moveDirection *= speed;

            if (Input.GetButtonDown("Jump"))
            {
                verticalSpeed = jumpForce;
            }
        }

        verticalSpeed -= gravity * Time.deltaTime;
        Vector3 movement = moveDirection + Vector3.up * verticalSpeed;
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

    private void CameraOption()
    {
        playerCamera.fieldOfView = cameraFov;
    }
}
