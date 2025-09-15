using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroidBeltModel : MonoBehaviour
{
    [SerializeField] private Transform _centralBody;
    [SerializeField] private float _innerRadius = 20f;
    [SerializeField] private float _outerRadius = 30f;
    [SerializeField] private int _asteroidCount = 50;
    [SerializeField] private GameObject _asteroidPrefab;

    void Start()
    {
        for (int i = 0; i < _asteroidCount; i++)
        {
            float r = Random.Range(_innerRadius, _outerRadius);
            float angle = Random.Range(0f, 360f);
            float rad = angle * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * r + _centralBody.position;
            Instantiate(_asteroidPrefab, pos, Quaternion.identity, transform);
        }
    }
}
