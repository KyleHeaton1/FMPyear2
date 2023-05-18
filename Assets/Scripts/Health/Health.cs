using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Health : MonoBehaviour
{
    [Header ("Health Properties")]
    [SerializeField] private int _startingHealth;
    [SerializeField] public int  _currentHealth;
    [SerializeField] private ValueSlider _healthBar;

    public AudioController _audio;
    [Space(10)]

    [Header ("Death Properties")]
    [SerializeField] private float _deathDelay;
    [SerializeField] private GameObject _specficObjectToDestroy;
    [SerializeField] private bool _autoDeleteOnDeath;
    [SerializeField] private bool _removeColliderOnDeath;
    [HideInInspector] public bool _dead = false;
    [HideInInspector] public bool _heathZero = false;
    [Space(10)] 

    [Header ("Sound Properties")]
    [SerializeField] private bool _isBuilding = false;
    [SerializeField] private bool _isFrog = false;
    [SerializeField] private bool _isAI= false;
    [Space(10)] 

    [SerializeField] private bool _destructible;

    [ConditionalHide("_destructible", true)]
    [Header ("Destruction Properties")]

    [SerializeField] private GameObject[] _gmToMeshRemove;
    [ConditionalHide("_destructible", true)]
    [SerializeField] private bool _removeMeshRenderer;
    [ConditionalHide("_destructible", true)]
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
            if(_isBuilding) 
            {
                FindObjectOfType<AudioManager>().StopSpecific("building");
                FindObjectOfType<AudioManager>().PlayOneShotSound("building");
                _isBuilding = false;
            }

            if(_isAI) 
            {
                FindObjectOfType<AudioManager>().StopSpecific("explo");
                FindObjectOfType<AudioManager>().PlayOneShotSound("explo");
                FindObjectOfType<AudioManager>().StopSpecific("heli");
                _isAI = false;
            }

            if(_audio != null)_audio.isAlive = false;

            if(_removeMeshRenderer)gameObject.GetComponent<MeshCollider>().enabled = false;
            _heathZero = true;
            Explosion();
            Invoke("ActivateDeath", _deathDelay);
            if(!_removeColliderOnDeath) return;
            else
            {
                gameObject.GetComponent<BoxCollider>().enabled = false;
            }
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
            if(!_removeMeshRenderer) _originalObject.SetActive(false);
            else foreach(GameObject _meshes in _gmToMeshRemove) _meshes.GetComponent<MeshRenderer>().enabled = false; 

            foreach (GameObject _piece in _destructParts)
            {
                _piece.SetActive(true);
                Rigidbody _rb = _piece.GetComponent<Rigidbody>();
                if(_rb != null) _rb.AddExplosionForce(Random.Range(_minExploForce, _maxExploForce), _originalObject.transform.position, _exploForceRadius);
            }
        }
    }
}