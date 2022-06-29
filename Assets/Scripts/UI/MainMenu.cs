using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public TMP_Text starsText;

    private void OnEnable()
    {
        starsText.text = GameManager.instance.playerPrefsManager.GetNumberOfStars().ToString();
    }
}
