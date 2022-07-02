using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke("LoadMainMenu", 4f);   
    }

    private void LoadMainMenu()
    {
        GameManager.instance.LoadScene("MainMenu");
    }
}
