using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private List<Transform> bodyParts;
    private float speed = 0.7f;
    private float strength = 1f;
    private Animator enemyAnimator;
    private Rigidbody rigidbody;
    private Transform target;
    private bool reachedWalls;
    private string reachedWallName;
    private GameManager gameManager;
    private List<GameObject> walls;
    
    public enum HitType
    {
        RightHook = 1,
        LeftUpper = 2,
        Kick = 3
    }

    private void Awake()
    {
        gameManager = GameManager.instance;
        enemyAnimator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        target = gameManager.gameController.launcher.transform;
        reachedWalls = false;
        walls = gameManager.gameController.walls;
        SetBodyActivation(false);
    }

    private void Update()
    {
        if (gameObject.activeSelf && !reachedWalls)
        {
            var step =  speed * Time.deltaTime; 
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
            Vector3 direction = (target.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Wall")
        {
            reachedWalls = true;
            reachedWallName = collision.collider.gameObject.name;
            SelectRandomHit();
        }
        if(collision.collider.gameObject.tag != "Enemy")
        SetBodyActivation(true);
    }


    private void SetupEnemy()
    {
        
    }
    
    private void SetBodyActivation(bool activeself)
    {
        foreach (var bodyPart in bodyParts)
        {
            bodyPart.gameObject.SetActive(activeself);
        }
    }

    public void DamageWall()
    {
        WallController reachedWall = GetReachedWall();
        reachedWall.Damage(strength);
    }

    private WallController GetReachedWall()
    {
        foreach (var wall in walls)
        {
            if (wall.name == reachedWallName)
            {
                return wall.GetComponent<WallController>();
            }
        }

        return new WallController();
    }
    
    public void SelectRandomHit()
    {
        float random = Random.Range(0, 100);
        int hitType = 1;
        if (random <= 33)
        {
            hitType = (int)HitType.RightHook;
        }
        else if (random <= 66)
        {
            hitType = (int)HitType.LeftUpper;
        }
        else if (random <= 100)
        {
            hitType = (int)HitType.Kick;
        }
        enemyAnimator.SetInteger("HitType", hitType);
    }
    
}
