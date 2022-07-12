using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapons", menuName = "Weapon/Weapons")]
public class Weapons : ScriptableObject
{
    public List<Weapon> weapons;
}
