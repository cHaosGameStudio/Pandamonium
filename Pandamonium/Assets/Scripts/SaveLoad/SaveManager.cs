﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveManager : MonoBehaviour {

    private string savePath;

    public GameState gameState = null;

    private static SaveManager instance;

    public static SaveManager I
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SaveManager>();
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

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        savePath = Application.persistentDataPath + "\\save.sav";

    }

    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(savePath);
        PlayerWithJoystick player = GameManager.I.playerInstance as PlayerWithJoystick;
        GameState game = new GameState(GameManager.I.currentLevel, player.coins, player.abilityManager.GetAbilities());
        gameState = game;
        bf.Serialize(file, game);
        file.Close();
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);
            GameState game = (GameState)bf.Deserialize(file);

            gameState = game;
           // (GameManager.I.playerInstance as PlayerWithJoystick).coins = game.coins;

            foreach(string ability in gameState.abilities)
            {
                print(ability);
            }

            file.Close();
        }
        else
        {
            gameState = null;
        }
    }

    [System.Serializable]
    public class GameState
    {

        public int gameLevel;
        public int coins;

        public List<string> abilities;

        public GameState(int gameLevel, int coins, List<string> abilities)
        {
            this.gameLevel = gameLevel;
            this.coins = coins;
            this.abilities = abilities;
        }

    }

}
