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
    public AudioSource canvasAudioSource;
    public Weapons weapons;
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public Weapon currentWeapon;

    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        SetCurrentWeapon();
    }

    public void SetCurrentWeapon()
    {
        var currentWeaponType = GameManager.instance.playerPrefsManager.GetCurrentWeaponType();
        foreach (var weapon in weapons.weapons)
        {
            if (weapon.weaponType == currentWeaponType)
            {
                currentWeapon = weapon;
                break;
            }
        }
    }

    private void Start()
    {
        GameManager.instance.audioController.PlayMusic(audioSource, AudioController.MusicType.Background, true);
    }

    public void OnButtonClickSFX()
    {
        GameManager.instance.audioController.PlaySfx(canvasAudioSource, AudioController.SFXType.Button);
    }
}
