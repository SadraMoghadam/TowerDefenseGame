using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private List<GameObject> CountdownNumbers;
    [SerializeField] private SettingPanel settingPanel;
    [SerializeField] private TMP_Text blastPowerSliderValue;
    [HideInInspector] private float timer;

    public static GameUIController instance;
    [HideInInspector] public AmmoController ammoController;

    private void Awake()
    {
        instance = this;
        ammoController = GetComponent<AmmoController>();
    }

    private void Start()
    {
        timer = GameController.instance.matchLength;
        var timeSpan = TimeSpan.FromSeconds(timer);
        timerText.text = $"{timeSpan.Minutes.ToString("00")}:{timeSpan.Seconds.ToString("00")}";
        LauncherController launcherController = GameController.instance.launcher;
        StartCoroutine(StartCountdown(0));
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

    private IEnumerator StartCountdown(int countdownNumber)
    {
        if(countdownNumber == 0)
            DisableTouchableButtons();
        foreach (var countdownNumberObject in CountdownNumbers)
        {
            countdownNumberObject.SetActive(false);
        }
        CountdownNumbers[countdownNumber].SetActive(true);
        yield return new WaitForSeconds(1f);
        if(countdownNumber < 3)
            StartCoroutine(StartCountdown(++countdownNumber));
        else
        {
            EnableTouchableButtons();
            StartCoroutine(StartTimer());
        }
    }

    public void DisableTouchableButtons()
    {
        launch.interactable = false;
        reload.interactable = false;
        settingsButton.interactable = false;
        joystick.enabled = false;
        blastPowerSlider.enabled = false;
    }
    
    
    public void EnableTouchableButtons()
    {
        launch.interactable = true;
        reload.interactable = true;
        settingsButton.interactable = true;
        joystick.enabled = true;
        blastPowerSlider.enabled = true;
    }

    private IEnumerator StartTimer()
    {
        var timeSpan = TimeSpan.FromSeconds(timer);
        timerText.text = $"{timeSpan.Minutes.ToString("00")}:{timeSpan.Seconds.ToString("00")}";
        yield return new WaitForSeconds(1f);
        timer--;
        if (!GameController.instance.endOfGame)
        {
            StartCoroutine(StartTimer());   
        }

        if (timer == 0)
        {
            GameController.instance.WonProcess(true);
        }
    }
    
}
