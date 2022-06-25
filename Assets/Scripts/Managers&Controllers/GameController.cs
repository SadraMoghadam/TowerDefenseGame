using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject launcher;
    public List<GameObject> walls;
    public int level;
    [HideInInspector] public int numberOfEnemiesAlive;
    [HideInInspector] public LevelDataReader levelDataReader;


    private void Awake()
    {
        levelDataReader = gameObject.GetComponent<LevelDataReader>();
    }

    private void GetLevelInformation()
    {
        
    }

    public void LostProcess()
    {
        Time.timeScale = 0;
        Debug.Log("You Lost");
    }
    
    public void WonProcess()
    {
        
    }
    
    public IEnumerator SlowMotion(float slowMotionCoefficient, float slowMotionTime)
    {
        Time.timeScale = slowMotionCoefficient;
        yield return new WaitForSeconds(slowMotionTime);
        Time.timeScale = 1;
        StopAllCoroutines();
    }
    
}
