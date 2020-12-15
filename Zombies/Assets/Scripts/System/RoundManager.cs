using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    [Header("Game Stats")]
    [SerializeField] int round = 1;
    [SerializeField] int zombiesRemaining = 0;
    [SerializeField] List<Zombie> zombiesActive = null;
    [Header("Config")]
    [SerializeField] int maxZombieSpawns = 30;
    [SerializeField] float roundStartWaitTime = 4f;
    [SerializeField] GlobalGUI globalGUI = null;
    [Space]
    [SerializeField] float testSpawn = 2.5f;
    [SerializeField] float nextSpawnTime = 1f;
    
    float roundCountdown;

    void Awake(){
        Instance = this;
        roundCountdown = roundStartWaitTime;
    }

    void Start(){
        zombiesRemaining = GetZombieCountInRound(round);
    }

    void Update() {
        if(roundCountdown > 0){
            roundCountdown -= Time.deltaTime;
        }

        if(roundCountdown <= 0 && nextSpawnTime > 0){
            nextSpawnTime -= Time.deltaTime;
        }else if(roundCountdown <= 0 && nextSpawnTime <= 0){
            if(zombiesActive.Count < maxZombieSpawns && zombiesRemaining > 0){
                if(zombiesActive.Count < zombiesRemaining){
                    ZoneManager.Instance.AddNewZombie();
                    nextSpawnTime = 2 + (round / (round + testSpawn)); 
                }               
            }     
        }
    }

    public void UpdateZombiesRemaining(Zombie zombie){
        if(zombiesActive.Contains(zombie)){
            zombiesRemaining--;
            zombiesActive.Remove(zombie);
        }

        if(zombiesRemaining <= 0)
            EndRound();
    }

    void EndRound(){
        if(zombiesRemaining <= 0){
            round++;
            zombiesRemaining = GetZombieCountInRound(round);
            globalGUI.UpdateDisplayedRound(round);
            roundCountdown = roundStartWaitTime;
        }
    }
    
    int GetZombieCountInRound(int _round){
        switch(_round){
            case 1:
                return 5;
            case 2:
                return 7;
            case 3:
                return 9;
            case 4:
                return 14;
            case 5:
                return 18;
            case 6:
                return 22;
            default:
                int num = (int)(0.000058f * Mathf.Pow(_round, 3) + 0.074032f * Mathf.Pow(_round, 2) + 0.718119f * _round + 14.738699f);
                return num;
        }
    }

    public int GetCurrentRound(){
        return round;
    }

    public void AddZombieToPool(Zombie zombie){
        zombiesActive.Add(zombie);
    }
}
