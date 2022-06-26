using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndOfGamePanel : MonoBehaviour
{
    public Button playAgainButton;
    public Button quitButton;
    public TMP_Text message;
    public RawImage messageBackground;
    public Sprite winBackground;
    public Sprite loseBackground;
    private bool result;

    public void OnEnable()
    {
        quitButton.onClick.AddListener((() =>
        {
            gameObject.SetActive(false);
            Time.timeScale = 1;
        }));
        playAgainButton.onClick.AddListener((() =>
        {
            gameObject.SetActive(false);
            Time.timeScale = 1;
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
        Invoke("StopTime", .5f);
    }
    
    private void StopTime()
    {
        Time.timeScale = 0;
    }

    public void EOGPanelShow(bool result)
    {
        this.result = result;
        this.gameObject.SetActive(true);
    }
}
