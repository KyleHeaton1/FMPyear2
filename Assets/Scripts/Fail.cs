using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fail : MonoBehaviour
{
    [SerializeField] private Health _playerHealth;
    [SerializeField] private GameObject _playerMesh;
    [SerializeField] private GameObject _failUI;
    [SerializeField] private GameObject _playerRagdoll;
    [SerializeField] private GameObject _deathCam;
    [SerializeField] private GameObject _laserCam, _tpCam;
    [SerializeField] private ScoreSystem _scoreSystem;
    [SerializeField] private PlayerMovement _pm;
    [SerializeField] private float _delay;
    [SerializeField] private Rigidbody _rb;
    bool _processScreen = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_playerHealth._heathZero|| _scoreSystem._failed) EndGame();
    }

    void EndGame()
    {
        _pm._canInput = false;
        _pm._canMove = false;
        _pm.enabled = false;
        _playerMesh.SetActive(false);
        _playerRagdoll.SetActive(true);
        _deathCam.SetActive(true);
        _tpCam.SetActive(false);
        _laserCam.SetActive(false); 
        _rb.mass = 10;
        if(_processScreen) Invoke("FailScreen", _delay);
    }

    void FailScreen()
    {
        _processScreen = false;
        _failUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }
}
