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

    public void OnEnable()
    {
        List<int> levelsCompleted = GameManager.instance.playerPrefsManager.GetLevelsCompleted();
        nextLevelButton.gameObject.transform.parent.gameObject.SetActive(true);
        nextLevelButton.onClick.AddListener((() =>
        {
            if (GameController.instance.level <= levelsCompleted.Max())
            {
                GameController.instance.level++;   
            }
            OnButtonClicked();
        }));
        quitButton.onClick.AddListener((() =>
        {
            OnButtonClicked();
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

        if (levelsCompleted.Max() <= GameController.instance.level && !result)
        {
            nextLevelButton.gameObject.transform.parent.gameObject.SetActive(false);
        }
        Invoke("StopTime", .5f);
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
}
