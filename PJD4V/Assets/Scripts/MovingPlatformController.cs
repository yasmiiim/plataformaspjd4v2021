using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MovingPlatformController : MonoBehaviour
{
    public float moveSpeed;
    
    [SerializeField] private Vector2 movePosition;

    private Vector2 _initialPosition;

    private Vector2 _currentMoveDirection;
    
    private bool _isReturning;
    
    // Start is called before the first frame update
    void Start()
    {
        _initialPosition = transform.position;
        _currentMoveDirection = (_initialPosition + movePosition - (Vector2) transform.position).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlatform();
    }

    private void MovePlatform()
    {
        if (!_isReturning)
        {
            Debug.Log(Vector2.Distance(transform.position, _initialPosition + movePosition));
            if (Vector2.Distance(transform.position, _initialPosition + movePosition) < 1f)
            {
                _isReturning = true;
                _currentMoveDirection = (_initialPosition - (Vector2) transform.position).normalized;
            }
        }
        else
        {
            if (Vector2.Distance(transform.position, _initialPosition) < 1f)
            {
                _isReturning = false;
                _currentMoveDirection = (_initialPosition + movePosition - (Vector2) transform.position).normalized;
            }
        }

        transform.position += (Vector3)_currentMoveDirection * moveSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }
    
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + (Vector3)movePosition, Color.red);
    }
}
