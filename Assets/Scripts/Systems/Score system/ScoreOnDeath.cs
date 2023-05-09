using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreOnDeath : MonoBehaviour
{
    Health _health;
    ScoreSystem _scoring;
    bool _canAdd = true;
    [SerializeField] private int _scoreToAdd;

    void Start()
    {
        _health = gameObject.GetComponent<Health>();
        GameObject _gm = GameObject.Find("Game Manager");
        _scoring = _gm.GetComponent<ScoreSystem>();
    }

    void Update()
    {
        if(_health._heathZero && _canAdd)
        {
            ScoreToAdd();
        }
    }

    void ScoreToAdd()
    {
        if(_canAdd)_scoring.AddScore(_scoreToAdd);
        _canAdd = false;
    }
}
