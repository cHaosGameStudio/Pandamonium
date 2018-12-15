﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static bool joystick = true;

    private static GameManager instance;

    [HideInInspector]
    public AttackingCharacter playerInstance;

    private static int NUM_OF_LEVELS = 3;
    private static int FIRST_LEVEL_BUILD_INDEX = 4;

    public int currentLevel = 0;

    [HideInInspector]
    public string gameMode = "classic";
    [HideInInspector]
    public bool isRunStarted = false;

    [HideInInspector]
    public int coins = 0;

    [HideInInspector]
    public List<string> abilities;

    public static GameManager I
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
        {

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
            return;
        }

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        SaveManager.I.LoadGame();

    }

    private void Start()
    {
        if (SaveManager.I.gameState != null)
        {
            isRunStarted = SaveManager.I.gameState.isRunStarted;
            gameMode = SaveManager.I.gameState.gameMode;
            currentLevel = SaveManager.I.gameState.gameLevel;
            coins = SaveManager.I.gameState.coins;
            abilities = SaveManager.I.gameState.abilities;
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if(UIManager.I && UIManager.I.coinsText)
        {
            if (SaveManager.I.gameState != null)
            {
                coins = SaveManager.I.gameState.coins;
                UIManager.I.coinsText.text = coins.ToString();
            }
        }
    }

    public void PickupCoins(int amount)
    {
        coins += amount;
        UIManager.I.coinsText.text = coins.ToString();
    }

    public void OnDeath()
    {
        coins = 0;
    }

    public void GameOver()
    {
        isRunStarted = false;
        SaveManager.I.SaveGame();
        LoadScene("MainMenu");
    }

    public void LoadNextLevel()
    {
        currentLevel++;

        if (currentLevel >= NUM_OF_LEVELS) {

            if (gameMode.Equals("classic"))
            {
                currentLevel = 0;
                GameOver();
                return;
            }
        }
        

        SaveManager.I.SaveGame();

        // u build settings mora da bude game level za game levelom, redom
        string pathToScene = SceneUtility.GetScenePathByBuildIndex(FIRST_LEVEL_BUILD_INDEX + Mathf.Clamp(currentLevel, 0, NUM_OF_LEVELS - 1));
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(pathToScene);
        LoadSceneLong(sceneName);
        //print(SceneManager.GetSceneByBuildIndex(FIRST_LEVEL_BUILD_INDEX + currentLevel).name);
    }

    public void SetJoystick(bool joystick)
    {
        GameManager.joystick = joystick;
    }

    public string nextScene;

    public void ChooseGameMode(string gameMode)
    {
        this.gameMode = gameMode;
        LoadScene("CharacterSelection");
    }

    public void OnPlayPressed()
    {
        if(isRunStarted)
        {
            StartGame(currentLevel);
        }
        else
        {
            LoadScene("ModeChoiceScene");
        }
    }

    public void StartGame(int level)
    {
        isRunStarted = true;
        currentLevel = level;

        string pathToScene = SceneUtility.GetScenePathByBuildIndex(FIRST_LEVEL_BUILD_INDEX + Mathf.Clamp(currentLevel, 0, NUM_OF_LEVELS - 1));
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(pathToScene);
        LoadSceneLong(sceneName);
    }

    // ovo nadalje smestiti u scene manager neki

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void LoadSceneLong(string scene)
    {

        nextScene = scene;
        StartCoroutine(LoadAsyncScene("LoadingScene"));
        
    }

    void OnAsyncLoad(string loadedScene)
    {
        if (loadedScene.Equals("LoadingScene"))
        {
            StartCoroutine(LoadAsyncScene(nextScene));
        }
    }

    private void OnApplicationQuit()
    {

        if (SaveManager.I != null)
        {
            if(playerInstance == null)
            {
                SaveManager.I.SaveGame(abilities);
            }
            else
            {
                SaveManager.I.SaveGame();
            }
        }

    }

    IEnumerator LoadAsyncScene(string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
        Slider slider = FindObjectOfType<Slider>();

        while (!asyncLoad.isDone)
        {

            float progress = Mathf.Clamp01(asyncLoad.progress / .9f);
            if(slider)
                slider.value = progress;
            yield return null;
        }

        OnAsyncLoad(scene);
    }
}
