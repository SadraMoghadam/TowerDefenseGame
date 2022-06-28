using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject launcher;
    public List<GameObject> walls;
    public int level;
    [HideInInspector] public int numberOfEnemiesAlive;
    [HideInInspector] public LevelData levelData;
    private GameManager gameManager;
    public static GameController instance;


    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != "Game")
            enabled = false;
        else
            enabled = true;
        instance = this;
        gameManager = GameManager.instance;
        if (gameManager.redirectFromMainMenu)
        {
            level = gameManager.playerPrefsManager.GetInt(PlayerPrefsManager.PlayerPrefsKeys.ChosenLevel, 1);
        }
        else
        {
            level = gameManager.playerPrefsManager.GetLevelsCompleted() != null &&
                    gameManager.playerPrefsManager.GetLevelsCompleted().Count > 0
                ? gameManager.playerPrefsManager.GetLevelsCompleted().Max() + 1
                : 1;   
        }
        Debug.Log("Level: " + level);
    }

    private void OnEnable()
    {
        GetLevelInformation();
    }

    private void GetLevelInformation()
    {
        levelData = gameManager.levelDataReader.GetLevelData(level);
        numberOfEnemiesAlive = levelData.numberOfEnemies;
    }

    public void LostProcess()
    {
        GameUIController.instance.endOfGamePanel.EOGPanelShow(false);
    }
    
    public void WonProcess()
    {
        GameManager.instance.playerPrefsManager.AddLevelsCompleted(level);
        GameUIController.instance.endOfGamePanel.EOGPanelShow(true);
    }
    
    public IEnumerator SlowMotion(float slowMotionCoefficient, float slowMotionTime)
    {
        Time.timeScale = slowMotionCoefficient;
        yield return new WaitForSeconds(slowMotionTime);
        Time.timeScale = 1;
        StopAllCoroutines();
    }
    
}
