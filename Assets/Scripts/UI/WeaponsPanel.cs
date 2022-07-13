using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsPanel : MonoBehaviour
{
    [SerializeField] private GameObject weaponsContainer;
    [SerializeField] private GameObject weaponOption;
    private Weapons weapons;

    private void OnEnable()
    {
        weapons = MainMenuUIController.instance.weapons;
        if (weaponsContainer.transform.childCount == 0)
        {
            GameObject option;
            foreach (var weapon in weapons.weapons)
            {
                option = Instantiate(weaponOption);
                option.transform.parent = weaponsContainer.transform;
                WeaponOption weaponOptionInfo = option.GetComponent<WeaponOption>();
                weaponOptionInfo.icon.sprite = weapon.icon;
                weaponOptionInfo.weaponName.text = weapon.weaponType.ToString();
                weaponOptionInfo.button.onClick.AddListener(() => OnWeaponOptionButtonClick(weapon.weaponType));
            }
        }
    }

    private void OnWeaponOptionButtonClick(Weapon.WeaponType weaponType)
    {
        GameManager.instance.playerPrefsManager.SetWeaponType(weaponType);
        MainMenuUIController.instance.SetCurrentWeapon();
        MainMenuUIController.instance.OnButtonClickSFX();
    }
    
}
