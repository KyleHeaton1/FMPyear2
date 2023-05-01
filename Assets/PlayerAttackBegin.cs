using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackBegin : MonoBehaviour
{
    [SerializeField] private GameObject _hitBox;
    public void StartAttackHitBox(){_hitBox.SetActive(true);}
}
