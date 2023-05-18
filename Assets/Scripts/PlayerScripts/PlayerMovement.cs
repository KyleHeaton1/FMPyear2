using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Rendering.HighDefinition;

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
    [HideInInspector] public bool _canMove;

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
    
    [HideInInspector] public bool _grounded;
    bool _readyToLand;

    Rigidbody _rb;

    [SerializeField] private Transform _orientation;
    float _horizontalInput;
    float _verticalInput;

    Vector3 _moveDirection;

    [Header("Attack Settings")]
    [SerializeField] GameObject _attackHitBox;
    [SerializeField] GameObject _gpHitBox;
    [SerializeField] public int _damage;
    [SerializeField] public int _GPdamage;
    [SerializeField] float _attackCooldown;
    [SerializeField] float _GPCooldown;
    [SerializeField] float _GPForce;
    [SerializeField] GameObject _GPdecal;
    [SerializeField] Transform _GPFP;
    [SerializeField] Material _GPCrackMat;
    [SerializeField] DecalProjector _fadeDecal; 
    [SerializeField] GameObject _vfxCollide;
    bool _resetFade;
    int _fadeID;
    float crackTime;

    bool _readyToAttack;
    bool _opisiteAttackAnim;
    bool _readyToGP;
    bool _canGP;
    bool _isGP = false;

    [Header("Laser Settings")]
    [SerializeField] int _laserTickDamage;
    [SerializeField] Transform _firePoint;
    [SerializeField] GameObject _laserPoint;
    [SerializeField] public GameObject _laserUI;
    [SerializeField] GameObject _laserVFXObj;
    [SerializeField] LineRenderer _line;
    [SerializeField] Camera _laserCamera;
    [SerializeField] GameObject[] _laserEyes;
    [SerializeField] float rayLength;
    [SerializeField] private ValueSlider _energyBar;
    [SerializeField] int _startingEnergy;
    [SerializeField] int _currentEnergy;
    bool _activeLaser;
    [HideInInspector] public bool _readyToLaser;

    [Header("Other Settings")]
    [SerializeField] private ThirdPersonCameraControl _camControl;
    [SerializeField] private Animator _anim;
    float _animVelocity = 0;
    float _veloAcceleration = .1f;
    float _veloDeceleration = .1f;
    int _veloHash;
    bool _canJump, _canAttack, _canLaser;

    [SerializeField] private Health _playerHealth;
    [SerializeField] private Material _damageMat;

    [HideInInspector] public bool _canInput = true;
    [HideInInspector] public bool _camReady = true;

    [SerializeField] private _States _state;


    public enum _States
    {
        idle, 
        land, 
        midair, 
        laserWalk,
        laserIdle,
        groundPound,
        attack1, 
        attack2, 
        airAttack1, 
        airAttack2, 
        dash, 
        jump, 
        damage,
        runnning  
    }

    void Start()
    {
        //get components 
        _rb = GetComponent<Rigidbody>();
        _line = GetComponent<LineRenderer>();
        
        //set ALL bools to true
        _readyToJump = _readyToDash = _canDash = _rb.freezeRotation = _readyToAttack = _readyToGP = _readyToLaser = _canJump = _canAttack = _canLaser = _canMove = true;
        _readyToLand = false;

        //assign floats/ints to base values
        _baseSpeed = _moveSpeed;
        _baseDashTime = _dashRefreshTimer;

        //set animations for blend tree and animator
        _veloHash = Animator.StringToHash("velocity");
        _state = _States.idle;

        //reset crack shader speed
        _fadeID = Shader.PropertyToID("_speed");

        //set energy value to slider
        if(_energyBar != null)_energyBar.SetMaxValue(_startingEnergy);
        _currentEnergy = _startingEnergy;
    }

    void Update()
    {
        //raycast generated for ground check, spawned from the players height down. 
        _grounded = Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f+ 0.3f, _whatIsGround);
        if(_energyBar!= null) _energyBar.SetValue(_currentEnergy);

        //process all functions
        SpeedControl();
        if(_canInput)Inputs();
        ProcessAnims();

        if(_playerHealth._currentHealth <= 50) _damageMat.SetFloat("_CutDamage",  99);
        else _damageMat.SetFloat("_CutDamage",  0);

        //slows down object if grounded, if not then the lower the drag means the less it will get slowed down
        //ON GROUND
        if (_grounded)
        {
            _rb.drag = _groundDrag;
            if(_readyToLand)
            {
                if(_isGP)
                {   
                    //Actives the hitbox and the animation for the ground pound
                    _gpHitBox.SetActive(true);
                    _anim.SetFloat("landSpeedMultiply", .5f);

                    //Changes the speed of the crack time
                    _GPCrackMat.SetFloat("_speed", crackTime);
                    FindObjectOfType<AudioManager>().PlayOneShotSound("gp");
                    //Checks if groundpound hits the floor
                    RaycastHit _rayHit;
                    if (Physics.Raycast(_GPFP.position, transform.TransformDirection(Vector3.down), out _rayHit, 1f))
                    {
                        _resetFade = false;

                        //Spawns the decal and visual effects for the groundpound
                        GameObject _decalPrefab = Instantiate(_GPdecal, _rayHit.point, _GPFP.rotation);
                        GameObject _vfxPrefb = Instantiate(_vfxCollide, _rayHit.point, this.transform.rotation);
                        _fadeDecal = _decalPrefab.GetComponent<DecalProjector>();

                        //Clears up the decals and visual effects
                        Invoke("ResetFade", 3);
                        Destroy(_decalPrefab, 6);
                        Destroy(_vfxPrefb, 2);
                    }
                }
                else _anim.SetFloat("speed", 1f);

                _state = _States.land;
                 FindObjectOfType<AudioManager>().PlayOneShotSound("land");
                _anim.SetInteger("state", 3);
                _anim.SetBool("gpToLand" ,true);

                //Starts the refresh moves function
                RefreshMoves();
                _readyToLand = _isGP = false;
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
            _anim.SetBool("gpToLand" ,true);
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

        if(_currentEnergy <= 0) 
        {
            _canLaser = false;
            StopLaser();
        }
        else _canLaser = true;
    }

    //Processes movment faster than update
    void FixedUpdate(){if(_canMove)Movement();} 
    
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
        if(Input.GetButton("Jump") && _readyToJump && _grounded && _canJump)
        {
            //makes it so we cant jump again
            _readyToJump = false;

            //activates jump
            Jump();

            _state = _States.jump;

            FindObjectOfType<AudioManager>().PlayOneShotSound("jump");

            //waits the cooldown time so we cant just straight after we just finished one
            Invoke("ResetJump", _jumpCooldown);
        }
        if(Input.GetKey(KeyCode.LeftShift) && _readyToDash && _canDash)
        {
            //makes it so we cant dash again
            _readyToDash = _canLaser = false;

            //activates dash
            Dash();

            _state = _States.dash;

            //adds to dash counter
            _dashCount++;

            //waits the cooldown time so we cant just straight after we just finished one
            Invoke("DashReplen", _dashCooldown);
        }
        if(Input.GetButton("Fire1") && _readyToAttack && _camControl._isLaserMode == false && _canAttack)
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
            _readyToGP = _canAttack = _canLaser = false;

            //activates ground pound
            GroundPound();

            //waits the cooldown time so we cant just straight after we just finished one
            Invoke("ResetGroundPound", _GPCooldown);
            
        }
        if(Input.GetButton("Fire1") && _readyToLaser && _camControl._isLaserMode && _canLaser)
        {
            //changes the speed of the player while in laser mode (cam zoom mode)
            _moveSpeed = _laserMoveSpeed;

            //activates laser
            Laser();

            FindObjectOfType<AudioManager>().PlayOneShotSound("laser");

            //activaes laser bool making so other anims cant override it
            _activeLaser = true;
        }

        if(_camControl._isLaserMode == false) _activeLaser = false;
        //Camera control change - Mouse button 1 (right click) changes which camera mode the player is on, each varible resets depending on what input has been pressed down or up
        if(Input.GetButtonDown("Fire2"))
        {
            SwitchCam(true);
            _laserUI.SetActive(true);

            _moveSpeed = _laserMoveSpeed;
            _readyToJump = _canDash = false;
        }
        if(Input.GetButtonUp("Fire2"))
        {
            StopLaser();
            SwitchCam(false);
            _laserUI.SetActive(false);

            _moveSpeed = _baseSpeed;
            _readyToJump = _canDash = true;
        }
        if(Input.GetButtonUp("Fire1")) StopLaser();


        if(_fadeDecal != null)
        {
            float _fadeDecalFactor =  _fadeDecal.fadeFactor;
            if(_resetFade) _fadeDecalFactor -= Time.deltaTime * 0.05f;
            else _fadeDecalFactor += Time.deltaTime;

            if(_fadeDecalFactor >= 0.036f) _fadeDecalFactor = 0.036f;

            if(_fadeDecalFactor == 0) _GPCrackMat.SetFloat("_speed", _GPCrackMat.GetFloat(_fadeID) - Time.deltaTime);
            else _GPCrackMat.SetFloat("_speed", _GPCrackMat.GetFloat(_fadeID) + Time.deltaTime * 2);

            _fadeDecal.fadeFactor = _fadeDecalFactor;
        }
    }
    void RefreshMoves(){_canAttack = _canGP = _canJump = _canLaser =  _canMove = true;}
    void ResetFade(){_resetFade = true;}
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
        RefreshMoves();
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
        FindObjectOfType<AudioManager>().PlayOneShotSound("dash");
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

        FindObjectOfType<AudioManager>().PlayOneShotSound("attack");
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
        _attackHitBox.SetActive(false);
    }


    // || GROUND POUND ||
    void GroundPound()
    {
        _state =_States.groundPound;
        _canMove = _canAttack =  false;
        _rb.AddForce(-transform.up * _GPForce, ForceMode.Impulse);
        _readyToLand = _isGP =true;
    }
    void ResetGroundPound() 
    { 
        _readyToGP = true;
    }
    void SwitchCam(bool _switch)
    {
        if(!_camReady) return;
        if(_switch) _camControl._isLaserMode = true;
        else _camControl._isLaserMode = false;
    }


    // || LASER ||
    void Laser()
    {
        _currentEnergy -= 1;
        _readyToJump = false;
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
           Health _destructHealth = _laser.collider.gameObject.GetComponent<Health>();
           if(_destructHealth != null) _destructHealth.TakeDamage(_laserTickDamage);
        }
        else 
        {
            _line.SetPosition(1, _rayOrigin + (_laserCamera.transform.forward * rayLength));
            _laserVFXObj.transform.LookAt(_rayOrigin + (_laserCamera.transform.forward * rayLength));
        }
        Debug.DrawRay(_firePoint.transform.position, _laserCamera.transform.forward, Color.green);
        _line.enabled = true;
    }
    public void StopLaser()
    {
        _activeLaser = _line.enabled =false;
        _laserVFXObj.SetActive(false);
        foreach (GameObject _e in _laserEyes) _e.SetActive(false);
        FindObjectOfType<AudioManager>().StopSpecific("laser");
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