using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject _target, _projectile;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _timeBetweenShots;
    [SerializeField] private int _range;
    float _baseTime;
    void Start(){_baseTime = _timeBetweenShots;}
    void Update()
    {
        Vector3 _thisPos = this.transform.position;
        Vector3 _targetPos = _target.transform.position;
        if (Vector3.Distance(_thisPos, _targetPos) < _range)
        {
            _timeBetweenShots-= Time.deltaTime;
            if(_timeBetweenShots <= 0)Shoot();
        }
    }

    void Shoot()
    {
        GameObject _projectilePrefab = Instantiate(_projectile, _firePoint.position, _firePoint.rotation);
        _timeBetweenShots = _baseTime;
    }
}