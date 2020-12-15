using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] PlayerGUI gui;

    public void UpdateBulletCount(int clip, int reserve){
        gui.UpdateDisplayedAmmo(clip, reserve);
    }

    public void AddKillPoints(){
        player.AddPoints(100);
    }
}
