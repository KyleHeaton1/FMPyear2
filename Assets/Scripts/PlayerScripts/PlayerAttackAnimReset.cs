using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackAnimReset : MonoBehaviour
{
    [SerializeField] private GameObject _hitBox;
    public void ResetAttackHitBox(){_hitBox.SetActive(true);}
}
