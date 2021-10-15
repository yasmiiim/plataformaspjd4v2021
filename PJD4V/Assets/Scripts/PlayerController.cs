using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public int maxEnergy;

    public float spendRate;

    public float energyBitRecover;
    
    public float velocidade;

    public float jumpForce;

    public float jumpTime;

    public float jetpackForce;

    public LayerMask killMask;
    
    public LayerMask groundMask;

    public ContactFilter2D groundFilter;

    [SerializeField] private Vector3 groundCheck;

    [SerializeField] private Vector3 boxSize;
    
    [SerializeField] private PlayerInput playerInput;

    [SerializeField] private GameObject jetEffect;

    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private GameObject muzzlePrefab;

    [SerializeField] private Transform shootOrigin;
    
    private GameInput _gameInput;
    
    private Rigidbody2D _rigidbody2D;

    private Vector2 _playerMovement;

    private bool _doJump;

    private bool _isGrounded;

    private float _startJumpTime;

    private bool _canDoubleJump;

    private bool _isMovingRight = true;

    private bool _active = true;

    private bool _dead;

    private Animator _animator;

    private float _currentEnergy;

    private bool _canJetpack;

    private bool _isJetpacking;

    private bool _isShooting;
    private bool _shotMade;
    
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
        _animator = GetComponent<Animator>();
        
        _gameInput = new GameInput();

        _currentEnergy = maxEnergy;
    }

    private void Update()
    {
       // _isGrounded = Physics2D.Linecast(transform.position,
       //     transform.position + groundCheck, groundMask);

        //_isGrounded = Physics2D.BoxCast(transform.position + new Vector3(groundCheck.x * transform.localScale.x, groundCheck.y, 0f),
        //    boxSize, 0f, Vector2.up, groundCheck.z, groundMask);

        //RaycastHit2D[] hits = new RaycastHit2D[]{};

        /*
        int hitResults = Physics2D.BoxCast(
            transform.position + new Vector3(groundCheck.x * transform.localScale.x, groundCheck.y, 0f),
            boxSize, 0, Vector2.up, groundFilter, hits, groundCheck.z);
        if (hitResults > 0) _isGrounded = true;
        */
        
        /*
        int hitResults = Physics2D.BoxCastNonAlloc(
            transform.position + new Vector3(groundCheck.x * transform.localScale.x, groundCheck.y, 0f),
            boxSize, 0, Vector2.up, hits, groundCheck.z, groundMask);
        if (hitResults > 0) _isGrounded = true;
        */
        if (_active)
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position + new Vector3(groundCheck.x * transform.localScale.x, groundCheck.y, 0f),
                boxSize, 0f, Vector2.up, groundCheck.z, groundMask);
            _isGrounded = false;
            if (hits.Length > 0)
            {
                foreach (RaycastHit2D hit in hits)
                {
                    if (Vector2.Angle(hit.normal, Vector2.up) < 20 &&
                        hit.point.y < transform.position.y - 1.2f)
                    {
                        _isGrounded = true;
                        break;
                    }
                }
            }
            
            if(!_canDoubleJump && _isGrounded) _canDoubleJump = _isGrounded;
            
            AnimationUpdate();
            
            SpendEnergy();
            Shooting();
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
        float moverV = Input.GetAxis("Vertical");
        float moverH = Input.GetAxis("Horizontal");

        Vector2 moviment = new Vector2(moverH, moverV);
        */
        
        //_rigidbody2D.AddForce(_playerMovement * velocidade);

        if (_active)
        {
            _rigidbody2D.velocity = new Vector2(_playerMovement.x * velocidade * Time.fixedDeltaTime, _rigidbody2D.velocity.y);
                    
            if(_isMovingRight && _playerMovement.x < 0) Flip();
            if(!_isMovingRight && _playerMovement.x > 0) Flip();
            
            
            Jump();
            Jetpack();
        }
        
    }

    private void OnActionTriggered(InputAction.CallbackContext obj)
    {
        if (_active)
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
                        if (_canJetpack)
                        {
                            _canJetpack = false;
                            _isJetpacking = true;
                            jetEffect.SetActive(true);
                        }
                        
                        if (_canDoubleJump)
                        {
                            _doJump = true;
                            _startJumpTime = Time.time;
                            _canDoubleJump = false;
                            
                            _canJetpack = true;
                        }
                    }
                }
    
                if (obj.canceled)
                {
                    _doJump = false;

                    if (_isJetpacking)
                    {
                        _isJetpacking = false;
                        jetEffect.SetActive(false);
                    }
                }
            }

            if (obj.action.name == _gameInput.Gameplay.Fire.name)
            {
                if (obj.performed)
                {
                    ShootBullet();
                }
            }
            
        }
        else
        {
            if (_dead) 
            {
                if (obj.action.name == _gameInput.Gameplay.Jump.name)
                {
                    if(obj.performed) GameManager.Instance.ProcessDeath();
                }
            }
            else
            {
                if (obj.action.name == _gameInput.Gameplay.Jump.name)
                {
                    if(obj.performed) GameManager.Instance.LoadNextLevel();
                }
            }
        }
        
    }

    private void Jump()
    {
        if (_doJump)
        {
            //_rigidbody2D.AddForce(Vector2.up * jumpForce);

            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpForce * Time.fixedDeltaTime);
            
            if(Time.time - _startJumpTime > jumpTime) _doJump = false;
        }
    }

    private void Jetpack()
    {
        if (_isJetpacking)
        {
            _rigidbody2D.AddForce(Vector2.up * jetpackForce * Time.fixedDeltaTime);
            if (_isGrounded)
            {
                _isJetpacking = false;
                jetEffect.SetActive(false);
            }
        }
    }

    private void ShootBullet()
    {
        if (!_isShooting && !_shotMade)
        {
            _animator.SetTrigger("ShootBullet");
            _isShooting = true;
        }
    }

    private void Shooting()
    {
        if (_isShooting)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("JumpShoot") ||
                _animator.GetCurrentAnimatorStateInfo(0).IsName("IdleShoot") ||
                _animator.GetCurrentAnimatorStateInfo(0).IsName("WalkShoot"))
            {
                GameObject newBullet = Instantiate(bulletPrefab, shootOrigin.transform.position, Quaternion.identity);
                newBullet.transform.localScale = transform.localScale;

                GameObject muzzle = Instantiate(muzzlePrefab, shootOrigin.transform.position, Quaternion.identity);
                _isShooting = false;
                _shotMade = true;
            }
        } else if (_shotMade)
        {
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("JumpShoot") &&
                !_animator.GetCurrentAnimatorStateInfo(0).IsName("IdleShoot") &&
                !_animator.GetCurrentAnimatorStateInfo(0).IsName("WalkShoot"))
            {
                _shotMade = false;
            }
        }
    }
    
    private void Flip()
    {
        _isMovingRight = !_isMovingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1,
            transform.localScale.y, transform.localScale.z);
    }

    private void AnimationUpdate()
    {
        _animator.SetFloat("Speed", Mathf.Abs(_playerMovement.x));
        _animator.SetBool("IsGrounded", _isGrounded);
        _animator.SetFloat("VertSpeed", _rigidbody2D.velocity.y);
    }

    private void KillPlayer()
    {
        _active = false;
        _dead = true;

        _rigidbody2D.bodyType = RigidbodyType2D.Static;
        
        _animator.SetBool("Active", _active);
        _animator.Play("Dead");
        
        HUDObserverManager.PlayerDeath(true);
        jetEffect.SetActive(false);
    }

    private void PlayerVictory()
    {
        _active = false;
        
        _rigidbody2D.velocity = Vector2.zero;
        _playerMovement = Vector2.zero;
        
        _animator.SetBool("Active", _active);
        _animator.Play("Victory");

        HUDObserverManager.PlayerVictory(true);
        jetEffect.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Kill"))
        {
            if(other.contacts.Any(contact => Vector2.Angle(contact.normal, Vector2.up) < 20))
                KillPlayer();
        }

        if (other.gameObject.CompareTag("TakeEnergy"))
        {
            _currentEnergy -= energyBitRecover;
            HUDObserverManager.PlayerEnergyChangedChannel(_currentEnergy);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Victory"))
        {
            PlayerVictory();
        }

        if (other.CompareTag("EnergyBit"))
        {
            _currentEnergy += energyBitRecover;
            HUDObserverManager.PlayerEnergyChangedChannel(_currentEnergy);
            
            Destroy(other.gameObject);
        }

        if (other.CompareTag("ExtraLife"))
        {
            GameManager.Instance.AddLife(1);
            
            Destroy(other.gameObject);
        }
    }

    private void SpendEnergy()
    {
        _currentEnergy -= spendRate * Time.deltaTime;

        if (_currentEnergy < 0)
        {
            _currentEnergy = 0;
            KillPlayer();
        }
        if (_currentEnergy > maxEnergy) _currentEnergy = maxEnergy;
        
        HUDObserverManager.PlayerEnergyChangedChannel(_currentEnergy);
    }

    private void OnDrawGizmos()
    {
        //Debug.DrawLine(transform.position, transform.position + groundCheck, Color.red);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position + new Vector3(groundCheck.x * transform.localScale.x, groundCheck.y, 0f), boxSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position + new Vector3(groundCheck.x * transform.localScale.x, groundCheck.y + groundCheck.z, 0f), boxSize);
    }
}
