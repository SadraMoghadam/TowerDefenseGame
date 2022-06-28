using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public Slider blastPowerSlider;
    public Button launch;
    public Button reload;
    public TMP_Text ammoInfo;
    public Button settingsButton;
    public Joystick joystick;
    public GameObject miniMap;
    public EndOfGamePanel endOfGamePanel;
    [SerializeField] private SettingPanel settingPanel;
    [SerializeField] private TMP_Text blastPowerSliderValue;

    public static GameUIController instance;
    [HideInInspector] public AmmoController ammoController;

    private void Awake()
    {
        instance = this;
        ammoController = GetComponent<AmmoController>();
    }

    private void Start()
    {
        LauncherController launcherController = GameController.instance.launcher.GetComponent<LauncherController>();
        launch.onClick.AddListener(launcherController.Shot);
        settingsButton.onClick.AddListener((() => settingPanel.gameObject.SetActive(true)));
        reload.onClick.AddListener((() => ammoController.Reload()));
        blastPowerSlider.value = 17;
        launcherController.blastPower = blastPowerSlider.value;
        blastPowerSlider.onValueChanged.AddListener((value) =>
        {
            launcherController.blastPower = value;
            blastPowerSliderValue.text = value.ToString();
        });
    }
}
