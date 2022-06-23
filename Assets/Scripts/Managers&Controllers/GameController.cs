using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject launcher;
    public List<GameObject> walls;
    [HideInInspector] public int numberOfEnemiesAlive;
}
