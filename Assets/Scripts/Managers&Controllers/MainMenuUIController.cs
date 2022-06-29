using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIController : MonoBehaviour
{
    public static MainMenuUIController instance;
    public LevelsPanel levelsPanel; 
    public MainMenu mainMenu;
    public SettingPanel settingPanel;

    private void Awake()
    {
        instance = this;
    }
    
    
    
}
