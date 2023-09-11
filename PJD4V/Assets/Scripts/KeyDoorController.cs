using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoorController : MonoBehaviour
{

    public void UseKey()
    {
        GetComponent<Animator>().Play("Opening");
        GetComponent<AudioSource>().Play();
    }
}
