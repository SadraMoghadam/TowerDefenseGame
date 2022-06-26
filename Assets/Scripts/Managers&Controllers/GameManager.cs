using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public GameController gameController;
    [HideInInspector] public GameSetting gameSetting;
    [HideInInspector] public Color GameMainColor = new Color(0, 99, 61);
    [HideInInspector] public Color gameRedColor = new Color(200, 24, 0);
    
    public static GameManager instance;
    private void Awake()
    {
        instance = this;
        gameController = GetComponent<GameController>();
        gameSetting = GetComponent<GameSetting>();
    }
}
