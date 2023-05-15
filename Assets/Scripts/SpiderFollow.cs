using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderFollow : MonoBehaviour
{
    public float _value;
    public GameObject _playerPos;

    void FixedUpdate()
    {
        transform.position = new Vector3(_playerPos.transform.position.x, _value, _playerPos.transform.position.z);
    }
}
