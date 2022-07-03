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
    [HideInInspector] public AudioSource audioSource;

    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
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
