using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Orbit data model (radius, period, central body)
public class OrbitModel
{
    [SerializeField] private Transform _centralBody; // The star
    [SerializeField] private float _radius = 10f;    // Orbit radius
    [SerializeField] private float _period = 10f;    // Time to complete one orbit in seconds

    public OrbitModel(Transform centralBody, float radius, float period)
    {
        _centralBody = centralBody;
        _radius = radius;
        _period = period;
    }

    public Transform CentralBody
    {
        get => _centralBody;
        set => _centralBody = value;
    }

    public float Radius
    {
        get => _radius;
        set => _radius = value;
    }

    public float Period
    {
        get => _period;
        set => _period = value;
    }
}
