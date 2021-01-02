using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int healthBlockCount = 2;
    [SerializeField] float regenTime = 1.8f;
    [SerializeField] float waitAfterDamage = 1f;
    [SerializeField] float blockToughness = 1f;

    [SerializeField] List<float> healthBlocks = null;
    int currentBlock = 0;

    PlayerGUI gui;

    void Awake() {
        gui = GetComponentInChildren<PlayerGUI>();
        InitializeHealthBlocks(healthBlockCount);
    }

    void Start(){
        HealthBlockUpdate();
    }

    public void TakeDamage(float damage){
        float remainderCheck = (healthBlocks[currentBlock] - damage);
        if(remainderCheck <= 0){
            // Destroy Block
            HealthBlockUpdate();
            currentBlock++;
        }else{
            healthBlocks[currentBlock] = remainderCheck;
        }
    }

    public void HealthBlockUpdate(){

    }

    void InitializeHealthBlocks(int blockCount){
        for (int i = 0; i < blockCount; i++)
        {
            healthBlocks.Add(blockToughness);
        }
    }
}
