using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<Weapon> weapons = null;
    [SerializeField] int currentWeaponHeld = 0;
    [Space]
    [SerializeField] PlayerController player;
    [SerializeField] PlayerGUI gui;
    [Space]
    [Header("Debugging")]
    [SerializeField] bool forceWeaponSwaps;

    public void UpdateBulletCount(int clip, int reserve){
        gui.UpdateDisplayedAmmo(clip, reserve);
    }

    public void AddKillPoints(){
        player.AddPoints(100);
    }

    public void AddNewWeapon(Weapon weapon){
        // take new weapon animation play?
        int i = GetWeaponSlotPlacement();
        Weapon m_weapon = Instantiate(weapon, gameObject.transform);;
        
        // fix location
        if(weapons[i] == null)
            weapons[i] = m_weapon;
        else{
            Destroy(weapons[i].gameObject);
            weapons[i] = m_weapon;
        }

        ChangeHeldWeapon(i);
        // swap to other weapon
    }

    int GetWeaponSlotPlacement(){
        for (int i = 0; i < weapons.Count; i++)
        {
            if(weapons[i] == null){
                return i;
            }
        }

        return currentWeaponHeld;
    }

    public void SwapWeapon(float direction){
        int weaponListSize = CountWeaponList();
        if(weaponListSize > 1){
            // > = UP || < = DOWN
            if (direction > 0)
            {
                // up
                if(weaponListSize == (currentWeaponHeld + 1)){
                    ChangeHeldWeapon(0);
                }else{
                    ChangeHeldWeapon(currentWeaponHeld+1);
                }
            }
            else if (direction < 0)
            {
                // down
                if((currentWeaponHeld - 1) < 0){
                    ChangeHeldWeapon(weaponListSize - 1);
                }else{
                    ChangeHeldWeapon(currentWeaponHeld-1);
                }
            }
        }
    }

    void ChangeHeldWeapon(int newActiveNumber){
        if(!forceWeaponSwaps){
            if(weapons[currentWeaponHeld] != null)
                weapons[currentWeaponHeld].SetWeaponActive(false);
        }
        else
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].SetWeaponActive(false);
            }
        }
        
        if(weapons[newActiveNumber] != null){
            currentWeaponHeld = newActiveNumber;
            weapons[newActiveNumber].SetWeaponActive(true);
            UpdateBulletCount(weapons[newActiveNumber].GetComponent<Gun>().ReturnCurrentClipCount(), weapons[newActiveNumber].GetComponent<Gun>().ReturnCurrentAmmoReserve());
        }
    }

    int CountWeaponList(){
        int c = 0;
        for (int i = 0; i < weapons.Count; i++)
        {
            if(weapons[i] != null)
                c++;
        }

        return c;
    }
}
