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
    private GameObject[] weaponsObject;
    [HideInInspector] public bool ableToShot;
    [HideInInspector] public Camera mainCamera;
    [HideInInspector] public Weapon.WeaponType weaponType1;
    [HideInInspector] public Weapon.WeaponType weaponType2;
    [HideInInspector] public Weapon.WeaponType currentWeaponType;
    private void Awake()
    {
        weaponsObject = new GameObject[2];
        weapons = weaponsScriptableObject.weapons;
        ableToShot = true;
        weaponType1 = GameManager.instance.playerPrefsManager.GetWeaponType(1);
        weaponType2 = GameManager.instance.playerPrefsManager.GetWeaponType(2);
        currentWeaponType = GameManager.instance.playerPrefsManager.GetCurrentWeaponType();
        string weaponName = "Default";
        foreach (var weapon in weapons)
        {
            if (weapon.weaponType == weaponType1 && weapon.weaponName == weaponName)
            {
                var weaponObject = Instantiate(weapon.weaponGameObject);
                weaponObject.transform.parent = this.transform;
                weaponObject.transform.localPosition = Vector3.zero;
                weaponsObject[0] = weaponObject;
                if (currentWeaponType == weaponType1)
                {
                    weaponObject.SetActive(true);
                    CurrentWeapon = weaponObject;
                }
                else
                {
                    weaponObject.SetActive(false);
                }
            }
            if (weapon.weaponType == weaponType2 && weapon.weaponName == weaponName)
            {
                var weaponObject = Instantiate(weapon.weaponGameObject);
                weaponObject.transform.parent = this.transform;
                weaponObject.transform.localPosition = Vector3.zero;
                weaponsObject[1] = weaponObject;
                if (currentWeaponType == weaponType2)
                {
                    weaponObject.SetActive(true);
                    CurrentWeapon = weaponObject;
                }
                else
                {
                    weaponObject.SetActive(false);
                }
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

    public void ChangeWeapon()
    {
        currentWeaponType = GameManager.instance.playerPrefsManager.GetCurrentWeaponType();
        if (currentWeaponType == weaponType1)
        {
            currentWeaponType = weaponType2;
            CurrentWeapon = weaponsObject[1];
            weaponsObject[1].SetActive(true);
            weaponsObject[0].SetActive(false);
        }
        else
        {
            currentWeaponType = weaponType1;
            CurrentWeapon = weaponsObject[0];
            weaponsObject[0].SetActive(true);
            weaponsObject[1].SetActive(false);
        }
        GameManager.instance.playerPrefsManager.SetCurrentWeaponType(currentWeaponType);
    }

    public GameObject GetWeapon()
    {
        return CurrentWeapon;
    }
}
