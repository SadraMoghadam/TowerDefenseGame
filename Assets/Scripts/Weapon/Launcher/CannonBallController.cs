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
    [HideInInspector] public AudioSource audioSource;


    private void Start()
    {
        explosionHappened = false;
        gameManager = GameManager.instance;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name.Contains("Launcher"))
            return;
        if (!explosionHappened && !gameManager.gameSetting.slowMotionOnExplosion)
        {
            ExplosionProcess();
            explosionHappened = true;
            Destroy(Instantiate(explosion, transform.position, transform.rotation), 1.8f);
        }
        else if (!explosionHappened && gameManager.gameSetting.slowMotionOnExplosion)
        {
            ExplosionProcess();
            explosionHappened = true;
            Destroy(Instantiate(explosion, transform.position, transform.rotation), 1.8f);
            StartCoroutine(GameController.instance.SlowMotion(0.3f, 0.6f));
        }
        this.transform.localScale = Vector3.zero;
        Destroy(this.gameObject, 1.5f);
    }

    private void ExplosionProcess()
    {
        gameManager.audioController.PlaySfx(audioSource, AudioController.SFXType.Explosion);
        Collider[] collisions = new Collider[40];
        for (int i = 0; i < explosionRadius.Length; i++)
        {
            var surroundingObjectsCount = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius[i], collisions);
            for (int j = 0; j < surroundingObjectsCount; j++)
            {
                if (collisions[j].gameObject.CompareTag("Enemy"))
                {
                    EnemyAI enemy = collisions[j].gameObject.GetComponent<EnemyAI>();
                    Rigidbody rigidbody = collisions[j].GetComponent<Rigidbody>();
                    if (enemy == null || rigidbody == null)
                    {
                        return;
                    }
                    enemy.Damage(explosionForce[i] / 10 * 1.5f);
                    rigidbody.AddExplosionForce(explosionForce[i], transform.position + Vector3.down, explosionRadius[i]);
                }

                // if (collisions[j].gameObject.CompareTag("Launcher"))
                // {
                //     StartCoroutine(GameController.instance.weapon.mainCamera.gameObject.GetComponent<CameraShake>().Shake(.3f, .2f));
                // }
            }
        }
        
    }
}
