using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Weapon/Weapon")]
public class Weapon: ScriptableObject
{
    public enum WeaponType
    {
        Launcher = 0,
        Turret = 1
    }

    public WeaponType weaponType;
    public string weaponName;
    public GameObject weaponGameObject;
    public bool continuousFiring;
    public Sprite icon;
}
