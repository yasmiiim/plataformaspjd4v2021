using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoorController : MonoBehaviour
{

    private bool _isOpen;
    
    public bool UseKey()
    {
        if (_isOpen) return false;
        
        GetComponent<Animator>().Play("Opening");
        GetComponent<AudioSource>().Play();
        _isOpen = true;
        return true;

    }
}
