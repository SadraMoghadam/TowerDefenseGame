using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    public Toggle projectionLine;
    public Toggle slowMotion;
    public Toggle miniMap;
    public TMP_Dropdown cameraPosition;
    public Slider music;
    public Slider sfx;
    public Button quit;
    private GameManager gameManager;
    private GameController gameController;

    private void Awake()
    {
        gameManager = GameManager.instance;
        gameController = GameController.instance;
        projectionLine.isOn = gameManager.gameSetting.drawProjectionLine;
        slowMotion.isOn = gameManager.gameSetting.slowMotionOnExplosion;
        cameraPosition.value = gameManager.gameSetting.cameraPosition;
        miniMap.isOn = gameManager.gameSetting.miniMap;
        music.value = gameManager.gameSetting.music;
        sfx.value = gameManager.gameSetting.sfx;
        quit.onClick.AddListener((() =>
        {
            gameObject.SetActive(false);
            Time.timeScale = 1;
            GameManager.instance.LoadScene("MainMenu");
        }));
    }

    private void OnEnable()
    {
        Time.timeScale = 1;
        
        projectionLine.onValueChanged.AddListener((value) =>
        {
            gameManager.playerPrefsManager.SetBool(PlayerPrefsManager.PlayerPrefsKeys.drawProjectionLine, value);
            gameManager.gameSetting.drawProjectionLine = value;
        });
        slowMotion.onValueChanged.AddListener((value) =>
        {
            gameManager.playerPrefsManager.SetBool(PlayerPrefsManager.PlayerPrefsKeys.slowMotionOnExplosion, value);
            gameManager.gameSetting.slowMotionOnExplosion = value;
        });
        miniMap.onValueChanged.AddListener((value) =>
        {
            gameManager.playerPrefsManager.SetBool(PlayerPrefsManager.PlayerPrefsKeys.miniMap, value);
            GameUIController.instance.miniMap.SetActive(value);
            gameManager.gameSetting.miniMap = value;
        });
        cameraPosition.onValueChanged.AddListener((value) =>
        {
            var weaponType = GameManager.instance.playerPrefsManager.GetCurrentWeaponType();
            if (weaponType == Weapon.WeaponType.Launcher)
            {
                gameManager.playerPrefsManager.SetInt(PlayerPrefsManager.PlayerPrefsKeys.cameraPosition, value);
                gameManager.gameSetting.cameraPosition = value;
                Vector3 cameraTransform = gameController.weapon.mainCamera.transform.localPosition;
                float cameraZ = 0;
                if (value == 1)
                {
                    cameraZ = 3;
                }
                else if (value == 2)
                {
                    cameraZ = -3;
                }
                gameController.weapon.mainCamera.transform.localPosition = new Vector3(cameraTransform.x, cameraTransform.y, cameraZ);
            }
        });
        music.onValueChanged.AddListener((value) =>
        {
            gameManager.playerPrefsManager.SetFloat(PlayerPrefsManager.PlayerPrefsKeys.music, value);
            gameManager.gameSetting.music = value;
            if (MainMenuUIController.instance != null)
            {
                gameManager.audioController.SetMusicVolume(MainMenuUIController.instance.audioSource, value);   
            }

            if (GameController.instance != null)
            {
                gameManager.audioController.SetMusicVolume(GameController.instance.audioSource, value);
            }
        });
        sfx.onValueChanged.AddListener((value) =>
        {
            gameManager.playerPrefsManager.SetFloat(PlayerPrefsManager.PlayerPrefsKeys.sfx, value);
            gameManager.gameSetting.sfx = value;
        });
        Invoke("StopTime", .5f);
    }

    private void StopTime()
    {
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }
}
