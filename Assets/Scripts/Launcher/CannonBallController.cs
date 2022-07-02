using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallController : MonoBehaviour
{
    public GameObject explosion;
    private bool explosionHappened;
    private GameManager gameManager;
    private float[] explosionForce = {200, 165, 135};
    private float[] explosionRadius = {4, 5, 6};


    private void Start()
    {
        explosionHappened = false;
        gameManager = GameManager.instance;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Launcher")
            return;
        if (!explosionHappened && !gameManager.gameSetting.slowMotionOnExplosion)
        {
            ExplosionProcess();
            explosionHappened = true;
        }
        else if (!explosionHappened && gameManager.gameSetting.slowMotionOnExplosion)
        {
            ExplosionProcess();
            explosionHappened = true;
            StartCoroutine(GameController.instance.SlowMotion(0.3f, 0.6f));
        }
        Destroy(Instantiate(explosion, transform.position, transform.rotation), 1.8f);
        this.transform.localScale = Vector3.zero;
        Destroy(this.gameObject, 0.9f);
    }

    private void ExplosionProcess()
    {
        for (int i = 0; i < explosionRadius.Length; i++)
        {
            var surroundingObjects = Physics.OverlapSphere(transform.position, explosionRadius[i]);
            foreach (var surroundingObject in surroundingObjects)
            {
                if (surroundingObject.gameObject.tag == "Enemy")
                {
                    EnemyAI enemy = surroundingObject.gameObject.GetComponent<EnemyAI>();
                    Rigidbody rigidbody = surroundingObject.GetComponent<Rigidbody>();
                    if (enemy == null || rigidbody == null)
                    {
                        return;
                    }
                    enemy.Damage(explosionForce[i] / 10 * 1.5f);
                    rigidbody.AddExplosionForce(explosionForce[i], transform.position + Vector3.down, explosionRadius[i]);
                }
            }
        }
        
    }
}
