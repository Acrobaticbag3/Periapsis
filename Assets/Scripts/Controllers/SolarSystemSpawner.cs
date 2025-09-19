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

    [Header("Gas Giant")]
    private float _gasGiantChance = 0.2f;
    private int _minGasGiantIndex = 3;

    [Header("Planet Settings")]
    private int _minPlanets = 3;
    private int _maxPlanets = 7;
    private float _planetMinDist = 10f;
    private float _planetSpacing = 8f;

    [Header("Moon Settings")]
    private int _minMoons = 0;
    private int _maxMoons = 3;
    private float _moonMinDist = 1.5f;
    private float _moonMaxDist = 4f;

    [Header("Asteroid Belt Settings")]
    private int _minBelts = 0;
    private int _maxBelts = 3;
    private int _astroidsPerField = 50;
    private float _fieldRadius = 5f;
    private float _beltInnerOffset = 0.5f;
    private float _beltWidth = 2f;
    private double _asteroidSizeVariation = 0.3;
    private double _minimumAsteroidSize = 0.2;

    [Header("Mass Settings")]
    private float _starMass = 1f;
    private float _planetMass = 0.3f;

    [Header("Scalers")]
    private float _timeScale = 0.5f;
    private float _gasGiantScale = 2.5f;
    private float _planetScale = 1f;
    private float _moonScale = 0.5f;
    private float _planetOrbitLineWidth = 0.05f;
    private float _moonOrbitLineWidth = 0.025f;

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
            bool isGasGiant = (_rng.NextDouble() < _gasGiantChance && i > _minGasGiantIndex);
            GameObject prefab = isGasGiant ? _gasGiantPrefab : _planetPrefab;
            string name = (isGasGiant ? "Gas Giant" : "Planet") + (i + 1);

            float radius = currentOrbit;
            float period = KeplerPeriod(radius, _starMass, _timeScale);

            GameObject planet = SpawnOrbitingBody(
                name,
                prefab,
                _star,
                radius,
                period,
                isGasGiant ? _gasGiantScale : _planetScale,
                _planetOrbitLineWidth
            );

            // === MOONS === \\
            int moonCount = isGasGiant
                ? _rng.Next(_minMoons + 3, _maxMoons + 4)
                : _rng.Next(_minMoons, _maxMoons + 1);

            for (int m = 0; m < moonCount; m++)
            {
                float moonRadius = (float)(_rng.NextDouble() * (_moonMaxDist - _moonMinDist) + _moonMinDist);
                float moonPeriod = KeplerPeriod(moonRadius, _planetMass, _timeScale);
                SpawnOrbitingBody(
                    $"{name} Moon {m + 1}",
                    _moonPrefab,
                    planet.transform,
                    moonRadius,
                    moonPeriod,
                    _moonScale,
                    _moonOrbitLineWidth
                );
            }

            currentOrbit += _planetSpacing + (float)_rng.NextDouble() * 5f; // Arbitrary int for "jitter" placement
        }

        // === ASTEROID BELTS === \\
        int beltCount = _rng.Next(_minBelts, _maxBelts + 1);
        for (int b = 0; b < beltCount; b++)
        {
            float beltMinRadius = currentOrbit + _beltInnerOffset;
            float beltMaxRadius = beltMinRadius + _beltWidth;
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
            position.z += (float)(_rng.NextDouble() * 2f - 1f); // 1f for "jitter" placement

            GameObject asteroid = Instantiate(_asteroidPrefab, position, Quaternion.identity, belt.transform);
            asteroid.transform.localScale = Vector3.one * (float)(_rng.NextDouble() * _asteroidSizeVariation + _minimumAsteroidSize);
        }
    }

    private float KeplerPeriod(float radius, float centralMass, float scale = 1f)
    {
        // T = 2π * sqrt(r^3 / (G*M))
        // Collapse G --> timeScale --> avoid LARGE numbers
        return scale *2f * Mathf.PI * Mathf.Sqrt((radius * radius * radius) / centralMass);
    }
}
