using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpkeepModifier
{
    private Species _species;
    [SerializeField, Range(0.1f, 3f)] private float _upkeepMultiplier = 1f;

    public Species Species
    {
        get => _species;
        set => _species = value;
    }

    public float UpkeepMultiplier { get => _upkeepMultiplier; }
}
