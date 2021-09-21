using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HUDObserverManager
{
    public static event Action<int> ONLivesChangedChannel;

    public static void LivesChangedChannel(int lives)
    {
        ONLivesChangedChannel?.Invoke(lives);
    }
}
