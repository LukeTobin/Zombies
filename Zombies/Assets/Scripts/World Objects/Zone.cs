using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Zone : MonoBehaviour
{
    [SerializeField] string zone_id = "zone_";
    [SerializeField] bool hasLock = false;
    [Space]
    [SerializeField] List<Zone> neighbourZones = null;
    [SerializeField] List<ZombieSpawner> spawners = null;
    
    bool zoneHasActivePlayers = false;

    public string GetZoneID(){
        return zone_id;
    }

    public bool GetZoneLock(){
        return hasLock;
    }

    public void SetZoneHasActivePlayers(bool active){
        zoneHasActivePlayers = active;
    }

    public List<Zone> GetNeighbourZones(){
        return neighbourZones;
        /*
        List<Zone> _zones = null;
        foreach(Zone zone in neighbourZones){
            if(!zone.hasLock){
                _zones.Add(zone);
            }
        }
        return _zones;
        */
    }

    public void SetZoneLock(bool locked){
        hasLock = locked;
    }

    public void SpawnNewZombie(){
        if(spawners != null && !hasLock)
            spawners[Random.Range(0, spawners.Count)].SpawnZombie();
    }
}
