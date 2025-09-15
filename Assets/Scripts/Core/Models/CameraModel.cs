using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModel : MonoBehaviour
{
    // Internal
    [SerializeField] private Vector3 _position = Vector3.zero;
    [SerializeField] private float _rotation = 0f;
    [SerializeField] private float _zoom = 2f;

    // Tweakable
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _zoomSpeed = 10f;
    [SerializeField] private float _maxZoom = 100;
    [SerializeField] private float _minZoom = 0f;

    [SerializeField] private CameraMode _mode = CameraMode.Tactical;

    // Controlled access
    public Vector3 Position
    {
        get => _position;
        set => _position = value;
    }

    public float Rotation
    {
        get => _rotation;
        set => _rotation = value;
    }

    public float Zoom
    {
        get => _zoom;
        set => _zoom = Mathf.Clamp(value, _minZoom, _maxZoom);
    }

    public CameraMode Mode
    {
        get => _mode;
        set => _mode = value;
    }

    // Read only
    public float MoveSpeed => _moveSpeed;
    public float ZoomSpeed => _zoomSpeed;
    public float MaxZoom => _maxZoom;
    public float MinZoom => _minZoom;

    private void Awake()
    {
        if (_mode == CameraMode.Strategic) _zoom = 5f;
        else if (_mode == CameraMode.Tactical) _zoom = 100f;
    }
}
