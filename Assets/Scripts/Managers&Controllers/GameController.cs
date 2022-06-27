using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject launcher;
    public List<GameObject> walls;
    public int level;
    [HideInInspector] public int numberOfEnemiesAlive;
    [HideInInspector] public LevelDataReader levelDataReader;
    [HideInInspector] public LevelData levelData;
    private GameManager gameManager;


    private void Awake()
    {
        gameManager = GameManager.instance;
        levelDataReader = gameObject.GetComponent<LevelDataReader>();
        level = gameManager.PlayerPrefsManager.GetLevelsCompleted() != null &&
                gameManager.PlayerPrefsManager.GetLevelsCompleted().Count > 0
            ? gameManager.PlayerPrefsManager.GetLevelsCompleted().Max() + 1
            : 1;
        Debug.Log("Level: " + level);
    }

    private void OnEnable()
    {
        GetLevelInformation();
    }

    private void GetLevelInformation()
    {
        levelData = levelDataReader.GetLevelData(level);
        numberOfEnemiesAlive = levelData.numberOfEnemies;
    }

    public void LostProcess()
    {
        GameUIController.instance.endOfGamePanel.EOGPanelShow(false);
    }
    
    public void WonProcess()
    {
        GameManager.instance.PlayerPrefsManager.AddLevelsCompleted(level);
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
