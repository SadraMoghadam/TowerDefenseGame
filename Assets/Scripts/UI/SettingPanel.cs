using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    public Toggle projectionLine;
    public Toggle slowMotion;
    public Toggle miniMap;
    public Slider music;
    public Slider sfx;
    public Button quit;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.instance;
        projectionLine.isOn = gameManager.gameSetting.drawProjectionLine;
        slowMotion.isOn = gameManager.gameSetting.slowMotionOnExplosion;
        miniMap.isOn = gameManager.gameSetting.miniMap;
        music.value = gameManager.gameSetting.music;
        sfx.value = gameManager.gameSetting.sfx;
        quit.onClick.AddListener((() => Debug.Log("Quit To Menu")));
    }

    private void OnEnable()
    {
        projectionLine.onValueChanged.AddListener((value) => gameManager.gameSetting.drawProjectionLine = value);
        slowMotion.onValueChanged.AddListener((value) => gameManager.gameSetting.slowMotionOnExplosion = value);
        miniMap.onValueChanged.AddListener((value) =>
        {
            GameUIController.instance.miniMap.SetActive(value);
            gameManager.gameSetting.miniMap = value;
        });
        music.onValueChanged.AddListener((value) => gameManager.gameSetting.music = value);
        sfx.onValueChanged.AddListener((value) => gameManager.gameSetting.sfx = value);
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
