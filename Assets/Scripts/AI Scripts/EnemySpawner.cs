using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _theEnemy;
    [SerializeField] private float _timeBetweenSpawn;
    float _timer;

    void Awake() 
    {
        _timer = _timeBetweenSpawn;    
    }

    void Update()
    {
        _timer -= Time.deltaTime;
        if(_timer <= 0)
        {
            SpawnEnemy();
            _timer = _timeBetweenSpawn;
        }
    }

    void SpawnEnemy()
    {
        Instantiate(_theEnemy, this.transform.position, this.transform.rotation);
    }
}
