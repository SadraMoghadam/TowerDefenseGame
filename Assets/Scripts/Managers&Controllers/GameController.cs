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
    [HideInInspector] public int level = 1;
    [HideInInspector] public int stars = 2;
    [HideInInspector] public int numberOfEnemiesAlive;
    [HideInInspector] public LevelData levelData;
    private GameManager gameManager;
    public static GameController instance;

    [HideInInspector] public float matchLength = 120;
    // [SerializeField] private Camera camera;
    // private int cameraFieldOfViewCoefficient = 3;

    public enum EnemyTypes
    {
        Default = 0,
        Dasher = 1,
        Strong = 2,
        Boss = 3
    }

    private void Awake()
    {
        matchLength = 120;
        if (SceneManager.GetActiveScene().name != "Game")
            enabled = false;
        else
            enabled = true;
        instance = this;
        gameManager = GameManager.instance;
        level = gameManager.playerPrefsManager.GetInt(PlayerPrefsManager.PlayerPrefsKeys.ChosenLevel, 1);
        // if (gameManager.redirectFromMainMenu)
        // {
        //     level = gameManager.playerPrefsManager.GetInt(PlayerPrefsManager.PlayerPrefsKeys.ChosenLevel, 1);
        // }
        // else
        // {
        //     level = gameManager.playerPrefsManager.GetLevelsCompleted() != null &&
        //             gameManager.playerPrefsManager.GetLevelsCompleted().Count > 0
        //         ? gameManager.playerPrefsManager.GetLevelsCompleted().Max() + 1
        //         : 1;   
        // }
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
        stars = 3;
        if (!GameManager.instance.playerPrefsManager.GetComletedLevelNumbers().Contains(level))
        {
            int preStars = GameManager.instance.playerPrefsManager.GetNumberOfStars();
            GameManager.instance.playerPrefsManager.SetNumberOfStars(preStars + stars);
        }
        GameManager.instance.playerPrefsManager.AddLevelsCompleted(level, stars);
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
