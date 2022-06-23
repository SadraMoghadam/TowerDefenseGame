using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public GameController gameController;
    [HideInInspector] public LevelManager LevelManager;
    
    public static GameManager instance;
    private void Awake()
    {
        instance = this;
        gameController = GetComponent<GameController>();
    }
}
