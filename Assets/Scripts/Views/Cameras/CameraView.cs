using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    [SerializeField] private CameraModel cameraModel;
    [SerializeField] private Transform cameraTransform;
    private Camera cam;

    private void Awake() => cam = cameraTransform.GetComponent<Camera>();

    private void LateUpdate()
    {
        switch (cameraModel.Mode)
        {
            case CameraMode.Tactical:
                ApplyTacticalView();
                break;
            case CameraMode.Strategic:
                ApplyStrategicView();
                break;
        }
    }

    private void ApplyTacticalView()
    {
        cam.orthographic = true;

        Vector3 targetPos = new Vector3(cameraModel.Position.x, cameraModel.Position.y, -10f);
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPos, Time.deltaTime * 5f);

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            cameraModel.Zoom * (cameraModel.Mode == CameraMode.Tactical ? 2f : 1f),
            Time.deltaTime * 5f // smooth speed multiplier
        );

        cameraTransform.rotation = Quaternion.Euler(0f, 0f, cameraModel.Rotation);
    }

    private void ApplyStrategicView()
    {
        cam.orthographic = true; // or false for perspective

        Vector3 targetPos = new Vector3(cameraModel.Position.x, cameraModel.Position.y, -5f);
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPos, Time.deltaTime * 5f);

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            cameraModel.Zoom * (cameraModel.Mode == CameraMode.Tactical ? 2f : 1f),
            Time.deltaTime * 5f // smooth speed multiplier
        );
        
        cameraTransform.rotation = Quaternion.Euler(0f, 0f, cameraModel.Rotation);
    }
}