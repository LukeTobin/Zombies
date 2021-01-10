using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Weapon : MonoBehaviourPunCallbacks
{
    public enum Rarity{
        common,
        uncommon,
        rare,
        epic,
        legendary
    }

    public enum WeaponType{
        none,
        rifle,
        pistol,
        smg
    }

    [Header("Uniques")]
    public string weaponName = "";
    public int weaponDamage = 10;
    public Rarity weaponRarity = Rarity.common;
    public WeaponType weaponType = WeaponType.none;

    public void SetWeaponActive(bool condition){
        if(photonView.IsMine)
            gameObject.SetActive(condition);
    }
}
