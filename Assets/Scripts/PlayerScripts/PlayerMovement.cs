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

    [SerializeField] private float _dashForce;
    [SerializeField] private float _dashCooldown;

    bool _readyToJump;
    bool _readyToDash;
    bool _canMove;

    [Header("Ground Check Settings")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _whatIsGround;
    bool _grounded;

    Rigidbody _rb;
    [SerializeField] private Transform _orientation;
    float _horizontalInput;
    float _verticalInput;

    Vector3 _moveDirection;
    



    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _readyToJump = true;
        _readyToDash = true;
    }
    
    void Update()
    {
        //raycast generated for ground check, spawned from the players height down. 
        _grounded = Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f+ 0.3f, _whatIsGround);

        SpeedControl();
        Inputs();

        //slows down object if grounded, if not then the lower the drag means the less it will get slowed down
        if (_grounded)
        {
            _rb.drag = _groundDrag;
        }
        else 
        {
            _rb.drag = 0;
        }
    }

    void Inputs()
    {
        
        //gets horizontal and vertical input to a float
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");


        //checks the requirements to jump
        if(Input.GetButton("Jump") && _readyToJump && _grounded)
        {
            //makes it so we cant jump again
            _readyToJump = false;

            //activates jump
            Jump();

            //waits the cooldown time so we cant just straight after we just finished one
            Invoke("ResetJump", _jumpCooldown);
        }

        
        if(Input.GetKey(KeyCode.LeftShift) && _readyToDash)
        {
            Debug.Log("ALLah");
            //makes it so we cant jump again
            _readyToDash = false;

            //activates jump
            Dash();

            //waits the cooldown time so we cant just straight after we just finished one
            Invoke("ResetDash", _dashCooldown);
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
        if(_grounded)
        {
            //adds force to rigidbody, normalizes direction meaning it keeps vector magintude to 1 keeping the magintude to a small number
            //adds movement speed to the direction times a constant force along with the force mode
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            //movement while in air
            _rb.AddForce(_moveDirection.normalized * _moveSpeed * 10f * _airMultiplier, ForceMode.Force);
        }
        

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
    void Dash()
    {
        _grounded = false;
        _moveSpeed = 30;
    }
    void ResetDash()
    {
        _readyToDash = true;
        _moveSpeed = 5;
    }

}
