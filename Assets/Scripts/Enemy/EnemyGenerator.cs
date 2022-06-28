using System;
using System.Collections;
using System.Collections.Generic;
using GeneralTools.Randomize;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class EnemyGenerator : MonoBehaviour
{
    public EnemyTypes EnemyTypes;
    [SerializeField] private Transform enemiesContainer;
    private GameManager gameManager;
    private int level;
    private LevelData levelData;
    private GameObject enemyObject;
    private int totalNumberOfEnemies;
    private float minPositiveX = 150;
    private float maxPositiveX = 160;
    private float minPositiveZ = 150;
    private float maxPositiveZ = 160;
    private float minNegativeX = 110;
    private float maxNegativeX = 120;
    private float minNegativeZ = 110;
    private float maxNegativeZ = 120;
    private int numOfBosses = 0;


    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != "Game")
            enabled = false;
        else
            enabled = true;
    }

    public void OnEnable()
    {
        gameManager =  GameManager.instance;
        level = gameObject.GetComponent<GameController>().level;
        levelData = gameManager.levelDataReader.GetLevelData(level);
        totalNumberOfEnemies = levelData.numberOfEnemies;
        Debug.Log(levelData.ToString());
        StartCoroutine(GenerateOnTime(totalNumberOfEnemies / levelData.numberOfGroups));
    }

    private IEnumerator GenerateOnTime(int numberOfEnemies)
    {
        float waitTime = levelData.difficulty == "Hard" ? 7 : levelData.difficulty == "Normal" ? 11 : 15;
        float randomPosX = Random.Range(minPositiveX, maxPositiveX);
        float randomNegX = Random.Range(minNegativeX, maxNegativeX);
        float randomPosZ = Random.Range(minPositiveZ, maxPositiveZ);
        float randomNegZ = Random.Range(minNegativeZ, maxNegativeZ);

        float randomX = randomPosX;
        float randomZ = randomPosZ;
        float selectPosNegX = Random.Range(0, 2);
        if (selectPosNegX < 0.5)
        {
            randomX = randomNegX;
        }
        else
        {
            randomX = randomPosX;
        };
        float selectPosNegZ = Random.Range(0, 2);
        if (selectPosNegZ < 0.5)
        {
            randomZ = randomNegZ;
        }
        else
        {
            randomZ = randomPosZ;
        }
        EnemyGroupGenerator(new Vector3(randomX, 0, randomZ), numberOfEnemies, 3 + numberOfEnemies / 20);
        totalNumberOfEnemies -= numberOfEnemies;
        yield return new WaitForSeconds(waitTime);
        int generatableEnemiesNumber = numberOfEnemies;
        if (totalNumberOfEnemies - numberOfEnemies <= numberOfEnemies)
        {
            generatableEnemiesNumber = totalNumberOfEnemies;   
        }
        StartCoroutine(GenerateOnTime(generatableEnemiesNumber));
    }

    private void EnemyGroupGenerator(Vector3 firstEnemyPosition, int numberOfEnemies, int numberOfColumns)
    {
        int counter = 0;
        int maxBosses = 0;
        Vector3 enemyPosition = firstEnemyPosition;
        for (int i = 0; i < numberOfEnemies; i++)
        {
            if (counter % numberOfColumns == 0)
            {
                enemyPosition = new Vector3(firstEnemyPosition.x, 0, enemyPosition.z + 2);
            }
            enemyPosition += 2 * Vector3.right;
            int enemyId = (int)GameController.EnemyTypes.Default;
            if (levelData.enemyTypeIds.Count > 1)
            {
                enemyId = Random.Range(0, levelData.enemyTypeIds.Count);
                maxBosses = levelData.difficulty == "Hard" ? 2 : levelData.difficulty == "Normal" ? 1 : 0;
                if (numOfBosses >= maxBosses && enemyId == (int)GameController.EnemyTypes.Boss)
                {
                    enemyId--;
                }
            }

            if (enemyId == (int)GameController.EnemyTypes.Boss)
            {
                numOfBosses++;
            }
            enemyObject = Instantiate(EnemyTypes.enemyTypes[levelData.enemyTypeIds[enemyId]].prefab, enemyPosition,
                Quaternion.identity);
            enemyObject.transform.parent = enemiesContainer;
            counter++;
        }
    }
}
