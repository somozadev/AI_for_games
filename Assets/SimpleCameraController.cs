using System;
using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float boostMultiplier = 2f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 3f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float zoomMinDistance = 1f;
    [SerializeField] private float zoomMaxDistance = 100f;

    private Vector3 moveDirection;
    private Vector3 lastMousePosition;

    private void OnEnable()
    {
        Cursor.visible = false;
    }
    private void OnDisable()
    {
        Cursor.visible = true;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMovement()
    {
        float boost = Input.GetKey(KeyCode.LeftShift) ? boostMultiplier : 1f;

        float moveX = Input.GetAxis("Horizontal"); 
        float moveZ = Input.GetAxis("Vertical");   
        float moveY = 0f;

        if (Input.GetKey(KeyCode.Q)) moveY -= 1f;
        if (Input.GetKey(KeyCode.E)) moveY += 1f;

        moveDirection = new Vector3(moveX, moveY, moveZ).normalized;

        transform.Translate(moveDirection * moveSpeed * boost * Time.deltaTime, Space.Self);
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButton(1))
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

            float yaw = mouseDelta.x * rotationSpeed * Time.deltaTime; 
            float pitch = -mouseDelta.y * rotationSpeed * Time.deltaTime; 

            transform.Rotate(Vector3.up, yaw, Space.World);
            transform.Rotate(Vector3.right, pitch, Space.Self);
        }

        lastMousePosition = Input.mousePosition;
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float zoomAmount = scroll * zoomSpeed * Time.deltaTime;

        Vector3 forward = transform.forward;
        Vector3 newPosition = transform.position + forward * zoomAmount;

        float distance = Vector3.Distance(newPosition, transform.position);
        if (distance > zoomMinDistance && distance < zoomMaxDistance)
        {
            transform.position = newPosition;
        }
    }
}
