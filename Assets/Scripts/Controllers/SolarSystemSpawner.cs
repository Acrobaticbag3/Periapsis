using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _starPrefab;
    [SerializeField] private GameObject _planetPrefab;
    [SerializeField] private GameObject _gasGiantPrefab;
    [SerializeField]private GameObject _asteroidPrefab;

    [Header("Orbit Line Settings")]
    [SerializeField] private Material _orbitLineMaterial;
    [SerializeField] private int _orbitSegments = 100;

    [Header("Asteroid Belt Settings")]
    [SerializeField] private int _numberOfFields = 5;
    [SerializeField] private int _astroidsPerField = 50;
    [SerializeField] private float _fieldRadius = 5f;
    [SerializeField] private float _minDistanceFromStar = 15f;
    [SerializeField] private float _maxDistanceFromStar = 60f;

    private Transform _star;

    void Start()
    {
        SpawnSystem();
    }

    void SpawnSystem()
    {
        // === STAR ===
        _star = Instantiate(_starPrefab, Vector3.zero, Quaternion.identity).transform;
        _star.name = "Star";

        // === PLANETS ===
        SpawnPlanet("Planet A", 10f, 8f, Color.cyan, _planetPrefab);
        SpawnPlanet("Planet B", 18f, 12f, Color.green, _planetPrefab);
        SpawnPlanet("Planet C", 25f, 18f, Color.gray, _planetPrefab);

        // === GAS GIANT ===
        SpawnPlanet("Gas Giant", 32f, 30f, Color.red, _gasGiantPrefab);

        // ===  ASTEROID BELT ===
        SpawnAsteroidBelt();
    }

    void SpawnPlanet(string name, float orbitRadius, float orbitPeriod, Color color, GameObject prefab)
    {
        // Planet instance
        GameObject planet = Instantiate(prefab, _star.position + Vector3.right * orbitRadius, Quaternion.identity);
        planet.name = name;
        planet.transform.localScale = Vector3.one * (prefab == _gasGiantPrefab ? 2.5f : 1f);

        // Orbit data
        OrbitModel orbit = new OrbitModel(_star, orbitRadius, orbitPeriod);

        // Movement
        PlanetModel orbiting = planet.AddComponent<PlanetModel>();
        orbiting.Orbit = orbit;

        // Visual orbit ring
        GameObject orbitLine = new GameObject(name + " Orbit");
        LineRenderer lr = orbitLine.AddComponent<LineRenderer>();
        OrbitRingView ring = orbitLine.AddComponent<OrbitRingView>();
        ring.Orbit = orbit;
        ring.Segments = _orbitSegments;

        // Line renderer setup
        lr.material = _orbitLineMaterial;
        lr.widthMultiplier = 0.05f;
        lr.loop = true;
    }

    void SpawnAsteroidBelt()
    {
        GameObject asteroidFieldsParent = new GameObject("Asteroid Belt");
        for (int f = 0; f < _numberOfFields; f++)
        {
            // Random center for field
            float distance = Random.Range(_minDistanceFromStar, _maxDistanceFromStar);
            float angle = Random.Range(0, 360f) * Mathf.Deg2Rad;
            Vector3 fieldCenter = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * distance + _star.position;

            GameObject field = new GameObject("Asteroid Field " + (f + 1));
            field.transform.parent = asteroidFieldsParent.transform;
            field.transform.position = fieldCenter;

            // Spawn stuffs
            for (int i = 0; i < _astroidsPerField; i++)
            {
                Vector2 offset = Random.insideUnitCircle * _fieldRadius;
                Vector3 pos = fieldCenter + new Vector3(offset.x, offset.y, 0f);

                GameObject asteroid = Instantiate(_asteroidPrefab, pos, Quaternion.identity, field.transform);
                asteroid.transform.localScale = Vector3.one * Random.Range(0.2f, 0.5f);
            }
        }
    }
}
