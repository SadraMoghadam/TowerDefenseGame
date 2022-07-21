using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public WeaponController weapon;
    public List<GameObject> walls;
    [HideInInspector] public int level = 1;
    [HideInInspector] public int stars = 2;
    [HideInInspector] public int numberOfEnemiesAlive;
    [HideInInspector] public LevelData levelData;
    private GameManager gameManager;
    public static GameController instance;
    [HideInInspector] public float matchLength = 180;
    [HideInInspector] public bool endOfGame = false;
    [HideInInspector] public AudioSource audioSource;
    private Camera camera;
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
        QualitySettings.vSyncCount = 0;
        // Application.targetFrameRate = 60;
        matchLength = 180;
        endOfGame = false;
        if (SceneManager.GetActiveScene().name != "Game")
            enabled = false;
        else
            enabled = true;
        instance = this;
        gameManager = GameManager.instance;
        audioSource = GetComponent<AudioSource>();
        level = gameManager.playerPrefsManager.GetInt(PlayerPrefsManager.PlayerPrefsKeys.ChosenLevel, 1);
        Debug.Log("Level: " + level);
    }

    private void Start()
    {
        GetLevelInformation();
        camera = weapon.mainCamera;
        SetCamera(3);
        gameManager.audioController.PlayMusic(audioSource, AudioController.MusicType.Wind, true);
    }

    private void GetLevelInformation()
    {
        levelData = gameManager.levelDataReader.GetLevelData(level);
        numberOfEnemiesAlive = levelData.numberOfEnemies;
    }

    public void LostProcess()
    {
        GameUIController.instance.endOfGamePanel.EOGPanelShow(false);
        endOfGame = true;
    }
    
    public void WonProcess(bool timeOut = false)
    {
        List<int> damages = WallsDamage();
        if (timeOut == true)
        {
            stars = 1;
        }
        else if (damages.Min() <= 20)
        {
            stars = 2;
        }
        else
        {
            stars = 3;
        }
        if (!GameManager.instance.playerPrefsManager.GetComletedLevelNumbers().Contains(level))
        {
            int preStars = GameManager.instance.playerPrefsManager.GetNumberOfStars();
            GameManager.instance.playerPrefsManager.SetNumberOfStars(preStars + stars);
        }
        GameManager.instance.playerPrefsManager.AddLevelsCompleted(level, stars);
        GameUIController.instance.endOfGamePanel.EOGPanelShow(true);
        endOfGame = true;
    }

    public List<int> WallsDamage()
    {
        List<int> damages = new List<int>();
        for (int i = 0; i < walls.Count; i++)
        {
            damages.Add((int)walls[i].GetComponent<WallController>().health);
        }

        return damages;
    }
    
    public IEnumerator SlowMotion(float slowMotionCoefficient, float slowMotionTime)
    {
        Time.timeScale = slowMotionCoefficient;
        yield return new WaitForSeconds(slowMotionTime);
        Time.timeScale = 1;
        StopAllCoroutines();
    }

    private void SetCamera(float cameraOffset)
    {
        var weaponType = GameManager.instance.playerPrefsManager.GetCurrentWeaponType();
        if (weaponType != Weapon.WeaponType.Launcher)
        {
            return;
        }

        int cameraPosition = gameManager.playerPrefsManager.GetInt(PlayerPrefsManager.PlayerPrefsKeys.cameraPosition, 0);
        Vector3 cameraTransform = camera.transform.localPosition;
        float cameraZ = 0;
        if (cameraPosition == 1)
        {
            cameraZ = cameraOffset;
        }
        else if (cameraPosition == 2)
        {
            cameraZ = -cameraOffset;
        }
        camera.transform.localPosition = new Vector3(cameraTransform.x, cameraTransform.y, cameraZ);
    }
    
}
