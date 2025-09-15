using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Internal
    [SerializeField] private CameraModel cameraModel;
    [SerializeField] private float rotationSpeed = 50f; // Since input response, does not belong in model, according to MVC

    private void Update()
    {
        HandleModeSwitch();
        HandleMovement();
        HandleZoom();
        HandleRotation();
    }

    private void HandleModeSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            cameraModel.Mode = cameraModel.Mode == CameraMode.Tactical
                ? CameraMode.Strategic
                : CameraMode.Tactical;

            cameraModel.Zoom = cameraModel.Mode == CameraMode.Strategic ? 5f : 100f;
        }

    }

    private void HandleMovement()
    {
        // WASD
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 input = new Vector3(horizontal, vertical, 0f);
        if (input != Vector3.zero)
        {
            // Rotate move input to allow for local movement
            float angleRad = cameraModel.Rotation * Mathf.Deg2Rad;
            float cos = Mathf.Cos(angleRad);
            float sin = Mathf.Sin(angleRad);

            // Apply 2D rotation to input vector
            Vector3 rotatedInput = new Vector3(
                input.x * cos - input.y * sin,
                input.x * sin + input.y * cos,
                0f
            );

            float speed = cameraModel.MoveSpeed;
            if (Input.GetKey(KeyCode.LeftShift)) speed *= 3f;
            cameraModel.Position += rotatedInput * speed * Time.deltaTime;
        }
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            cameraModel.Zoom -= scroll * cameraModel.ZoomSpeed; // Model clamps
        }
    }

    private void HandleRotation()
    {
        float rotInput = 0f;
        if (Input.GetKey(KeyCode.Q)) rotInput = -2f;
        if (Input.GetKey(KeyCode.E)) rotInput = 2f;

        if (rotInput != 0)
        {
            float newRot = cameraModel.Rotation + rotInput * rotationSpeed * Time.deltaTime;
            cameraModel.Rotation = newRot;
        }
    }
}