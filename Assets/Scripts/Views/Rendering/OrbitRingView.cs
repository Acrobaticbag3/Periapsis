using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitRingView : MonoBehaviour
{
    [SerializeField] private OrbitModel _orbit;
    [SerializeField] private int _segments = 100;
    private LineRenderer _lr;

    public OrbitModel Orbit
    {
        get => _orbit;
        set
        {
            _orbit = value;
            if (_orbit != null) DrawOrbit();    // In case runtime assignment
        }
    }

    public int Segments
    {
        get => _segments;
        set
        {
            _segments = value;
            if (_orbit != null) DrawOrbit();    // In case of segment change
        }
    }

    void Awake()
    {
        _lr = GetComponent<LineRenderer>();
        _lr.useWorldSpace = false;
    }

    void Start()
    {
        if (_orbit != null)
            DrawOrbit();
    }

    void LateUpdate()
    {
        if (_lr.enabled && _orbit != null && _orbit.CentralBody != null)
        {
            transform.position = _orbit.CentralBody.position;
        }
    }

    void DrawOrbit()
    {
        if (_orbit == null || _orbit.CentralBody == null)
            return;

        int segments = Mathf.Max(100, _segments);
        _lr.positionCount = segments + 1;   // +1 for manual closing of loop

        float step = 2f * Mathf.PI / segments;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * step;
            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * _orbit.Radius;
            _lr.SetPosition(i, pos);
        }
    }

    // === === === Visibility API in use by Input Controller === === === \\
    private void EnsureLR()
    {
        if (_lr == null)
            _lr = GetComponent<LineRenderer>();
    }
    public void ToggleVisibility()
    {
        EnsureLR();
        if (_lr == null) return;
        _lr.enabled = !_lr.enabled;
    }

    public void SetVisibility(bool visible)
    {
        EnsureLR();
        if (_lr == null) return;
        _lr.enabled = visible;
    }

    public bool IsVisable()
    {
        EnsureLR();
        return _lr != null && _lr.enabled;
    }
}
