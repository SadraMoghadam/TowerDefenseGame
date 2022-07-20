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
    public Button fire;
    public Button reload;
    public Button changeWeapon;
    public TMP_Text ammoInfo;
    public TMP_Text FPS;
    public Button settingsButton;
    public Joystick joystick;
    public GameObject miniMap;
    public EndOfGamePanel endOfGamePanel;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private List<GameObject> CountdownNumbers;
    [SerializeField] private SettingPanel settingPanel;
    [SerializeField] private TMP_Text blastPowerSliderValue;
    [SerializeField] private Animator changeWeaponAnimation;
    [SerializeField] private TMP_Text changeWeaponText;
    [HideInInspector] private float timer;
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public bool isHeldDown = false;

    public static GameUIController instance;
    [HideInInspector] public AmmoController ammoController;
    private float fpsTimer, fpsRefresh, avgFramerate;
    private GameManager gameManager;
    private GameController gameController;
    private WeaponController weaponController;

    private void Awake()
    {
        instance = this;
        ammoController = GetComponent<AmmoController>();
        audioSource = GetComponent<AudioSource>();
        gameManager = GameManager.instance;
        gameController = GameController.instance;
    }

    private void Start()
    {
        blastPowerSlider.gameObject.SetActive(false);
        timer = gameController.matchLength;
        var timeSpan = TimeSpan.FromSeconds(timer);
        timerText.text = $"{timeSpan.Minutes.ToString("00")}:{timeSpan.Seconds.ToString("00")}"; 
        weaponController = gameController.weapon;
        if (weaponController.currentWeaponType == Weapon.WeaponType.Launcher)
        {
            LauncherUIBehaviour();
        }
        changeWeaponText.text = gameController.weapon.currentWeaponType == gameController.weapon.weaponType1 ? 
            gameController.weapon.weaponType2.ToString() : gameController.weapon.weaponType1.ToString();
        StartCoroutine(StartCountdown(0));
        
        settingsButton.onClick.AddListener((() => settingPanel.gameObject.SetActive(true)));
        reload.onClick.AddListener((() => ammoController.Reload()));
        changeWeapon.onClick.AddListener((() =>
        {
            StartCoroutine(ChangeWeaponProcess());
        }));
        
    }

    private void Update()
    {
        if (gameManager.gameSetting.DebugMode)
        {
            float timeLapse = Time.smoothDeltaTime;
            fpsTimer = fpsTimer <= 0 ? fpsRefresh : fpsTimer - timeLapse;
            if (fpsTimer <= 0)
            {
                avgFramerate = (int)(1f / timeLapse);
            }

            FPS.text = avgFramerate.ToString() + "fps";
        }
        
        if (weaponController.currentWeaponType == Weapon.WeaponType.Turret)
        {
            TurretController turretController = weaponController.GetWeapon().GetComponent<TurretController>();
            if (isHeldDown && gameController.weapon.ableToShot)
            {
                turretController.Shot();
            }
        }
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
        if (countdownNumber == 0)
        {
            gameManager.audioController.PlaySfx(audioSource, AudioController.SFXType.StartGame);
        }
        if(countdownNumber < 3)
            StartCoroutine(StartCountdown(++countdownNumber));
        else
        {
            EnableTouchableButtons();
            StartCoroutine(StartTimer());
        }
    }

    private IEnumerator ChangeWeaponProcess()
    {
        changeWeaponAnimation.gameObject.SetActive(true);
        changeWeaponText.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        changeWeaponAnimation.gameObject.SetActive(false);
        changeWeaponText.gameObject.SetActive(true);
        weaponController.ChangeWeapon();
        fire.onClick.RemoveAllListeners();
        blastPowerSlider.gameObject.SetActive(false);
        changeWeaponText.text = gameController.weapon.currentWeaponType == gameController.weapon.weaponType1 ? 
            gameController.weapon.weaponType2.ToString() : gameController.weapon.weaponType1.ToString();
        // changeWeaponText.text = gameController.weapon.currentWeaponType.ToString();
        if (weaponController.currentWeaponType == Weapon.WeaponType.Launcher)
        {
            LauncherUIBehaviour();
        }
        ammoController.SetAmmoByType();
    }
    
    private void LauncherUIBehaviour()
    {
        blastPowerSlider.gameObject.SetActive(true);
        LauncherController launcherController = weaponController.GetWeapon().GetComponent<LauncherController>();
        fire.onClick.AddListener(() =>
        {
            if(gameController.weapon.ableToShot)
                launcherController.Shot();
        });
        launcherController.blastPower = blastPowerSlider.value;
        blastPowerSlider.onValueChanged.AddListener((value) =>
        {
            launcherController.blastPower = value;
            blastPowerSliderValue.text = value.ToString();
        });
        blastPowerSlider.value = 17;
    }

    public void DisableTouchableButtons()
    {
        fire.interactable = false;
        reload.interactable = false;
        settingsButton.interactable = false;
        changeWeapon.interactable = false;
        joystick.enabled = false;
        blastPowerSlider.enabled = false;
    }
    
    
    public void EnableTouchableButtons()
    {
        fire.interactable = true;
        reload.interactable = true;
        settingsButton.interactable = true;
        changeWeapon.interactable = true;
        joystick.enabled = true;
        blastPowerSlider.enabled = true;
    }

    private IEnumerator StartTimer()
    {
        var timeSpan = TimeSpan.FromSeconds(timer);
        timerText.text = $"{timeSpan.Minutes.ToString("00")}:{timeSpan.Seconds.ToString("00")}";
        yield return new WaitForSeconds(1f);
        timer--;
        if (!gameController.endOfGame)
        {
            StartCoroutine(StartTimer());   
        }

        if (timer == 0)
        {
            gameController.WonProcess(true);
        }
    }
 
    public void onPress ()
    {
        if (weaponController.currentWeaponType == Weapon.WeaponType.Launcher)
            return;
        isHeldDown = true;
        // Debug.Log(isHeldDown);
    }
 
    public void onRelease ()
    {
        if (weaponController.currentWeaponType == Weapon.WeaponType.Launcher)
            return;
        isHeldDown = false;
        // Debug.Log(isHeldDown);
    }
    
}
