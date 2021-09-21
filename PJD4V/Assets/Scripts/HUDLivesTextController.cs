using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDLivesTextController : MonoBehaviour
{

    private TMP_Text _text;
    
    private void OnEnable()
    {
        HUDObserverManager.ONLivesChangedChannel += OnLivesChangedChannel;
    }

    private void OnDisable()
    {
        HUDObserverManager.ONLivesChangedChannel -= OnLivesChangedChannel;
    }

    private void Start()
    {
        _text = GetComponent<TMP_Text>();
    }

    private void OnLivesChangedChannel(int obj)
    {
        _text.text = obj.ToString();
        _text.color = obj < 0 ? Color.clear : Color.white;
    }
}
