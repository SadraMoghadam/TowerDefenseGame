using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting : MonoBehaviour
{
    public bool DebugMode = true;
    public bool drawProjectionLine = true;
    public bool slowMotionOnExplosion = true;
    public bool miniMap = true;
    public int cameraPosition = 0;
    public float music = 0.3f;
    public float sfx = 0.5f;
    private PlayerPrefsManager playerPrefsManager;

    private void Awake()
    {
        playerPrefsManager = GameManager.instance.playerPrefsManager;
        drawProjectionLine =
            playerPrefsManager.GetBool(PlayerPrefsManager.PlayerPrefsKeys.drawProjectionLine, drawProjectionLine);
        slowMotionOnExplosion =
            playerPrefsManager.GetBool(PlayerPrefsManager.PlayerPrefsKeys.slowMotionOnExplosion, slowMotionOnExplosion);
        miniMap = playerPrefsManager.GetBool(PlayerPrefsManager.PlayerPrefsKeys.miniMap, miniMap);
        cameraPosition = playerPrefsManager.GetInt(PlayerPrefsManager.PlayerPrefsKeys.cameraPosition, cameraPosition);
        music = playerPrefsManager.GetFloat(PlayerPrefsManager.PlayerPrefsKeys.music, music);
        sfx = playerPrefsManager.GetFloat(PlayerPrefsManager.PlayerPrefsKeys.sfx, sfx);
    }
}
