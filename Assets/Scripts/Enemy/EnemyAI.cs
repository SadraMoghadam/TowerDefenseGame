using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    [HideInInspector] public Rigidbody rigidbody;
    [HideInInspector] public bool isAlive;
    [SerializeField] private List<Transform> bodyParts;
    [SerializeField] private Slider healthBar;
    [SerializeField] private EnemyType enemyType;
    private Animator enemyAnimator;
    private Transform target;
    private bool reachedWalls;
    private string reachedWallName;
    private GameManager gameManager;
    private List<GameObject> walls;
    private float health;
    
    public enum HitType
    {
        RightPunch = 1,
        LeftPunch = 2,
        LeftKick = 3,
        RightKick = 4,
        Head = 5,
        Body = 6
    }

    private void Awake()
    {
        isAlive = true;
        gameManager = GameManager.instance;
        enemyAnimator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        target = gameManager.gameController.launcher.transform;
        reachedWalls = false;
        walls = gameManager.gameController.walls;
        SetBodyActivation(false);
        health = enemyType.maxHealth;
        healthBar.value = health / enemyType.maxHealth;
    }

    private void Update()
    {
        if (gameObject.activeSelf && !reachedWalls && isAlive)
        {
            var step =  enemyType.speed * Time.deltaTime; 
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
            Vector3 direction = (target.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isAlive)
            return;
        if (collision.collider.gameObject.tag == "Wall")
        {
            reachedWalls = true;
            reachedWallName = collision.collider.gameObject.name;
            SelectRandomHit();
        }

        if (collision.collider.gameObject.tag != "Enemy")
        {
            SetBodyActivation(true);   
        }
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
        if (!isAlive)
            return;
        WallController reachedWall = GetReachedWall();
        reachedWall.Damage(enemyType.strength);
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
        if (!isAlive)
            return;
        float random = Random.Range(0, 100);
        int hitType = 1;
        if (random <= 33)
        {
            hitType = (int)HitType.RightPunch;
        }
        else if (random <= 66)
        {
            hitType = (int)HitType.LeftPunch;
        }
        else if (random <= 100)
        {
            hitType = (int)HitType.LeftKick;
        }
        enemyAnimator.SetInteger("HitType", hitType);
    }

    public void Damage(float strength)
    {
        health -= strength;
        healthBar.value = health / enemyType.maxHealth;
        if (healthBar.value <= 0)
        {
            rigidbody.freezeRotation = false;
            isAlive = false;
            RagdollActivation(true);
            StartCoroutine(DestoryDeadEnemy());
        }
    }

    public IEnumerator DestoryDeadEnemy()
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }

    private void RagdollActivation(bool activate)
    {
        var rigColliders = GetComponentsInChildren<Collider>();
        var rigRigidbodies = GetComponentsInChildren<Rigidbody>();
        if (activate)
        {
            GetComponent<BoxCollider>().enabled = false;
            rigidbody.isKinematic = true; 
            enemyAnimator.enabled = false;
            foreach (Rigidbody rb in rigRigidbodies) 
            {
                rb.isKinematic = false;
            }
            foreach (Collider col in rigColliders)
            {
                col.enabled = true;
            }   
        }
        if (!activate)
        {
            GetComponent<BoxCollider>().enabled = true;
            foreach (Rigidbody rb in rigRigidbodies) 
            {
                rb.isKinematic = true;
            }
            foreach (Collider col in rigColliders)
            {
                col.enabled = false;
            } 
        }
    }
    
}
