using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyTypes", menuName = "Enemy/EnemyTypes")]
public class EnemyTypes : ScriptableObject
{
    public List<EnemyType> enemyTypes;
}
