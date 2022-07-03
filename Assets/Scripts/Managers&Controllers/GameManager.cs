using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // [HideInInspector] public GameController gameController;
    [HideInInspector] public GameSetting gameSetting;
    [HideInInspector] public PlayerPrefsManager playerPrefsManager;
    [HideInInspector] public LevelDataReader levelDataReader;
    [HideInInspector] public AudioController audioController;
    [HideInInspector] public Color GameMainColor = new Color(0, 99, 61);
    [HideInInspector] public Color gameRedColor = new Color(200, 24, 0);
    [HideInInspector] public bool redirectFromMainMenu;
    
    public static GameManager instance;
    private void Awake()
    {
        if (instance != null) 
            Destroy(gameObject);
        GameManager[] gameManagers = FindObjectsOfType<GameManager>();
        if(gameManagers.Length > 1)
            for (int i = 0; i < gameManagers.Length - 1; i++)
            {
                Destroy(gameManagers[i].gameObject);   
            }
        // gameController = GetComponent<GameController>();
        levelDataReader = GetComponent<LevelDataReader>();
        gameSetting = GetComponent<GameSetting>();
        playerPrefsManager = GetComponent<PlayerPrefsManager>();
        audioController = GetComponent<AudioController>();
        DontDestroyOnLoad(this.gameObject);
        instance = this;
    }

    public async void LoadScene(string sceneName)
    {
        var scene = SceneManager.LoadSceneAsync(sceneName);
        // if (sceneName == "Game")
        // {
        //     gameController = FindObjectOfType<GameController>();
        // }
        SceneManager.LoadScene("Loading");
        scene.allowSceneActivation = false;
        await Task.Delay(200);
        var slider = FindObjectOfType<Slider>();
        do
        {
            await Task.Delay(100);
            slider.value = scene.progress;
        } while (scene.progress < 0.9f);

        await Task.Delay(1000);
        scene.allowSceneActivation = true;
        SceneManager.LoadScene(sceneName);
    }
    
}
