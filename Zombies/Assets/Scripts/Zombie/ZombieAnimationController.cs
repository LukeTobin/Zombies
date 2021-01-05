using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnimationController : MonoBehaviour
{
    [SerializeField] List<string> attackAnimTriggers = null;

    Animator animator;
    float currentAnimationLength = 0f;

    void Awake(){
        animator = GetComponent<Animator>();
    }

    public void PlayAttackAnimation(){
        int n = Random.Range(0, attackAnimTriggers.Count);
        animator.SetTrigger(attackAnimTriggers[n]);
        
        currentAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length - 0.5f;
    }

    public float GetCurrentClipLength(){
        return currentAnimationLength;
    }
}
