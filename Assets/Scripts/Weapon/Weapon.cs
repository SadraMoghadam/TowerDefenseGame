using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Weapon
{
    [SerializeField] public enum WeaponType
    {
        Launcher = 0,
        Turret = 1
    }

    [SerializeField] public WeaponType weaponType;
    [SerializeField] public string weaponName;
    [SerializeField] public GameObject weaponGameObject;
}
