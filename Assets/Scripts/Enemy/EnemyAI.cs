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
    private GameController gameController;
    private List<GameObject> walls;
    private float health;
    private WallController reachedWall;
    private bool isWallChanged;
    
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
        isWallChanged = true;
        isAlive = true;
        gameManager = GameManager.instance;
        gameController = GameController.instance;
        enemyAnimator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        target = gameController.weapon.gameObject.transform;
        reachedWalls = false;
        walls = gameController.walls;
        SetBodyActivation(false);
        health = enemyType.maxHealth;
        healthBar.value = health / enemyType.maxHealth;
    }

    private void FixedUpdate()
    {
        if (gameObject.activeSelf && isAlive)
        {
            if (!reachedWalls)
            {
                var step =  enemyType.speed * Time.deltaTime; 
                transform.position = Vector3.MoveTowards(transform.position, target.position, step);
            }
            Vector3 direction = (target.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isAlive)
            return;    
        if (collision.collider.gameObject.tag != "Enemy")
        {
            SetBodyActivation(true);   
        }
    }
    
    // private void OnCollisionExit(Collision collision)
    // {
    //     if (!isAlive)
    //         return;
    //     if (collision.collider.gameObject.tag == "Wall")
    //     {
    //         reachedWalls = false;
    //         enemyAnimator.SetInteger("HitType", 0);
    //     }
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAlive)
            return;
        if (other.gameObject.CompareTag("Wall"))
        {
            reachedWalls = true;
            reachedWallName = other.gameObject.transform.parent.name;
            SelectRandomHit();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (!isAlive)
            return;
        if (other.gameObject.tag == "Wall")
        {
            isWallChanged = true;
            reachedWalls = false;
            enemyAnimator.SetInteger("HitType", 0);
        }
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
        if (isWallChanged)
        {
            isWallChanged = false;
            reachedWall = GetReachedWall();
        }
        if (reachedWall == null)
        {
            return;
        }
        reachedWall.Damage(enemyType.strength);
    }

    private WallController GetReachedWall()
    {
        for (int i = 0; i < walls.Count; i++)
        {
            if (walls[i] != null && walls[i].name == reachedWallName)
            {
                return walls[i].GetComponent<WallController>();
            }
        }

        return null;
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
        // Debug.Log(health);
        healthBar.value = health / enemyType.maxHealth;
        if (healthBar.value <= 0)
        {
            this.gameObject.layer = LayerMask.NameToLayer ("Ignore Raycast");
            rigidbody.freezeRotation = false;
            isAlive = false;
            RagdollActivation(true);
            StartCoroutine(DestoryDeadEnemy());
        }
    }

    public IEnumerator DestoryDeadEnemy()
    {
        yield return new WaitForSeconds(3f);
        var rigColliders = GetComponentsInChildren<Collider>();
        for (int i = 0; i < rigColliders.Length; i++)
        {
            rigColliders[i].enabled = false;
        }
        // int numOfEnemies = --GameController.instance.numberOfEnemiesAlive;
        // if (numOfEnemies <= 0)
        // {
        //     GameController.instance.WonProcess();
        // }
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
        GameController.instance.numberOfEnemiesAlive -= 1;
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
