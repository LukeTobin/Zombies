using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    Animator animator;

    void Awake() {
        animator = GetComponent<Animator>();        
    }

    public void SetAnimBool(string animName, bool state){
        animator.SetBool(animName, state);
    }

    public void SetAnimFloat(string animName, float value){
        animator.SetFloat(animName, value);
    }

    public void SetAnimTrigger(string animName){
        animator.SetTrigger(animName);
    }
}
