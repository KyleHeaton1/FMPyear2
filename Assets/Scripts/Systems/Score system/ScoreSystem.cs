using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    [Header ("Score Properties")]
    [SerializeField] public int _score = 0;
    [SerializeField] private int _scoreToReach;
    [SerializeField] private TMP_Text _scoreText;

    [SerializeField] private bool _useTimeSystem;

    [ConditionalHide("_useTimeSystem", true)]
    [Header ("Timer Properties")]

    [SerializeField] private TMP_Text _timeText;
    [ConditionalHide("_useTimeSystem", true)]
    [SerializeField] private float _timeLimit;
    [ConditionalHide("_useTimeSystem", true)]
    [SerializeField] private bool _addTime; 
    float _currentTime;
    int _currentSeconds;
    int _currentMins;

    [HideInInspector] public bool _failed = false;
    [HideInInspector] public bool _won = false;
    
    void Start()
    {
        if(!_addTime) _currentTime = _timeLimit;
    }

    public void AddScore(int _addedScore)
    {
        _score += _addedScore;
    }

    public void SubtractScore(int _subtractedScore)
    {
        _score -= _subtractedScore;
    }

    // Update is called once per frame
    void Update()
    {
        if (_score >= _scoreToReach)
        {
            Finished(true);
        }

        TimeSytem();

        if(_scoreText != null) _scoreText.text = "" + _score;
    }

    void Finished(bool _hasWon)
    {
        if(!_hasWon)
        {
            _failed = true; 
            _won = false;
        } // fail
        else
        {
            _failed = false;
            _won = true;
        } // win
    }

    void TimeSytem()
    {
        if(!_useTimeSystem) return;
        else 
        {
            if(_addTime)
            {
                _currentTime += Time.deltaTime;
                if(_currentTime >= _timeLimit) Finished(false);
            } 
            else  
            {
                _currentTime -= Time.deltaTime;
                if(_currentTime <= 0) Finished(false);
            }
        }

        _currentMins = (int)Mathf.Floor(_currentTime / 60);
        _currentSeconds = (int)(_currentTime % 60);

        if(_timeText != null) _timeText.text = "" + _currentMins + ":" + _currentSeconds;
    }
}