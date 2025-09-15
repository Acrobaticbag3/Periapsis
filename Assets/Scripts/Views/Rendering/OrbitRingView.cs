using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitRingView : MonoBehaviour
{
    [SerializeField] private OrbitModel _orbit;
    [SerializeField] private int _segments = 100;
    [SerializeField] private LineRenderer _lr;

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

    void Start()
    {
        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = _segments + 1;
        DrawOrbit();
    }

    void DrawOrbit()
    {
        if (_orbit == null || _orbit.CentralBody == null)
            return;

        float angle = 0f;
        for (int i = 0; i <= _segments; i++)
        {
            float rad = Mathf.Deg2Rad * angle;
            Vector3 pos = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad) * _orbit.Radius);
            _lr.SetPosition(i, _orbit.CentralBody.position + pos);
            angle += 360f / _segments;
        }
    }
}
