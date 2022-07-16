using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoController : MonoBehaviour
{
    public bool isInReload;
    private int magazineSpace = 8;
    private int pocketAmmoCount;
    private GameManager gameManager;
    [HideInInspector] public int totalAmmo;
    [SerializeField] private GameObject reloadTextObject;
    [SerializeField] private Animator reloadAnimation;
    private GameController gameController;
    private GameObject ammoContainer;
    private GameObject[] ammos;


    private void Start()
    {
        isInReload = false;
        gameController = GameController.instance;
        gameManager = GameManager.instance;
        totalAmmo = gameManager.levelDataReader
            .GetLevelData(gameController.level).numberOfAmmos;
        if (gameController.weapon.weaponType == Weapon.WeaponType.Launcher)
        {
            var launcherController = gameController.weapon.GetWeapon().GetComponent<LauncherController>();
            ammos = launcherController.ammos;
            ammoContainer = launcherController.ammoContainer;
        }
        else if (gameController.weapon.weaponType == Weapon.WeaponType.Turret)
        {
            totalAmmo *= 10;
            magazineSpace = 50;
        }
        pocketAmmoCount = totalAmmo - magazineSpace;
        pocketAmmoCount = pocketAmmoCount < 0 ? 0 : pocketAmmoCount;
        GameUIController.instance.ammoInfo.text = (totalAmmo >= magazineSpace ? magazineSpace : totalAmmo).ToString() + "/" + pocketAmmoCount;
    }

    public void DecreaseAmmo()
    {
        // if (totalAmmo - pocketAmmoCount <= 0)
        //     return;
        totalAmmo--;
        int inMagAmmo = totalAmmo - pocketAmmoCount;
        if (inMagAmmo < 0)
            inMagAmmo = 0;
        GameUIController.instance.ammoInfo.text = inMagAmmo.ToString() + "/" + pocketAmmoCount;
        int index = inMagAmmo % magazineSpace;
        if (gameController.weapon.weaponType == Weapon.WeaponType.Launcher)
        {
            ammos[index].SetActive(false);
        } 


        if (totalAmmo <= 0)
        {
            gameController.weapon.ableToShot = false;   
            gameController.LostProcess();
        }
        if (index == 0 && totalAmmo > 0)
        {
            Reload();
        }
    }

    public void Reload()
    {
        StartCoroutine(ReloadProcess());
    }
    
    private IEnumerator ReloadProcess()
    {
        reloadAnimation.gameObject.SetActive(true);
        reloadTextObject.SetActive(false);
        int inMagAmmo = totalAmmo - pocketAmmoCount;
        gameController.weapon.ableToShot = false;
        GameUIController.instance.fire.interactable = false;
        isInReload = true;
        yield return new WaitForSeconds((float)(magazineSpace - inMagAmmo) * .4f % 8);
        isInReload = false;
        GameUIController.instance.fire.interactable = true;
        gameController.weapon.ableToShot = true;
        int index = (totalAmmo >= magazineSpace ? magazineSpace : totalAmmo);

        if (gameController.weapon.weaponType == Weapon.WeaponType.Launcher)
        {
            for (int i = inMagAmmo; i < index; i++)
            {
                if (pocketAmmoCount == 0)
                    break;
                ammos[i].SetActive(true);
            }
        }

        reloadAnimation.gameObject.SetActive(false);
        reloadTextObject.SetActive(true);
        pocketAmmoCount -= (index - inMagAmmo);
        GameUIController.instance.ammoInfo.text = (index).ToString() + "/" + pocketAmmoCount.ToString();
        StopAllCoroutines();
    }
}
