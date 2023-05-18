using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fail : MonoBehaviour
{
    [SerializeField] private Health _playerHealth;
    [SerializeField] private ThirdPersonCameraControl _playerCam;
    [SerializeField] private GameObject _playerMesh, _player;
    [SerializeField] private GameObject _failUI, _winUI;
    [SerializeField] private GameObject _playerRagdoll, _playerWinObj;
    [SerializeField] private GameObject _deathCam, _winCam;
    [SerializeField] private GameObject _laserCam, _tpCam;
    [SerializeField] private ScoreSystem _scoreSystem;
    [SerializeField] private PlayerMovement _pm;
    [SerializeField] private float _delay;
    [SerializeField] private Rigidbody _rb;
    bool _processScreen = true;
    bool _hasWonGame = false;

    void Update()
    {
        if(_hasWonGame == false)
        {
            if(_playerHealth._heathZero || _scoreSystem._failed) 
            {
                EndGame();
                 Failed();
            }
        }

        if(_scoreSystem._failed == false && _scoreSystem._won == true)
        {
            EndGame();
            Won();
        }
    }

    void EndGame()
    {
        _pm._canInput = _pm._canMove = _pm.enabled = _pm._readyToLaser = _pm._camReady = _playerCam._isLaserMode = false;
        _playerMesh.SetActive(false);
        _deathCam.SetActive(true);
        _tpCam.SetActive(false);
        _laserCam.SetActive(false); 
        _pm._laserUI.SetActive(false);
        _pm.StopLaser();
        _rb.mass = 10;
    }

    void Failed()
    {
        _playerRagdoll.SetActive(true);
        _deathCam.SetActive(true);
        if(_processScreen) Invoke("FailScreen", _delay);
    }

    void Won()
    {
        _hasWonGame = true;
        _winCam.SetActive(true);
        _playerWinObj.SetActive(true);
        if(_processScreen) Invoke("WinScreen", _delay);
    }

    void WinScreen()
    {
        _processScreen = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _winUI.SetActive(true);
        FindObjectOfType<AudioManager>().StopAll();
    }

    void FailScreen()
    {
        _processScreen = false;
        _failUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        FindObjectOfType<AudioManager>().StopAll();
    }
}
