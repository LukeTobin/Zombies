using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnimationController : MonoBehaviour
{
    [SerializeField] List<string> attackAnimTriggers = null;

    Animator animator;
    float currentAnimationLength = 0f;
    bool shouldRun;

    void Awake(){
        animator = GetComponent<Animator>();
    }

    void Update(){
        if(shouldRun){
            if(!animator.GetBool("Runner")){
                animator.SetBool("Runner", shouldRun);
            }
        }
    }

    public void PlayAttackAnimation(){
        int n = Random.Range(0, attackAnimTriggers.Count);
        animator.SetTrigger(attackAnimTriggers[n]);
        
        currentAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length - 0.5f;
    }

    public void UpdateMovementSpeed(bool isRunning){
        shouldRun = isRunning;
        animator.SetBool("Runner", isRunning);
    }

    public float GetCurrentClipLength(){
        return currentAnimationLength;
    }

    public void SetIdleAnimation(){
        animator.SetBool("StayIdle", true);
    }
}
