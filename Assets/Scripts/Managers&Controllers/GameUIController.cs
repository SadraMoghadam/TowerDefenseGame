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
    public Button settingsButton;
    public Joystick joystick;
    public GameObject miniMap;
    [SerializeField] private SettingPanel settingPanel;
    [SerializeField] private TMP_Text blastPowerSliderValue;

    public static GameUIController instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        LauncherController launcherController = GameManager.instance.gameController.launcher.GetComponent<LauncherController>();
        launch.onClick.AddListener(launcherController.Shot);
        settingsButton.onClick.AddListener((() => settingPanel.gameObject.SetActive(true)));
        blastPowerSlider.value = 17;
        launcherController.blastPower = blastPowerSlider.value;
        blastPowerSlider.onValueChanged.AddListener((value) =>
        {
            launcherController.blastPower = value;
            blastPowerSliderValue.text = value.ToString();
        });
    }
}
