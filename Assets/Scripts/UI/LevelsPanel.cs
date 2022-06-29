using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

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
    private ScrollRect scrollRect;
    private float eachButtonSpace = 117.5f;


    private void Awake()
    {
        levelsData = GameManager.instance.levelDataReader.levelData;
        scrollRect = gameObject.GetComponentInChildren<ScrollRect>();
    }

    private void OnEnable()
    {
        List<int> levelsCompleted = GameManager.instance.playerPrefsManager.GetComletedLevelNumbers();
        int levels = levelsData.Count - 1;
        RectTransform objectRect = levelsContainer.GetComponent<RectTransform>();
        objectRect.sizeDelta = new Vector2(objectRect.sizeDelta.x, (float)levels * eachButtonSpace);
        bool firstTime = false;
        for (int i = 0; i < levels; i++)
        {
            GameObject buttonObject;
            if (levelsContainer.transform.childCount <= 0)
            {
                firstTime = true;
            }

            if (firstTime)
            {
                if (i == levels - 1)
                {
                    firstTime = false;
                }
                buttonObject = Instantiate(levelButtonObject);
            }
            else
            {
                buttonObject = levelsContainer.gameObject.GetComponentsInChildren<Button>()[i].gameObject;
            }
            buttonObject.transform.parent = levelsContainer.transform;
            buttonObject.GetComponentInChildren<TMP_Text>().text = "Level " + (i + 1).ToString();
            Button levelButton = buttonObject.GetComponent<Button>();
            if (levelsCompleted == null || levelsCompleted.Count <= 0)
            {
                if (i == 0)
                {
                    levelButton.interactable = true; 
                    SelectLevelButton(levelButton, false);
                }
                else
                {
                    levelButton.interactable = false;
                }
            }
            else if (levelsCompleted.Max() >= i && i != levels - 1)
            {
                levelButton.interactable = true;
                if (levelsCompleted.Max() == i)
                {
                    SelectLevelButton(levelButton, true);
                }
            }
            else
            {
                levelButton.interactable = false;
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
            button.gameObject.GetComponent<Animator>().Play("Select");
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

    private void SelectLevelButton(Button levelButton, bool snap)
    {
        levelButton.Select();
        levelButton.gameObject.GetComponent<Animator>().Play("Select");
        SetUpLevelPopUp(levelButton);
        if (snap)
        {
            SnapTo(scrollRect, levelButton.gameObject.GetComponent<RectTransform>());
        }
    }
    
    private void SnapTo( ScrollRect scroller, RectTransform child )
    {
        Canvas.ForceUpdateCanvases();

        var contentPos = (Vector2)scroller.transform.InverseTransformPoint( scroller.content.position );
        var childPos = (Vector2)scroller.transform.InverseTransformPoint( child.position );
        var endPos = contentPos - childPos;
        endPos.x = 0;
        endPos.y -= eachButtonSpace;
        scroller.content.anchoredPosition = endPos;
        scroller.content.offsetMin = new Vector2(0, scroller.content.offsetMin.y);
        scroller.content.offsetMax = new Vector2(0, scroller.content.offsetMax.y);
    }
    
    
}
