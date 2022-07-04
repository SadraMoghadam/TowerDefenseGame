using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WallController : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    private float health;
    private float maxHealth;
    private GameManager gameManager;

    private void Start()
    {
        maxHealth = 100;
        health = maxHealth;
        gameManager = GameManager.instance;
        healthBar.value = health / maxHealth;
        healthBar.gameObject.GetComponentsInChildren<TMP_Text>()[0].text = (healthBar.value * 100).ToString() + "%";
    }

    public void Damage(float hitStrength)
    {
        health -= hitStrength;
        if (healthBar == null)
        {
            return;
        }
        healthBar.value = health / maxHealth;
        healthBar.gameObject.GetComponentsInChildren<TMP_Text>()[0].text = (healthBar.value * 100).ToString() + "%";
        // Debug.Log(health);
        if (health <= 0)
        {
            Destroy(gameObject);
            GameController.instance.LostProcess();
        }
    }
}
