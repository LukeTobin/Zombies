using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int healthBlockCount = 2;
    [SerializeField] float regenTime = 1.8f;
    [SerializeField] float waitAfterDamage = 1f;
    [SerializeField] float blockToughness = 1f;
    [Space]
    [SerializeField] List<Slider> healthBlockUI = null;
    [Header("Debugging")]
    [SerializeField] List<float> healthBlocks = null;
    [Space]
    [SerializeField] float damageToTake = 0.5f;
    [SerializeField] bool takeDamage = false;
    
    int currentBlock = 0;

    float timeSinceLastDamage = 0f;
    bool healthFull = true;
    bool regenerating = false;

    bool down = false;

    PlayerGUI gui;

    void Awake() {
        gui = GetComponentInChildren<PlayerGUI>();
        InitializeHealthBlocks(healthBlockCount);
    }

    void Update(){
        if(takeDamage){
            TakeDamage(damageToTake);
            takeDamage = false;
        }

        // LOGIC FOR CHECKING WHEN TOO REGENERATE HEALTH
        if(timeSinceLastDamage > 0){
            timeSinceLastDamage -= Time.deltaTime;
        }else if(timeSinceLastDamage <= 0 && !healthFull){
            if(healthBlocks[currentBlock] != blockToughness && !regenerating){
                // Lerp values
                regenerating = true;
                StartCoroutine(LerpSliderValue(currentBlock, healthBlocks[currentBlock], blockToughness, regenTime));
                timeSinceLastDamage = 0;
            } else if(!regenerating){
                for (int i = currentBlock-1; i > -1; i--)
                {
                    if(healthBlocks[i] != blockToughness && !regenerating){
                        regenerating = true;
                        StartCoroutine(LerpSliderValue(currentBlock, healthBlocks[currentBlock], blockToughness, regenTime));
                    }
                }
            }
        }else if(down){
            Debug.Log("Player is dead");
        }
    }

    public void TakeDamage(float damage){
        float remainderCheck = (healthBlocks[currentBlock] - damage);
        if(remainderCheck <= 0 && !down){
            // Destroy Block
            healthBlockUI[currentBlock].value = 0;
            healthBlocks[currentBlock] = 0;
            //HealthBlockUpdate();
            if((currentBlock + 1) > healthBlockCount)
                DownPlayer();
            else
                currentBlock++;
        }else if(!down){
            healthBlockUI[currentBlock].value = remainderCheck;
            healthBlocks[currentBlock] = remainderCheck;
        }

        timeSinceLastDamage = waitAfterDamage;
        healthFull = false;
    }

    public void HealthBlockUpdate(){
        foreach(Slider uiBlock in healthBlockUI){
            uiBlock.gameObject.SetActive(false);
        }

        for (int i = 0; i < healthBlockCount; i++)
        {
            healthBlockUI[i].gameObject.SetActive(true);
        }
    }

    void InitializeHealthBlocks(int blockCount){
        for (int i = 0; i < blockCount; i++)
        {
            healthBlocks.Add(blockToughness);
        }

        HealthBlockUpdate();
        SetHealthBlockMaxHealth(blockToughness);
    }

    void SetHealthBlockMaxHealth(float max){
        foreach(Slider uiBlock in healthBlockUI){
            uiBlock.maxValue = max;
            uiBlock.value = max;
        }
    }

    IEnumerator LerpSliderValue(int sliderIndex, float currentHealth, float targetHealth, float speed){
        for(float t = 0f;t < speed;t += Time.deltaTime){
            healthBlockUI[sliderIndex].value = Mathf.Lerp(currentHealth, targetHealth, t / speed);
            healthBlocks[sliderIndex] = Mathf.Lerp(currentHealth, targetHealth, t / speed);
            yield return null;
        }

        healthBlockUI[sliderIndex].value = targetHealth;
        healthBlocks[sliderIndex] = targetHealth;
        regenerating = false;
    }

    void CheckForDamageTaken(){
        int damageCounter = 0;
        for (int i = 0; i < healthBlockCount; i++)
        {
            if(healthBlocks[i] != blockToughness){
                damageCounter++;
            }
        }

        if(damageCounter == 0)
            healthFull = true;
    }

    void DownPlayer(){
        Debug.Log("Player is downed");
        down = true;
    }
}
