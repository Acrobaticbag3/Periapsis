using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private OrbitRingView[] _orbitRings;

    void Awake()
    {
        _orbitRings = FindObjectsOfType<OrbitRingView>();
    }

    void Update()
    {
        if (_orbitRings == null) return;
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            foreach (var ring in _orbitRings)
                ring.ToggleVisibility();
        }
    }
}
