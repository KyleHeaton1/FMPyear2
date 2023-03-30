using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
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
    bool _readyToAttack;
    bool _opisiteAttackAnim;


    [Header("Other Settings")]
    [SerializeField] Animator _anim;
    float _animVelocity = 0;
    float _veloAcceleration = .1f;
    float _veloDeceleration = .1f;
    int _veloHash;


    public enum _States
    {
        idle,
        land,
        midair,
        laser,
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

    public _States _state;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _readyToJump = _readyToDash = _canDash = _rb.freezeRotation = _readyToAttack = true;
        _baseSpeed = _moveSpeed;
        _baseDashTime = _dashRefreshTimer;
        _veloHash = Animator.StringToHash("velocity");
        _anim.SetInteger("state", 0);
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
        }

        //IN AIR
        else
        {
            _rb.drag = 0;
            _readyToLand = true;
            _state = _States.midair;
            _isMoving = false;
        }

        Debug.Log(_readyToLand);

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

    void Inputs()
    {
        
        //gets horizontal and vertical input to a float
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if(_horizontalInput != 0 || _verticalInput != 0)
        {
            _isMoving = true;
            _state = _States.runnning;
        }
        else
        {
            _isMoving = false;
            _state = _States.idle;
        } 
        
        //checks the requirements to jump
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


        if(Input.GetMouseButtonDown(0) && _readyToAttack)
        {

            
            //makes it so we cant attack again
            _readyToAttack = false;

            //activates attack
            Attack();

            //waits the cooldown time so we cant just straight after we just finished one
            Invoke("ResetAttack", _attackCooldown);
        }
    }
    
    void FixedUpdate()
    {
        Movement();    
    }

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

    //Jumping - adds an upward force for the player with delays between each jump
    void Jump()
    {
        //resets the y velocity
        _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        //adds impulse force to rigidbody
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }
    void ResetJump()
    {
        _readyToJump = true;
    }
    
    //Dashing - lots of varible changes in order to process the correct times for when the player is allowed to dash
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

    //attacking
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
    public void ResetAttackHitBox()
    {
        _attackHitBox.SetActive(false);
    }

    //Ground Pound

    //Laser


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

    }
}
