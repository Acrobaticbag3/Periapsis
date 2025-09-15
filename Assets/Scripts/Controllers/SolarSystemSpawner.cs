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
    [SerializeField] private int _asteroidCount = 200;
    [SerializeField] private float _beltInnerRadius = 35f;
    [SerializeField] private float _beltOuterRadius = 50f;

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
        GameObject belt = new GameObject("Asteroid Belt");
        for (int i = 0; i < _asteroidCount; i++)
        {
            float r = Random.Range(_beltInnerRadius, _beltOuterRadius);
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * r + _star.position;
            GameObject asteroid = Instantiate(_asteroidPrefab, pos, Quaternion.identity, belt.transform);
            asteroid.transform.localScale = Vector3.one * Random.Range(0.2f, 0.5f);
        }
    }
}
