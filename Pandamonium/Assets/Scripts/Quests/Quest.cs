﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Quest : MonoBehaviour {

    public enum QuestType { SLAY, COLLECT }

    [HideInInspector]
    public QuestType type;

    public int rewardCoins = 100;


    public int goal;

    public int progress;

    [HideInInspector]
    public int goalTemp;
    [HideInInspector]
    public int progressTemp;

    [HideInInspector]
    public bool loadedInfo = false;

    protected void Awake()
    {
        
    }

    protected virtual void Start()
    {
        goal = goalTemp;
        progress = progressTemp;
        loadedInfo = true;
    }

    public virtual void SetupDelegates()
    {

    }

    protected virtual void OnCompleted()
    {
        GameManager.I.coins += rewardCoins;

        InfoText.I.ShowMessage("Quest Complete");

        SaveManager.I.SaveGame();

        Destroy(this);
        
    }

    public abstract string GetParameter();
    public abstract void SetParameter(string param);
}
