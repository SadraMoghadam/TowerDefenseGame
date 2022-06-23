using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private float health;
    private float maxHealth = 100;
    private GameManager gameManager;

    private void Start()
    {
        health = maxHealth;
        gameManager = GameManager.instance;
    }

    public void Damage(float hitStrength)
    {
        health -= hitStrength;
        Debug.Log(health);
        if (health <= 0)
        {
            Destroy(gameObject);
            gameManager.gameController.LostProcess();
        }
    }
}
