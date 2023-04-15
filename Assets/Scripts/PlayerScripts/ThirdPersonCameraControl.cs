using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraControl : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _orientation;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _playerObj;
    [SerializeField] private Rigidbody _rb;
    [Header("Speed")]
    [SerializeField] private float _rotationSpeed;

    [Header("Alternate Cam Modes")]
    [SerializeField] public bool _isLaserMode;
    [SerializeField] private Transform _laserLookAt;
    [SerializeField] private GameObject _thirdPersonCam;
    [SerializeField] private GameObject _laserCam;
    void Start(){Cursor.lockState = CursorLockMode.Locked;}
    void FixedUpdate()
    {
        // rotate orientation
        Vector3 _viewDir = _player.position - new Vector3(transform.position.x, _player.position.y, transform.position.z);
        _orientation.forward = _viewDir.normalized;
        // roate player object
        if(!_isLaserMode)
        {
            SwitchCamera();
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 inputDir = _orientation.forward * verticalInput + _orientation.right * horizontalInput;
            if (inputDir != Vector3.zero) _playerObj.forward = Vector3.Slerp(_playerObj.forward, inputDir.normalized, Time.deltaTime * _rotationSpeed);
        }
        else
        {
            SwitchCamera();
            Vector3 _viewDirForLaser = _laserLookAt.position - new Vector3(transform.position.x, _laserLookAt.position.y, transform.position.z);
            _orientation.forward = _viewDirForLaser.normalized;
            _playerObj.forward = _viewDirForLaser.normalized;
        }
    }
    void SwitchCamera()
    {
        _thirdPersonCam.SetActive(false);
        _laserCam.SetActive(false);
        if(_isLaserMode)
        {
            _laserCam.SetActive(true);
            return;
        }
        else _thirdPersonCam.SetActive(true);
    }
}