using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarModel : MonoBehaviour
{
    [SerializeField] private StarType _starType;
    private float _mass = 1f;

    public StarType StarType
    {
        get => _starType;
        set => _starType = value;
    }

    public float Mass
    {
        get => _mass;
        set => _mass = value;
    }
}
