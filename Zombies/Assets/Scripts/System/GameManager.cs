using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] PlayerController player;

    private void Awake() {
        Instance = this;
    }

    public PlayerController GetPlayer(){
        return player;
    }
}
