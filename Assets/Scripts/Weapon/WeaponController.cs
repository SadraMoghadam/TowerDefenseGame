using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public List<Weapon> weapons;

    private void Awake()
    {
        Weapon.WeaponType weaponType = Weapon.WeaponType.Launcher;
        string weaponName = "Default";
        foreach (var weapon in weapons)
        {
            if (weapon.weaponType == weaponType && weapon.weaponName == weaponName)
            {
                var weaponObject = Instantiate(weapon.weaponGameObject);
                weaponObject.transform.parent = this.transform;
                weaponObject.transform.localPosition = Vector3.zero;
                break;
            }
        }
    }
}
