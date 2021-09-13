using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public float Lives;

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
            if(SceneManager.GetActiveScene().name == "Level1") LoadLevel2();
            if(SceneManager.GetActiveScene().name == "Level2") LoadLevel1();
        }
        else
        {
            if (SceneManager.GetActiveScene().name == "GameOver")
            {
                LoadLevel1();
                Lives = 3;
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
        SceneManager.LoadScene("GameOver");
    }
}
