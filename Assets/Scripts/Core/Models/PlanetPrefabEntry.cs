using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetPrefabEntry
{
    [SerializeField] private PlanetType _type;
    [SerializeField] private GameObject _prefab;

    public PlanetType Type
    {
        get => _type;
        set => _type = value;
    }

    public GameObject Prefab
    {
        get => _prefab;
        set => _prefab = value;
    }
}
