using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemSpawner : MonoBehaviour
{
    [Header("Seed")]
    [SerializeField] private int _seed = 0;
    private System.Random _rng;

    [Header("Prefabs")]
    [SerializeField] private GameObject _starPrefab;
    [SerializeField] private GameObject _planetPrefab;
    [SerializeField] private GameObject _gasGiantPrefab;
    [SerializeField] private GameObject _asteroidPrefab;
    [SerializeField] private GameObject _moonPrefab;

    [Header("Orbit Line Settings")]
    [SerializeField] private Material _orbitLineMaterial;
    [SerializeField] private int _orbitSegments = 100;

    [Header("Planet Settings")]
    [SerializeField] private int _minPlanets = 3;
    [SerializeField] private int _maxPlanets = 7;
    [SerializeField] private float _planetMinDist = 10f;
    [SerializeField] private float _planetSpacing = 8f;

    [Header("Moon Settings")]
    [SerializeField] private int _minMoons = 0;
    [SerializeField] private int _maxMoons = 3;
    [SerializeField] private float _moonMinDist = 1.5f;
    [SerializeField] private float _moonMaxDist = 4f;
    [SerializeField] private float _moonMinPeriod = 2;
    [SerializeField] private float _moonMaxPeriod = 6f;

    [Header("Asteroid Belt Settings")]
    [SerializeField] private int _minBelts = 0;
    [SerializeField] private int _maxBelts = 3;
    [SerializeField] private int _astroidsPerField = 50;
    [SerializeField] private float _fieldRadius = 5f;
    [SerializeField] private double _asteroidSizeVariation = 0.3;
    [SerializeField] private double _minimumAsteroidSize = 0.2;

    private Transform _star;

    void Start()
    {
        if (_seed == 0)
            _seed = UnityEngine.Random.Range(1, 999999);

        _rng = new System.Random(_seed);
        Debug.Log("Generating System with seed: " + _seed);
        SpawnSystem();
    }

    void SpawnSystem()
    {
        // === STAR === \\
        _star = SpawnOrbitingBody("Star", _starPrefab, null, 0, 0, 3, 0f).transform;

        // === PLANETS === \\
        int planetCount = _rng.Next(_minPlanets, _maxPlanets + 1);
        float currentOrbit = _planetMinDist;

        for (int i = 0; i < planetCount; i++)
        {
            bool isGasGiant = (_rng.NextDouble() < 0.2 && i > 2); // ~20%, can't be close to star either
            GameObject prefab = isGasGiant ? _gasGiantPrefab : _planetPrefab;
            // === NOTE Temp name fix === \\
            string name = (isGasGiant ? "Gas Giant" : "Planet") + (i + 1);

            float radius = currentOrbit;
            float period = radius * 5f;     //NOTE  proportional period, very simple, probably expand later

            GameObject planet = SpawnOrbitingBody(name, prefab, _star, radius, period, isGasGiant ? 2.5f : 1f, 0.05f);

            // === MOONS === \\
            int moonCount = isGasGiant ? _rng.Next(_minMoons + 3, _maxMoons + 4) : _rng.Next(_minMoons, _maxMoons + 1);
            for (int m = 0; m < moonCount; m++)
            {
                float moonRadius = (float)(_rng.NextDouble() * (_moonMaxDist - _moonMinDist) + _moonMinDist);
                float moonPeriod = (float)(_rng.NextDouble() * (_moonMaxPeriod - _moonMinPeriod) + _moonMinPeriod);
                SpawnOrbitingBody($"{name} Moon {m + 1}", _moonPrefab, planet.transform, moonRadius, moonPeriod, 0.5f, 0.025f);
            }

            currentOrbit += _planetSpacing + (float)_rng.NextDouble() * 5f;
        }

        // === ASTEROID BELTS === \\
        int beltCount = _rng.Next(_minBelts, _maxBelts + 1);
        for (int b = 0; b < beltCount; b++)
        {
            float beltMinRadius = currentOrbit + 5f;
            float beltMaxRadius = beltMinRadius + 20f;
            float beltDist = (float)(_rng.NextDouble() * (beltMaxRadius - beltMinRadius) + beltMinRadius);
            SpawnAsteroidBelt("Asteroid Belt " + (b + 1), beltDist);
        }
    }

    GameObject SpawnOrbitingBody(string name, GameObject prefab, Transform parentBody, float orbitRadius, float orbitPeriod, float scale, float lineWidth)
    {
        Vector3 pos = parentBody == null ? Vector3.zero : parentBody.position + Vector3.right * orbitRadius;
        GameObject body = Instantiate(prefab, pos, Quaternion.identity);
        body.name = name;
        body.transform.localScale = Vector3.one * scale;

        OrbitModel orbit = body.AddComponent<OrbitModel>();
        orbit.CentralBody = parentBody;
        orbit.Radius = orbitRadius;
        orbit.Period = orbitPeriod;

        if (parentBody != null)
            body.AddComponent<OrbitController>();

        if (orbitRadius > 0)
        {
            GameObject orbitLine = new GameObject(name + " Orbit");
            LineRenderer lr = orbitLine.AddComponent<LineRenderer>();
            OrbitRingView ring = orbitLine.AddComponent<OrbitRingView>();
            ring.Orbit = orbit;
            ring.Segments = _orbitSegments;

            lr.material = _orbitLineMaterial;
            lr.widthMultiplier = lineWidth;
            lr.loop = true;
        }

        return body;
    }

    void SpawnAsteroidBelt(string name, float innerRadius)
    {
        GameObject belt = new GameObject(name);
        belt.transform.position = _star.position;

        for (int i = 0; i < _astroidsPerField; i++)
        {
            float angle = (float)(_rng.NextDouble() * 2 * Mathf.PI);                    // 0 --> 360 degrees
            float distance = innerRadius + (float)(_rng.NextDouble() * _fieldRadius);   // Inner --> outer ring

            // Cartesian model, I think???
            Vector3 position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * distance;
            position += _star.position;
            position.z += (float)(_rng.NextDouble() * 2f - 1f);

            GameObject asteroid = Instantiate(_asteroidPrefab, position, Quaternion.identity, belt.transform);
            asteroid.transform.localScale = Vector3.one * (float)(_rng.NextDouble() * _asteroidSizeVariation + _minimumAsteroidSize);
        }
    }
}
