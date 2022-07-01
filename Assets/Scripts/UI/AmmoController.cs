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
    [SerializeField] private GameObject reloadTextObject;
    [SerializeField] private Animator reloadAnimation;

    private void Start()
    {
        gameManager = GameManager.instance;
        totalAmmo = gameManager.levelDataReader
            .GetLevelData(GameController.instance.level).numberOfAmmos;
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
            GameController.instance.launcher.GetComponent<LauncherController>().ableToShot = false;   
            GameController.instance.LostProcess();
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
        reloadAnimation.gameObject.SetActive(true);
        reloadTextObject.SetActive(false);
        int inMagAmmo = totalAmmo - pocketAmmoCount;
        GameController.instance.launcher.GetComponent<LauncherController>().ableToShot = false;
        yield return new WaitForSeconds((float)(magazineSpace - inMagAmmo) * .4f);
        GameController.instance.launcher.GetComponent<LauncherController>().ableToShot = true;
        int index = (totalAmmo >= magazineSpace ? magazineSpace : totalAmmo);
        for (int i = inMagAmmo; i < index; i++)
        {
            if(pocketAmmoCount == 0)
                break;
            ammos[i].SetActive(true);
        }

        reloadAnimation.gameObject.SetActive(false);
        reloadTextObject.SetActive(true);
        pocketAmmoCount -= (index - inMagAmmo);
        GameUIController.instance.ammoInfo.text = (index).ToString() + "/" + pocketAmmoCount.ToString();
    }
}
