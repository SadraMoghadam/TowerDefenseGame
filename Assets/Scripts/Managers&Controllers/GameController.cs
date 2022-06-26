using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject launcher;
    public List<GameObject> walls;
    public int level;
    [HideInInspector] public int numberOfEnemiesAlive;
    [HideInInspector] public LevelDataReader levelDataReader;
    [HideInInspector] public LevelData levelData;


    private void Awake()
    {
        levelDataReader = gameObject.GetComponent<LevelDataReader>();
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
