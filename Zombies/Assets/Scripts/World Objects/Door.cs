using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    [SerializeField] List<Zone> openZones = null;

    public override void Interact(PlayerController interactor){
        if(openZones != null)
            OpenConnectedZones();
        
        Destroy(gameObject);
    }

    void OpenConnectedZones(){
        foreach(Zone zone in openZones){
            zone.SetZoneLock(false);
        }
    }
}
