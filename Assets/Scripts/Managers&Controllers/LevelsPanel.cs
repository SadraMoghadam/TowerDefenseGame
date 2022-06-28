using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelsPanel : MonoBehaviour
{
    public GameObject levelsContainer;
    public GameObject levelButtonObject;
    public GameObject levelSelectPopUp;
    public TMP_Text levelText;
    public TMP_Text enemiesText;
    public TMP_Text ammoText;
    public TMP_Text difficultyText;
    public Button startGame;
    private List<LevelData> levelsData;
    private List<Button> levelButtons;
    

    private void OnEnable()
    {
        if(levelsContainer.transform.GetChildCount() > 0)
            return;
        levelsData = GameManager.instance.levelDataReader.levelData;
        List<int> levelsCompleted = GameManager.instance.playerPrefsManager.GetLevelsCompleted();
        int levels = levelsData.Count - 1;
        RectTransform objectRect = levelsContainer.GetComponent<RectTransform>();
        objectRect.sizeDelta = new Vector2(objectRect.sizeDelta.x, (float)levels * 117.5f);
        for (int i = 0; i < levels; i++)
        {
            var buttonObject = Instantiate(levelButtonObject);
            buttonObject.transform.parent = levelsContainer.transform;
            buttonObject.GetComponentInChildren<TMP_Text>().text = "Level " + (i + 1).ToString();
            if ((levelsCompleted == null || levelsCompleted.Count <= 0) && i == 0)
            {
                buttonObject.GetComponent<Button>().interactable = true;
            }
            else if (levelsCompleted.Max() >= i && i != levels - 1)
            {
                buttonObject.GetComponent<Button>().interactable = true;
            }
            else
            {
                buttonObject.GetComponent<Button>().interactable = false;
            }
        }
        levelButtons = levelsContainer.GetComponentsInChildren<Button>().ToList();
        foreach (var levelButton in levelButtons)
        {
            SelectButtonAnimation(levelButton);
        }
    }

    private void SelectButtonAnimation(Button button)
    {
        button.onClick.AddListener((() =>
        {
            foreach (var levelButton in levelButtons)
            {
                if (!levelButton.interactable || levelButton == button)
                {
                    continue;
                }
                levelButton.gameObject.GetComponent<Animator>().Play("Idle");
            }
            button.gameObject.GetComponent<Animator>().SetTrigger("Select");
            SetUpLevelPopUp(button);
        }));
    }

    private void SetUpLevelPopUp(Button button)
    {
        levelSelectPopUp.SetActive(true);
        int level = int.Parse(Regex.Match(button.GetComponentInChildren<TMP_Text>().text, @"\d+").Value);
        LevelData levelData = GameManager.instance.levelDataReader.GetLevelData(level);
        levelText.text = "Level : " + level;
        enemiesText.text = "Number Of Enemies : " + levelData.numberOfEnemies;
        ammoText.text = "Ammo : " + levelData.numberOfAmmos;
        difficultyText.text = "Difficulty : " + levelData.difficulty;
        GameManager.instance.playerPrefsManager.SetInt(PlayerPrefsManager.PlayerPrefsKeys.ChosenLevel, level);
        startGame.onClick.AddListener((() =>
        {
            GameManager.instance.redirectFromMainMenu = true;
            GameManager.instance.LoadScene("Game");
        }));
    }
    
}
