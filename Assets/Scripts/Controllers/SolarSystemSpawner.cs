using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemSpawner : MonoBehaviour
{
    [Header("Seed")]
    [SerializeField] private int _seed = 0;
    private System.Random _rng;

    [Header("Prefabs")]
    [SerializeField] private List<GameObject> _starPrefabs;
    [SerializeField] private List<PlanetPrefabEntry> _planetPrefabs;
    [SerializeField] private GameObject _asteroidPrefab;

    [Header("Orbit Line Settings")]
    [SerializeField] private Material _orbitLineMaterial;
    [SerializeField] private int _orbitSegments = 100;

    [Header("Star Settings")]
    private Transform _star;
    private StarType _starType;
    private float _starMass = 1f;

    [Header("Gas Giant Settings")]
    private float _gasGiantChance = 0.2f;

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
    private int _astroidsPerField = 50;
    private float _fieldRadius = 3f;
    private float _beltRadius = 1f;
    private float _beltInnerOffset = 0.5f;
    private double _asteroidSizeVariation = 0.3;
    private double _minimumAsteroidSize = 0.5;

    [Header("Scalers")]
    private float _timeScale = 0.5f;
    private float _gasGiantScale = 1.5f;
    private float _planetScale = 1f;
    private float _moonScale = 0.1f;
    private float _planetOrbitLineWidth = 0.05f;
    private float _moonOrbitLineWidth = 0.025f;

    [Header("Star/Planet Types")]
    private Dictionary<PlanetType, GameObject> _planetPrefabMap;
    private Dictionary<StarType, PlanetType[]> _starPlanetTable = new Dictionary<StarType, PlanetType[]>
    {
        { StarType.O, new[] { PlanetType.Volcanic, PlanetType.Toxic, PlanetType.GasGiant } },
        { StarType.B, new[] { PlanetType.Volcanic, PlanetType.Cryogenic, PlanetType.GasGiant } },
        { StarType.A, new[] { PlanetType.Arctic, PlanetType.Tundra, PlanetType.Desert, PlanetType.GasGiant } },
        { StarType.F, new[] { PlanetType.Continental, PlanetType.Savanna, PlanetType.Ocean, PlanetType.GasGiant } },
        { StarType.G, new[] { PlanetType.Continental, PlanetType.Savanna, PlanetType.Ocean, PlanetType. Desert, PlanetType.GasGiant } },
        { StarType.K, new[] { PlanetType.Savanna, PlanetType.Continental, PlanetType.Ocean, PlanetType.GasGiant } },
        { StarType.M, new[] { PlanetType.Cryogenic, PlanetType.Tundra, PlanetType.Desert, PlanetType.GasGiant } }
    };

    [Header("Planet Type Ranges")]
    private Dictionary<PlanetType, (int minIndex, int maxIndex)> _planetZones = new Dictionary<PlanetType, (int, int)>
    {
        { PlanetType.Volcanic, (0, 2) },
        { PlanetType.Toxic, (0, 3) },
        { PlanetType.Desert, (1, 4) },
        { PlanetType.Continental, (2, 5) },
        { PlanetType.Savanna, (2, 5) },
        { PlanetType.Ocean, (3, 5) },
        { PlanetType.Arctic, (3, 6) },
        { PlanetType.Tundra, (3, 6) },
        { PlanetType.Cryogenic, (4, 7) },
        { PlanetType.GasGiant, (4, 7) }
    };

    void Start()
    {
        if (_seed == 0)
            _seed = UnityEngine.Random.Range(1, 999999);

        _rng = new System.Random(_seed);
        Debug.Log("Generating System with seed: " + _seed);

        _planetPrefabMap = new Dictionary<PlanetType, GameObject>();
        foreach (var entry in _planetPrefabs)
        {
            if (!_planetPrefabMap.ContainsKey(entry.Type))
                _planetPrefabMap.Add(entry.Type, entry.Prefab);
        }

        SpawnSystem();
    }

    void SpawnSystem()
    {
        // === STAR === \\
        _starType = (StarType)_rng.Next(System.Enum.GetValues(typeof(StarType)).Length);
        GameObject starPrefab = _starPrefabs[(int)_starType];
        _star = SpawnOrbitingBody("Star", starPrefab, null, 0, 0, 3, 0f).transform;
        _starMass = _star.GetComponent<StarModel>()?.Mass ?? 1f;

        // === PLANETS === \\
        int planetCount = _rng.Next(_minPlanets, _maxPlanets + 1);
        float currentOrbit = _planetMinDist;

        for (int i = 0; i < planetCount; i++)
        {
            bool isGasGiant = false;
            var (minGasIndex, maxGasIndex) = _planetZones[PlanetType.GasGiant];
            if (minGasIndex <= i && i <= maxGasIndex)
                isGasGiant = (_rng.NextDouble() < _gasGiantChance);

            PlanetType planetType;
            GameObject prefab;

            if (isGasGiant)
            {
                planetType = PlanetType.GasGiant;
                prefab = _planetPrefabMap[PlanetType.GasGiant];
            }
            else
            {
                planetType = PickPlanetTypeForIndex(i, _starType);
                prefab = _planetPrefabMap[planetType];
            }

            string name = planetType.ToString() + " " + (i + 1);

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

            if (isGasGiant)
            {
                SpawnAsteroidBeltAroundPlanet(planet, "Ring of " + name, _beltRadius, _astroidsPerField);
            }

            // === MOONS === \\
            int moonCount = isGasGiant
                ? _rng.Next(_minMoons + 3, _maxMoons + 4)
                : _rng.Next(_minMoons, _maxMoons + 1);

            for (int m = 0; m < moonCount; m++)
            {
                float moonRadius = (float)(_rng.NextDouble() * (_moonMaxDist - _moonMinDist) + _moonMinDist);
                float moonPeriod = KeplerPeriod(moonRadius, _starMass, _timeScale); // Change to use planets mass instead
                GameObject moonPrefab = _planetPrefabMap[PlanetType.Moon];
                SpawnOrbitingBody(
                    $"{name} Moon {m + 1}",
                    moonPrefab,
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
        float beltDist = currentOrbit + _beltInnerOffset;
        SpawnAsteroidBelt("Main Asteroid Belt", beltDist);
    }

    private PlanetType PickPlanetTypeForIndex(int index, StarType starType)
    {
        PlanetType[] candidates = _starPlanetTable[starType];
        List<PlanetType> valid = new List<PlanetType>();

        foreach (var type in candidates)
        {
            if (_planetZones.ContainsKey(type))
            {
                var (minIndex, maxIndex) = _planetZones[type];
                if (minIndex <= index && index <= maxIndex)
                    valid.Add(type);
            }
        }
        if (valid.Count == 0)
            return PlanetType.Volcanic;

        return valid[_rng.Next(valid.Count)];
    }

    private float KeplerPeriod(float radius, float centralMass, float scale = 1f)
    {
        // T = 2π * sqrt(r^3 / (G*M))
        // Collapse G --> timeScale --> avoid LARGE numbers
        return scale * 2f * Mathf.PI * Mathf.Sqrt((radius * radius * radius) / centralMass);
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

        for (int i = 0; i < _astroidsPerField + 450; i++)
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

    void SpawnAsteroidBeltAroundPlanet(GameObject planet, string beltName, float radius, int asteroidCount)
    {
        GameObject belt = new GameObject(beltName);
        belt.transform.position = planet.transform.position;
        belt.transform.parent = planet.transform;

        for (int i = 0; i < asteroidCount; i++)
        {
            float angle = (float)(_rng.NextDouble() * 2 * Mathf.PI);                    // 0 --> 360 degrees
            float distance = radius + (float)(_rng.NextDouble() * _beltRadius);         // Inner --> outer ring

            Vector3 position = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * distance;
            position += planet.transform.position;
            position.z += (float)(_rng.NextDouble() * 2f - 1f); // 1f for "jitter" placement

            GameObject asteroid = Instantiate(_asteroidPrefab, position, Quaternion.identity, belt.transform);
            asteroid.transform.localScale = Vector3.one * (float)(_rng.NextDouble() * _asteroidSizeVariation + _minimumAsteroidSize);
        }
    }
}
