using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public float velocidade;

    public float jumpForce;

    public float jumpTime;

    public LayerMask groundMask;

    [SerializeField] private Vector3 groundCheck;
    
    [SerializeField] private PlayerInput playerInput;

    private GameInput _gameInput;
    
    private Rigidbody2D _rigidbody2D;

    private Vector2 _playerMovement;

    private bool _doJump;

    private bool _isGrounded;

    private float _startJumpTime;

    private bool _canDoubleJump;

    private void OnEnable()
    {
        playerInput.onActionTriggered += OnActionTriggered;
    }
    
    private void OnDisable()
    {
        playerInput.onActionTriggered -= OnActionTriggered;
    }

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _gameInput = new GameInput();
    }

    private void Update()
    {
        _isGrounded = Physics2D.Linecast(transform.position,
            transform.position + groundCheck, groundMask);
        if(!_canDoubleJump && _isGrounded) _canDoubleJump = _isGrounded;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
        float moverV = Input.GetAxis("Vertical");
        float moverH = Input.GetAxis("Horizontal");

        Vector2 moviment = new Vector2(moverH, moverV);
        */
        
        _rigidbody2D.AddForce(_playerMovement * velocidade);
        
        Jump();
    }
    
    private void OnActionTriggered(InputAction.CallbackContext obj)
    {
        if (obj.action.name == _gameInput.Gameplay.Move.name)
        {
            _playerMovement = obj.ReadValue<Vector2>();
            _playerMovement.y = 0;
        }

        if (obj.action.name == _gameInput.Gameplay.Jump.name)
        {
            if (obj.started)
            {
                if(_isGrounded)
                {
                    _doJump = true;
                    _startJumpTime = Time.time;
                }
                else
                {
                    if (_canDoubleJump)
                    {
                        _doJump = true;
                        _startJumpTime = Time.time;
                        _canDoubleJump = false;
                    }
                }
            }

            if (obj.canceled)
            {
                _doJump = false;
            }
        }
    }

    private void Jump()
    {
        if (_doJump)
        {
            _rigidbody2D.AddForce(Vector2.up * jumpForce);
            if(Time.time - _startJumpTime > jumpTime) _doJump = false;
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + groundCheck, Color.red);
    }
}
