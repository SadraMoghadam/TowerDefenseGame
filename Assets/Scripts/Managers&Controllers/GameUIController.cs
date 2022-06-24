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
        blastPowerSlider.value = 17;
        blastPowerSlider.onValueChanged.AddListener((value) =>
        {
            launcherController.blastPower = value;
            blastPowerSliderValue.text = value.ToString();
        });
    }
}
