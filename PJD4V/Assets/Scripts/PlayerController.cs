using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public float velocidade;

    [SerializeField] private PlayerInput playerInput;

    private GameInput _gameInput;
    
    private Rigidbody2D _rigidbody2D;

    private Vector2 _playerMovement;

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

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
        float moverV = Input.GetAxis("Vertical");
        float moverH = Input.GetAxis("Horizontal");

        Vector2 moviment = new Vector2(moverH, moverV);
        */
        
        _rigidbody2D.AddForce(_playerMovement * velocidade);
    }
    
    private void OnActionTriggered(InputAction.CallbackContext obj)
    {
        if (obj.action.name == _gameInput.Gameplay.Move.name)
        {
            _playerMovement = obj.ReadValue<Vector2>();
        }
    }
}
