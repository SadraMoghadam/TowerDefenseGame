using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallController : MonoBehaviour
{
    public GameObject explosion;
    private bool slowMotionHappened;
    private GameManager gameManager;
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float explosionRadius = 10f;


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
            ExplosionProcess();
            slowMotionHappened = true;
            StartCoroutine(GameManager.instance.gameController.SlowMotion(0.2f, 0.8f));
        }
        Destroy(Instantiate(explosion, transform.position, transform.rotation), 2);
        this.transform.localScale = Vector3.zero;
        Destroy(this.gameObject, 0.9f);
    }

    private void ExplosionProcess()
    {
        var surroundingObjects = Physics.OverlapSphere(transform.position, explosionRadius);
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
                enemy.Damage(50);
                rigidbody.AddExplosionForce(explosionForce, transform.position + Vector3.down, explosionRadius);
            }
        }
    }
}
