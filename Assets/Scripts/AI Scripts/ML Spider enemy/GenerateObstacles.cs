using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class GenerateObstacles : MonoBehaviour
{
    public GameObject[] _buildings;
    public float _spawnRadius;
    Vector3 _startingPos;

    void OnEnable()
    {
        _startingPos = transform.position;
        foreach(GameObject _building in _buildings)
        {
            var _newTargetPos = _startingPos + (Random.insideUnitSphere * _spawnRadius);
            _newTargetPos.y = _startingPos.y;
            transform.position = _newTargetPos;
            Instantiate(_building);
        }
    }

    
}
