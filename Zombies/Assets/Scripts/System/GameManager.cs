using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public string playerPrefab;
    public Transform spawnPoint;
    [Space]
    [SerializeField] PlayerController player;
    [Space]
    [SerializeField] bool offlineMode = false;
 
    private void Awake() {
        Instance = this;
    }
    
    private void Start() {
        SpawnPlayer();
    }

    public void SpawnPlayer(){
        if(!offlineMode)
            PhotonNetwork.Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        else
            player.gameObject.SetActive(true);
    }

    public PlayerController GetPlayer(){
        return player;
    }

    public bool IsGameOffline(){
        return offlineMode;
    }
}
