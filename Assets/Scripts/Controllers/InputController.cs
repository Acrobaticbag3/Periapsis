using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private float strategicAlpha = 1f;
    private float tacticalAlpha = 0.1f;

    private CameraMode _mode = CameraMode.Tactical; // Default

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ToggleCameraMode();
    }

    private void ToggleCameraMode()
    {
        _mode = (_mode == CameraMode.Tactical) ? CameraMode.Strategic : CameraMode.Tactical;
        UpdateOrbitAlphas();
    }

    private void UpdateOrbitAlphas()
    {
        float alpha = (_mode == CameraMode.Strategic) ? strategicAlpha : tacticalAlpha;
        foreach (var ring in FindObjectsOfType<OrbitRingView>())
            ring.SetAlpha(alpha);
    }
}
