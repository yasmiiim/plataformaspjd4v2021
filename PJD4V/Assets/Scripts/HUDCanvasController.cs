using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDCanvasController : MonoBehaviour
{
    public static HUDCanvasController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
