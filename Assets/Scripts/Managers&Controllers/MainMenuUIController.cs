using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIController : MonoBehaviour
{
    public static MainMenuUIController instance;
    public LevelsPanel levelsPanel; 

    private void Awake()
    {
        instance = this;
    }
    
    
    
}
