using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataReader : MonoBehaviour
{
    private string fileName = "LevelData";
    private List<LevelData> levelData;
    
    private void Awake()
    {
        levelData = ReadLevelsData(fileName);
    }

    public List<LevelData> ReadLevelsData(string filename)
    {
        int currentLineNumber = 0;
        int columnCount = 0;
        TextAsset txt = Resources.Load<TextAsset>(filename);
        string[] lines = txt.text.Split('\n');
        levelData = new List<LevelData>();
        foreach (string line in lines)
        {
            if (line == "")
            {
                continue;
            }

            string[] lineSplitted = line.Split(',');
            currentLineNumber++;

            if (currentLineNumber == 1)
            {
                columnCount = lineSplitted.Length;
                continue;
            }

            int level = int.Parse(lineSplitted[0]);
            string difficulty = lineSplitted[1];
            int numberOfCannonBalls = int.Parse(lineSplitted[2]);
            int numberOfRewinds = int.Parse(lineSplitted[3]);
            int numberOfEnemies = int.Parse(lineSplitted[4]);
            int numberOfGroups = int.Parse(lineSplitted[5]);
            List<int> enemyTypeIds = new List<int>();
            string[] types = lineSplitted[6].Split('&');
            foreach (var type in types)
            {
                enemyTypeIds.Add(int.Parse(type));
            }

            LevelData data = new LevelData(level, difficulty, numberOfCannonBalls, numberOfRewinds, numberOfEnemies,
                numberOfGroups, enemyTypeIds);
            levelData.Add(data);
        }

        return levelData;
    }

    public LevelData GetLevelData(int level)
    {
        return levelData[level];
    }
    
}
