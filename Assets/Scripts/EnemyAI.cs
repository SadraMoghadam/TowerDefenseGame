using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 0.5f;
    [SerializeField] private List<Transform> bodyParts;
    private Animator enemyAnimator;
    private Rigidbody rigidbody;
    private Transform target;
    private bool reachedWalls;
    
    public enum HitTypes
    {
        RightHook = 1,
        LeftUpper = 2,
        Kick = 3
    }

    private void Awake()
    {
        enemyAnimator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        target = GameManager.instance.gameController.launcher.transform;
        reachedWalls = false;
        SetBodyActivation(false);
    }

    private void Update()
    {
        if (gameObject.activeSelf && !reachedWalls)
        {
            var step =  speed * Time.deltaTime; 
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);   
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Wall")
        {
            reachedWalls = true;
        }
        SetBodyActivation(true);
    }


    private void SetBodyActivation(bool activeself)
    {
        foreach (var bodyPart in bodyParts)
        {
            bodyPart.gameObject.SetActive(activeself);
        }
    }

}
