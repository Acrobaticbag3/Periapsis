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
        set => _orbit = value;
    }

    public int Segments
    {
        get => _segments;
        set => _segments = value;
    }

    void Awake()
    {
        _lr = GetComponent<LineRenderer>();
        _lr.useWorldSpace = true;
    }

    void Update()
    {
        DrawOrbit();
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
            _lr.SetPosition(i, _orbit.CentralBody.position + pos);
        }
    }
}
