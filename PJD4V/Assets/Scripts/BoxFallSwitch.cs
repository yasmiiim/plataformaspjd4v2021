using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxFallSwitch : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _boxRB2D;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _boxRB2D.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}
