using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _laserMoveSpeed;
    [SerializeField] private float _groundDrag;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpCooldown;
    [SerializeField] private float _airMultiplier;
    
    bool _isMoving;
    float _baseSpeed;
    bool _readyToJump;
    bool _canMove;

    [Header("Dash Settings")]
    [SerializeField] private float _dashForce;
    [SerializeField] private float _dashCooldown;
    [SerializeField] private int _dashAmount;
    [SerializeField] private float _dashRefreshTimer;
    
    float _baseDashTime;
    int _dashCount = 0;
    bool _canDash;
    bool _readyToDash;
    
    [Header("Ground Check Settings")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _whatIsGround;
    
    bool _grounded;
    bool _readyToLand;

    Rigidbody _rb;

    [SerializeField] private Transform _orientation;
    float _horizontalInput;
    float _verticalInput;

    Vector3 _moveDirection;

    [Header("Attack Settings")]
    [SerializeField] GameObject _attackHitBox;
    [SerializeField] int _damage;
    [SerializeField] float _attackCooldown;
    [SerializeField] float _GPCooldown;
    [SerializeField] float _GPForce;
    bool _readyToAttack;
    bool _opisiteAttackAnim;
    bool _readyToGP;
    bool _canGP;

    [Header("Laser Settings")]
    [SerializeField] float _laserTickDamage;
    [SerializeField] Transform _firePoint;
    [SerializeField] GameObject _laserPoint;
    [SerializeField] GameObject _laserUI;
    [SerializeField] GameObject _laserVFXObj;
    [SerializeField] LineRenderer _line;
    [SerializeField] Camera _laserCamera;
    [SerializeField] GameObject[] _laserEyes;
    [SerializeField] float rayLength;
    bool _activeLaser;
    bool _readyToLaser;

    [Header("Other Settings")]
    [SerializeField] private ThirdPersonCameraControl _camControl;
    [SerializeField] private Animator _anim;
    float _animVelocity = 0;
    float _veloAcceleration = .1f;
    float _veloDeceleration = .1f;
    int _veloHash;

    [SerializeField] private _States _state;
    public enum _States
    {
        idle, //done
        land, //done
        midair, 
        laserWalk,
        laserIdle,
        groundPound,
        attack1, //done
        attack2, //done
        airAttack1, //done
        airAttack2, //done
        dash, //done
        jump, //done
        damage,
        runnning  //done
    }
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _line = GetComponent<LineRenderer>();
        _readyToJump = _readyToDash = _canDash = _rb.freezeRotation = _readyToAttack = _readyToGP = _readyToLaser =true;
        _baseSpeed = _moveSpeed;
        _baseDashTime = _dashRefreshTimer;
        _veloHash = Animator.StringToHash("velocity");
        _readyToLand = false;
        _state = _States.idle;
    }
    void Update()
    {
        //raycast generated for ground check, spawned from the players height down. 
        _grounded = Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f+ 0.3f, _whatIsGround);
        SpeedControl();
        Inputs();
        ProcessAnims();
        //slows down object if grounded, if not then the lower the drag means the less it will get slowed down
        //ON GROUND
        if (_grounded)
        {
            _rb.drag = _groundDrag;
            if(_readyToLand)
            {
                _state = _States.land;
                _anim.SetInteger("state", 3);
                _readyToLand = false;
            }
            _canGP = false;
        }

        //IN AIR
        else
        {
            _rb.drag = 0;
            _readyToLand = _canGP = true;
            _state = _States.jump;
            _isMoving = false;
        }
        //if the dash count is bigger or equal to dash amount - start coroutine
        if(_dashCount >= _dashAmount)StartCoroutine(DashMax());
        else if(_dashCount <= _dashAmount && _dashCount != 0)
        {
            _dashRefreshTimer -= Time.deltaTime;

            if(_dashRefreshTimer <= 0)
            {
                DashReplenBetween();
                _dashRefreshTimer = _baseDashTime;
            }
        }

    }
    //Processes movment faster than update
    void FixedUpdate(){Movement();} 
    void Inputs()
    {
        //gets horizontal and vertical input to a float
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if(_horizontalInput != 0 || _verticalInput != 0)
        {
            _isMoving = true;
            if(_activeLaser)_state = _States.laserWalk;
            else _state = _States.runnning;

        }
        else
        {
            _isMoving = false;
            if(_activeLaser)_state = _States.laserIdle;
            else _state = _States.idle;
        } 
        if(Input.GetButton("Jump") && _readyToJump && _grounded)
        {
            //makes it so we cant jump again
            _readyToJump = false;

            //activates jump
            Jump();

            _state = _States.jump;

            //waits the cooldown time so we cant just straight after we just finished one
            Invoke("ResetJump", _jumpCooldown);
        }
        if(Input.GetKey(KeyCode.LeftShift) && _readyToDash && _canDash)
        {
            //makes it so we cant dash again
            _readyToDash = false;

            //activates dash
            Dash();

            _state = _States.dash;

            //adds to dash counter
            _dashCount++;

            //waits the cooldown time so we cant just straight after we just finished one
            Invoke("DashReplen", _dashCooldown);
        }
        if(Input.GetMouseButtonDown(0) && _readyToAttack && _camControl._isLaserMode == false)
        {
            //makes it so we cant attack again
            _readyToAttack = false;

            //activates attack
            Attack();

            //waits the cooldown time so we cant just straight after we just finished one
            Invoke("ResetAttack", _attackCooldown);
            _activeLaser = false;
        }
        if(Input.GetKey(KeyCode.LeftControl) && _readyToGP && _canGP)
        {
            //makes it so we cant ground pound again
            _readyToGP = false;

            //activates ground pound
            GroundPound();

            //waits the cooldown time so we cant just straight after we just finished one
            Invoke("ResetGroundPound", _GPCooldown);
        }
        if(Input.GetMouseButton(0) && _readyToLaser && _camControl._isLaserMode)
        {
            //changes the speed of the player while in laser mode (cam zoom mode)
            _moveSpeed = _laserMoveSpeed;
            //activates laser
            Laser();
            //activaes laser bool making so other anims cant override it
            _activeLaser = true;
        }
        if(_camControl._isLaserMode == false) _activeLaser = false;

        //Camera control change - Mouse button 1 (right click) changes which camera mode the player is on, each varible resets depending on what input has been pressed down or up
        if(Input.GetMouseButtonDown(1))
        {
            SwitchCam(true);
            _laserUI.SetActive(true);
            _moveSpeed = _laserMoveSpeed;
        }
        if(Input.GetMouseButtonUp(1))
        {
            SwitchCam(false);
            _laserUI.SetActive(false);
            _moveSpeed = _baseSpeed;
            StopLaser();
        }
        if(Input.GetMouseButtonUp(0)) StopLaser();
    }

    // || PLAYER CORE MOVEMENT ||
    void Movement()
    {
        //calculates the movement direction, the orientation foward is equal to vert input, if vert input is negative then it will change to -forward, same goes for horizontal 
        _moveDirection = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;
        
        //while on ground
        if(_grounded && _isMoving)
        {
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);
            
            _state = _States.runnning;
            if (_animVelocity < 1) _animVelocity += _veloAcceleration;      
            _anim.SetFloat(_veloHash, _animVelocity);     
        }
        if(_grounded && !_isMoving)
        {
           if(_animVelocity > 0) _animVelocity -=  _veloDeceleration;
           if(_animVelocity == 0)  _state = _States.idle;
           _anim.SetFloat(_veloHash, _animVelocity);
        }
        //adds force to rigidbody, normalizes direction meaning it keeps vector magintude to 1 keeping the magintude to a small number
        //adds movement speed to the direction times a constant force along with the force mode
        //movement while in air
        else _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f * _airMultiplier, ForceMode.Force);
    }
    void SpeedControl()
    {
        //creates a new vector which measures the x and z of the rb velocity
        Vector3 _flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        //limit velocity if it goes over the movement speed
        if(_flatVel.magnitude > _moveSpeed)
        {
            //squishes down the new velocity by changing magintutde to 1 and multiplying it by the current movement speed
            Vector3 _limitedVel = _flatVel.normalized * _moveSpeed;
            //changes the velocity to the new limited version
            _rb.velocity = new Vector3(_limitedVel.x, _rb.velocity.y, _limitedVel.z);
        }
    }
    
    // || JUMPING ||
    
    //adds an upward force for the player with delays between each jump
    void Jump()
    {
        //resets the y velocity
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        //adds impulse force to rigidbody
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }
    void ResetJump(){_readyToJump = true;}

    // || DASHING ||

    //lots of varible changes in order to process the correct times for when the player is allowed to dash
    void Dash()
    {
        //the dash refresh timer is set back to the base time, grounded is set to false and the move speed is increased
        _dashRefreshTimer = _baseDashTime;
        _grounded = false;
        _moveSpeed = _dashForce;
    }
    void DashReplen()
    {
        //replens dash after dash has been carried out, setting values back to normal
        _readyToDash = true;
        _moveSpeed = _baseSpeed;
    }

    //Dash timers - to manage how the player cannot abuse dashes
    IEnumerator DashMax()
    {
        //count is reset, then we wait a penalty so the player is not allowed to dash for a certain amount of time before next dash
        _dashCount = 0;
        _canDash = false;
        yield return new WaitForSeconds(5);
        _readyToDash = _canDash = true;
        _dashRefreshTimer = _baseDashTime;
    }
    void DashReplenBetween()
    {
        //replen dash if the player has not dashed for a while and has not met max dashes
        _readyToDash = true;
        _dashCount = 0;
    }

    // || ATTACKING ||
    void Attack()
    {
        _attackHitBox.SetActive(true);
        if(_opisiteAttackAnim)
        {
            if(_grounded) 
            {
                _readyToJump = false;
                _state =_States.attack1;
            }
            else _state =_States.airAttack1;               
            _opisiteAttackAnim = false;
        }
        else 
        {
            if(_grounded)
            {
                _state =_States.attack2;
                _readyToJump = false;
            }
            else _state =_States.airAttack2;               
            _opisiteAttackAnim = true;
        }
        
    }
    void ResetAttack()
    {
        _readyToAttack = true;
        if(_grounded)_readyToJump = true;
    }

    // || GROUND POUND ||
    void GroundPound()
    {
        _state =_States.groundPound;
        Debug.Log("fart");
        _rb.AddForce(-transform.up * _GPForce, ForceMode.Impulse);
    }
    void ResetGroundPound()
    {
        _readyToGP = true;
    }

    void SwitchCam(bool _switch)
    {
        if(_switch) _camControl._isLaserMode = true;
        else _camControl._isLaserMode = false;
    }

    // || LASER ||
    void Laser()
    {
        _line.SetPosition(0, _firePoint.transform.position);
        RaycastHit _laser;
        Vector3 _rayOrigin = _firePoint.transform.position;
        foreach (GameObject _e in _laserEyes) _e.SetActive(true);
        _laserVFXObj.SetActive(true);
        if(Physics.Raycast(_firePoint.transform.position, _laserCamera.transform.forward, out _laser, rayLength))
        {
            _laserPoint.transform.LookAt(_laser.point);
            _line.SetPosition(1, _laser.point);
           _laserVFXObj.transform.LookAt(_laser.point);
        }
        else 
        {
            _line.SetPosition(1, _rayOrigin + (_laserCamera.transform.forward * rayLength));
           _laserVFXObj.transform.LookAt(_rayOrigin + (_laserCamera.transform.forward * rayLength));
        }
        Debug.DrawRay(_firePoint.transform.position, _laserCamera.transform.forward, Color.green);
        _line.enabled = true;
        
    }
    void StopLaser()
    {
        _activeLaser = _line.enabled =false;
        _laserVFXObj.SetActive(false);
        foreach (GameObject _e in _laserEyes) _e.SetActive(false);
    }
    // || ANIMATIONS ||
    void ProcessAnims()
    {
        //movement anims
        if(_state == _States.idle) _anim.SetInteger("state", 0);
        if(_state == _States.runnning) _anim.SetInteger("state", 1);
        if(_state == _States.jump) _anim.SetInteger("state", 2);
        if(_state == _States.land) _anim.SetInteger("state", 3);
        if(_state == _States.dash) _anim.SetInteger("state", 4);
        if(_state == _States.midair) _anim.SetInteger("state", 5);
        //action anims
        if(_state == _States.attack1) _anim.SetInteger("state", 6);
        if(_state == _States.attack2) _anim.SetInteger("state", 7);
        if(_state == _States.airAttack1) _anim.SetInteger("state", 8);
        if(_state == _States.airAttack2) _anim.SetInteger("state", 9);
        if(_state == _States.groundPound) _anim.SetInteger("state", 10);
        if(_state == _States.laserIdle) _anim.SetInteger("state", 11);
        if(_state == _States.laserWalk) _anim.SetInteger("state", 12);

    }
}