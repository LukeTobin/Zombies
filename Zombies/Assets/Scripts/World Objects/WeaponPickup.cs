using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : Interactable
{
    [SerializeField] Weapon weapon = null;

    public override void Interact(PlayerController interactor)
    {
        interactor.AddWeaponToInventory(weapon);
    }

    public Weapon GetWeapon(){
        return weapon;
    }

    public string GetWeaponName(){
        return weapon.weaponName;
    }

    public Weapon.Rarity GetWeaponRarity(){
        return weapon.weaponRarity;
    }
}
