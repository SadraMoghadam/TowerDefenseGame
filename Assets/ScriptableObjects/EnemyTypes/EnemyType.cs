using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyType", menuName = "Enemy/EnemyType")]
public class EnemyType : ScriptableObject
{
    public int id;
    public string name;
    public GameObject prefab;
    public float speed;
    public float strength;
    public float maxHealth;
    public List<EnemyAI.HitType> HitTypes;
}
