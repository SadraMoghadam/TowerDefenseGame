using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallController : MonoBehaviour
{
    public GameObject Explosion;
    private bool slowMotionHappened;
    private GameManager gameManager;


    private void Start()
    {
        slowMotionHappened = false;
        gameManager = GameManager.instance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Launcher")
            return;
        if (!slowMotionHappened && gameManager.gameSetting.SlowMotionOnExplosion)
        {
            slowMotionHappened = true;
            StartCoroutine(GameManager.instance.gameController.SlowMotion(0.5f, 0.2f));
        }
        Destroy(Instantiate(Explosion, transform.position, transform.rotation), 2);
        Destroy(this.gameObject, 0.3f);
    }
}
