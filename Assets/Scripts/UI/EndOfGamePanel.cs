using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndOfGamePanel : MonoBehaviour
{
    public Button playAgainButton;
    public Button quitButton;
    public Button nextLevelButton;
    public TMP_Text message;
    public RawImage messageBackground;
    public Sprite winBackground;
    public Sprite loseBackground;
    private bool result;
    [SerializeField] private List<GameObject> stars;

    public void OnEnable()
    {
        List<int> levelsCompleted = GameManager.instance.playerPrefsManager.GetComletedLevelNumbers();
        nextLevelButton.gameObject.transform.parent.gameObject.SetActive(true);
        nextLevelButton.onClick.AddListener((() =>
        {
            if (GameController.instance.level <= levelsCompleted.Max())
            {
                int level = GameManager.instance.playerPrefsManager.GetInt(
                    PlayerPrefsManager.PlayerPrefsKeys.ChosenLevel, 1);
                GameManager.instance.playerPrefsManager.SetInt(PlayerPrefsManager.PlayerPrefsKeys.ChosenLevel, level + 1);
            }
            OnButtonClicked();
        }));
        quitButton.onClick.AddListener((() =>
        {
            gameObject.SetActive(false);
            Time.timeScale = 1;
            GameManager.instance.LoadScene("MainMenu");
        }));
        playAgainButton.onClick.AddListener((() =>
        {
            OnButtonClicked();
        }));
        if (result)
        {
            message.text = "You Won";
            messageBackground.texture = winBackground.texture;
        }
        else
        {
            message.text = "You Lost";
            messageBackground.texture = loseBackground.texture;
        }

        if (levelsCompleted.Max()+1 == GameController.instance.level && !result)
        {
            nextLevelButton.gameObject.transform.parent.gameObject.SetActive(false);
        }
        // Invoke("StopTime", .5f);
        Invoke("SetupStars", .5f);
    }

    private void OnButtonClicked()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
        GameManager.instance.redirectFromMainMenu = false;
        GameManager.instance.LoadScene("Game");
    }
    
    private void StopTime()
    {
        Time.timeScale = 0;
    }

    // true = win   &&&   false = lose
    public void EOGPanelShow(bool result)
    {
        this.result = result;
        this.gameObject.SetActive(true);
    }

    public void SetupStars()
    {
        int levelStars = Math.Max(GameController.instance.stars, GameManager.instance.playerPrefsManager.GetStarsOfLevel(GameController.instance.level));
        
        if (levelStars == 0)
        {
            return;
        }
        for (int i = 0; i < stars.Count; i++)
        {
            if (i < levelStars)
            {
                // Invoke("ActivateStar", i * .2f);/
                stars[i].SetActive(true);
            }
            else
            {
                stars[i].SetActive(false);
            }
        }
    }

    public void ActivateStar(GameObject star)
    {
        star.SetActive(true);
    }
}
