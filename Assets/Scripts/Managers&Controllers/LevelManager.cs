using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [HideInInspector] public int level;
    [HideInInspector] public int numberOfEnemies;
    [HideInInspector] public int speed;
    [HideInInspector] public int numberOfGroups;
    [HideInInspector] public float groupGenerationMinWaitTime;
    [HideInInspector] public float groupGenerationMaxWaitTime;
}
