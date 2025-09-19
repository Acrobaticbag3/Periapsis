using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    [Header("Stuffs")]
    [SerializeField] private CameraModel cameraModel;
    [SerializeField] private Transform cameraTransform;
    private Camera cam;

    [Header("Zoom scale settings")]
    [SerializeField] private float _NORMALSCALE = 0.3f;
    [SerializeField] private float _LARGESCALE = 1.8f;
    [SerializeField] private float _baseLayer = -5f;

    private void Awake() => cam = cameraTransform.GetComponent<Camera>();

    private void LateUpdate()
    {
        switch (cameraModel.Mode)
        {
            case CameraMode.Tactical:
                ApplyView();
                break;
            case CameraMode.Strategic:
                ApplyView();
                break;
        }
    }

    private void ApplyView()
    {
        cam.orthographic = true; // or false for perspective

        Vector3 targetPos = new Vector3(cameraModel.Position.x, cameraModel.Position.y, _baseLayer);
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPos, Time.deltaTime * 5f);

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            cameraModel.Zoom * (cameraModel.Mode == CameraMode.Tactical ? _LARGESCALE : _NORMALSCALE),
            Time.deltaTime * 5f // smooth speed multiplier
        );
        
        cameraTransform.rotation = Quaternion.Euler(0f, 0f, cameraModel.Rotation);
    }
}