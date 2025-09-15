using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
    [SerializeField] private PlanetOrbitController[] _planets;

    void Update()
    {
        foreach (var planet in _planets)
        {
            planet?.Update();
        }
    }
}
