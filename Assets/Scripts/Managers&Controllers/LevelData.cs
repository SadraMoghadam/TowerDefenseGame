using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData
{
    public int level;
    public string difficulty;
    public int numberOfAmmos;
    public int numberOfRewinds;
    public int numberOfEnemies;
    public int numberOfGroups;
    public List<int> enemyTypeIds;

    public LevelData(int level, string difficulty, int numberOfCannonBalls, int numberOfRewinds, 
        int numberOfEnemies, int numberOfGroups, List<int> enemyTypeIds)
    {
        this.level = level;
        this.difficulty = difficulty;
        this.numberOfAmmos = numberOfCannonBalls;
        this.numberOfRewinds = numberOfRewinds;
        this.numberOfEnemies = numberOfEnemies;
        this.numberOfGroups = numberOfGroups;
        this.enemyTypeIds = enemyTypeIds;
    }

    public override string ToString()
    {
        return "level: " + level + " ||| cannon balls: " + numberOfAmmos + " ||| enemies: " + numberOfEnemies + "|" +
               numberOfGroups;
    }
}
