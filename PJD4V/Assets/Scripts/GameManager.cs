using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int Lives;

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

    private void Start()
    {
        LoadMainMenu();
        
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && Lives <0)
        {
            LoadNextLevel();
        }
    }

    public void ProcessDeath()
    {
        Lives--;
        HUDObserverManager.LivesChangedChannel(Lives);
        HUDObserverManager.PlayerDeath(false);
        
        if (Lives < 0)
        {
            LoadGameOver();
        }
        else
        {
            ResetCurrentLevel();
        }
    }

    public void LoadNextLevel()
    {
        if (Lives >= 0)
        {
            HUDObserverManager.PlayerVictory(false);
            if(SceneManager.GetActiveScene().name == "Level1") LoadLevel2();
            if(SceneManager.GetActiveScene().name == "Level2") LoadLevel1();
        }
        else
        {
            if (SceneManager.GetActiveScene().name == "GameOver")
            {
               LoadMainMenu();
               // Lives = 3;
               // HUDObserverManager.LivesChangedChannel(Lives);
            }
        }
        
    }

    public void ResetCurrentLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1");
    }
    
    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void LoadGameOver()
    {
        HUDObserverManager.ActivateHUD(false);
        SceneManager.LoadScene("GameOver");
    }

    public void AddLife(int value)
    {
        Lives += value;
        HUDObserverManager.LivesChangedChannel(Lives);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync("Initialize").completed += (op) => { SceneManager.LoadScene("MainMenu"); };
       // SceneManager.LoadScene("MainMenu");
    }

    public void InitializeGame()
    {
        HUDObserverManager.ActivateHUD(true);
        HUDObserverManager.LivesChangedChannel(Lives);
        Lives = 3;
        HUDObserverManager.LivesChangedChannel(Lives);
    }
}
