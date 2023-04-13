using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _timeAlive;
    [SerializeField] private int _damage;
    GameObject _playerObject;
    Health _playerHealth;
    Rigidbody _rb;
    void Start()
    {
       Invoke("DestroyObject", _timeAlive);
        _playerObject = GameObject.Find("Player");
        _playerHealth = _playerObject.GetComponent<Health>();
        _rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        _rb.velocity = -transform.up * 10;
    }
    void DestroyObject()
    {
        Destroy(gameObject);
    }
    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Player") _playerHealth.TakeDamage(_damage);
        Destroy(gameObject);
    }
}
