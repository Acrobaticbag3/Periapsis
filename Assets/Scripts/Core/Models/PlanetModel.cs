using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetModel : MonoBehaviour
{
    [SerializeField] private PlanetType _planetType;
    [SerializeField] private List<UpkeepModifier> _upkeepModifiers;
    private float _size = 1f;
    private Color _color = Color.white;
    private string _planetName;
    private OrbitModel _orbit;

    public PlanetType PlanetType
    {
        get => _planetType;
        set => _planetType = value;
    }

    public string PlanetName
    {
        get => _planetName;
        set => _planetName = value;
    }

    public float Size
    {
        get => _size;
        set => _size = value;
    }

    public Color Color
    {
        get => _color;
        set => _color = value;
    }

    public OrbitModel Orbit
    {
        get => _orbit;
        set => _orbit = value;
    }

    public float GetUpkeepForSpecies(Species species)
    {
        foreach (var modifier in _upkeepModifiers)
        {
            if (modifier.Species == species)
                return modifier.UpkeepMultiplier;
        }
        return 1f;
    }
}
