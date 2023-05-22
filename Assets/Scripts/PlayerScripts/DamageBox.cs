using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBox : MonoBehaviour
{   
    [SerializeField] private bool _isGPDamage;
    [SerializeField] GameObject _attackRubble;
    int _damageBoxDamage;
    [SerializeField] private PlayerMovement _pm;
    void Start(){if(!_isGPDamage)_damageBoxDamage = _pm._damage; else _damageBoxDamage = _pm._GPdamage;}
    void OnTriggerEnter(Collider other)
    {
        Health _destructHealth = other.gameObject.GetComponent<Health>();
        if(_destructHealth != null) _destructHealth.TakeDamage(_damageBoxDamage);
        if(other.gameObject.tag == "Buildings" ||other.gameObject.tag == "agent")
        {
            if(_attackRubble!= null){GameObject _vfx = Instantiate(_attackRubble, this.transform.position, this.transform.rotation);}
            FindObjectOfType<AudioManager>().StopSpecific("impact");
            if(!_isGPDamage)FindObjectOfType<AudioManager>().PlayOneShotSound("impact");
        }
    }
}