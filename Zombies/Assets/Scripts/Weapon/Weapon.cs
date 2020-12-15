using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Rarity{
        common,
        uncommon,
        rare,
        epic,
        legendary
    }

    public string weaponName = "";
    public int weaponDamage = 10;
    public Rarity weaponRarity = Rarity.common;
}
