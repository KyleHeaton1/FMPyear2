using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Health : MonoBehaviour
{
    [Header ("Health Properties")]
    [SerializeField] private int _startingHealth;
    [SerializeField] private int  _currentHealth;
    [SerializeField] private ValueSlider _healthBar;
    [Space(10)]

    [Header ("Death Properties")]
    [SerializeField] private float _deathDelay;
    [SerializeField] private GameObject _specficObjectToDestroy;
    [SerializeField] private bool _autoDeleteOnDeath;
    [HideInInspector] public bool _dead = false;
    [HideInInspector] public bool _heathZero = false;
    [Space(10)]

    [SerializeField] private bool _destructible;

    [ConditionalHide("_destructible", true)]
    [Header ("Destruction Properties")]

    [SerializeField] private GameObject _originalObject;
    [ConditionalHide("_destructible", true)]
    [SerializeField] private GameObject[] _destructParts;
    [ConditionalHide("_destructible", true)]
    [SerializeField] private float _minExploForce, _maxExploForce, _exploForceRadius;
    
    void Start()
    {
        if(_healthBar != null)_healthBar.SetMaxValue(_startingHealth);
        _currentHealth = _startingHealth;
    }
    void Update()
    {
        if(_healthBar != null) _healthBar.SetValue(_currentHealth);
        if(_currentHealth <= 0)
        {
            _heathZero = true;
            Explosion();
            Invoke("ActivateDeath", _deathDelay);
        }
    }

    public void TakeDamage(int _damage){_currentHealth -= _damage;}

    public void AddHealth(int _heal){_currentHealth += _heal;}

    public void ActivateDeath()
    {
        _dead = true;
        if(_specficObjectToDestroy != null) Destroy(_specficObjectToDestroy);
        if(!_autoDeleteOnDeath)return;
        else Destroy(gameObject);
    }

    public void Explosion()
    {
        if(!_destructible)return;
        else
        {
            _originalObject.SetActive(false);
            foreach (GameObject _piece in _destructParts)
            {
                _piece.SetActive(true);
                Rigidbody _rb = _piece.GetComponent<Rigidbody>();
                if(_rb != null) _rb.AddExplosionForce(Random.Range(_minExploForce, _maxExploForce), _originalObject.transform.position, _exploForceRadius);
            }
        }
    }
}