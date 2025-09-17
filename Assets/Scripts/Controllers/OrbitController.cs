using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
    [SerializeField] private OrbitModel _orbit;
    [SerializeField] private float _angle = 0f;
    [SerializeField] private float _orbitTimeScale = 10f;    // Larger values = slower orbits

    private void Awake()
    {
        if (_orbit == null)
            _orbit = GetComponent<OrbitModel>();
    }

    private void Start()
    {
        _orbit = GetComponent<OrbitModel>();
        _angle = Random.Range(0f, 360f);
    }

    private void Update()
    {
        if (_orbit == null || _orbit.CentralBody == null)
            return;

        // Actually orbit
        _angle += (360f / _orbit.Period) * Time.deltaTime / _orbitTimeScale;

        // Orbital position
        float rad = _angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * _orbit.Radius;
        transform.position = _orbit.CentralBody.position + offset;
    }
}
