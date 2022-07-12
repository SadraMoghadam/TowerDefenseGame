using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text starsText;
    [SerializeField] private Image weaponImage;

    private void OnEnable()
    {
        starsText.text = GameManager.instance.playerPrefsManager.GetNumberOfStars().ToString();
        weaponImage.sprite = MainMenuUIController.instance.currentWeapon.icon;
    }
}
