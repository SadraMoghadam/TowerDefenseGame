using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoController : MonoBehaviour
{
    public GameObject ammoContainer;
    public GameObject[] ammos;
    private int magazineSpace = 8;
    private int pocketAmmoCount;
    private GameManager gameManager;
    private int totalAmmo;

    private void Start()
    {
        gameManager = GameManager.instance;
        totalAmmo = gameManager.gameController.levelDataReader
            .GetLevelData(gameManager.gameController.level).numberOfAmmos;
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
        ammos[index].SetActive(false);


        if (totalAmmo <= 0)
        {
            gameManager.gameController.launcher.GetComponent<LauncherController>().ableToShot = false;   
            gameManager.gameController.LostProcess();
        }
        if (index <= 0 && totalAmmo > 0)
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
        int inMagAmmo = totalAmmo - pocketAmmoCount;
        gameManager.gameController.launcher.GetComponent<LauncherController>().ableToShot = false;
        yield return new WaitForSeconds((float)(magazineSpace - inMagAmmo) * .4f);
        gameManager.gameController.launcher.GetComponent<LauncherController>().ableToShot = true;
        int index = (totalAmmo >= magazineSpace ? magazineSpace : totalAmmo);
        for (int i = inMagAmmo; i < index; i++)
        {
            if(pocketAmmoCount == 0)
                break;
            ammos[i].SetActive(true);
        }

        pocketAmmoCount -= (index - inMagAmmo);
        GameUIController.instance.ammoInfo.text = (index).ToString() + "/" + pocketAmmoCount.ToString();
    }
}
