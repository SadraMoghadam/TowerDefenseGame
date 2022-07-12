using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public List<Weapon> weapons;
    [SerializeField] private Weapons weaponsScriptableObject;
    private GameObject CurrentWeapon;
    [HideInInspector] public bool ableToShot;
    [HideInInspector] public Camera mainCamera;
    [HideInInspector] public Weapon.WeaponType weaponType;
    private void Awake()
    {
        weapons = weaponsScriptableObject.weapons;
        ableToShot = true;
        weaponType = GameManager.instance.playerPrefsManager.GetWeaponType();
        string weaponName = "Default";
        foreach (var weapon in weapons)
        {
            if (weapon.weaponType == weaponType && weapon.weaponName == weaponName)
            {
                var weaponObject = Instantiate(weapon.weaponGameObject);
                weaponObject.transform.parent = this.transform;
                weaponObject.transform.localPosition = Vector3.zero;
                CurrentWeapon = weaponObject;
                break;
            }
        }
        
        List<Camera> cameras = FindObjectsOfType<Camera>().ToList();
        foreach (var camera in cameras)
        {
            if (camera.gameObject.CompareTag("MainCamera"))
            {
                mainCamera = camera;
                break;
            }
        }
    }

    public GameObject GetWeapon()
    {
        return CurrentWeapon;
    }
}
