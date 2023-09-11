using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
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
    [SerializeField] private Animator _anim;
    bool _processScreen = true;
    [HideInInspector] public bool _hasWonGame = false;
    bool canPlaySound = true;
    public bool _final;

    public TMP_Text[] _time; 

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
        _scoreSystem._useTime = false;
        if(_time != null) foreach(TMP_Text time in _time) time.text = "Time: " + _scoreSystem._timeText.text;
    }

    void Failed()
    {
        if(canPlaySound) FindObjectOfType<AudioManager>().PlayOneShotSound("Fail");
        canPlaySound = false;
        _playerRagdoll.SetActive(true);
        _deathCam.SetActive(true);
        if(_processScreen) Invoke("FailScreen", _delay);
    }

    void Won()
    {
        Scene scene = SceneManager.GetActiveScene();
        if(canPlaySound) FindObjectOfType<AudioManager>().PlayOneShotSound("Win");
        canPlaySound = false;
        _hasWonGame = true;
        _winCam.SetActive(true);
        _playerWinObj.SetActive(true);
        if(!_final)if(_processScreen) Invoke("WinScreen", _delay);
        if(_final && scene.name == "FinalBoss")
        {
            _anim.SetBool("fade", true); 
            Invoke("CreditsLoad", _delay);
        } 
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

    void CreditsLoad()
    {
        FindObjectOfType<AudioManager>().StopAll();
        SceneManager.LoadScene("Credits");
    }
}
