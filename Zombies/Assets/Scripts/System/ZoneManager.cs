using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager Instance;

    [Header("Active Zones")]
    public List<Zone> mainZones;
    public List<Zone> secondaryZones;
    
    [Header("Zone Config")]
    [Tooltip("Adjusts the % chance of a zombie spawning at a point in the main zone, compared too the secondary zone. Non-editable during runtime.")]
    [Range(1,99)]
    [SerializeField] float mainZoneSpawnRate = 65f;

    [Header("Debug")]
    [SerializeField] bool disableSpawning = false;

    float subSpawnRate;

    void Awake() {
        Instance = this;
    }

    void Start() {
        subSpawnRate = (100f - mainZoneSpawnRate);
    }

    public void AddNewMainZone(Zone zone){
        mainZones.Add(zone);

        foreach(Zone _zone in zone.GetNeighbourZones()){
            secondaryZones.Add(_zone);
        }
    }

    public void RemoveZones(Zone zone){
        if(mainZones.Contains(zone)){
            mainZones.Remove(zone);
        }

        foreach(Zone _zone in zone.GetNeighbourZones()){
            if(secondaryZones.Contains(_zone)) 
                secondaryZones.Remove(_zone);
        }
    }

    public void AddNewZombie(){
        if(!disableSpawning){
            float rand = Random.Range(1f, 101f);
            if(rand <= mainZoneSpawnRate){
                mainZones[Random.Range(0, mainZones.Count)].SpawnNewZombie();
            }else{
                secondaryZones[Random.Range(0, secondaryZones.Count)].SpawnNewZombie();
            }
        }
    }
}
