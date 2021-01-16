using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationShowcase : MonoBehaviour
{
    enum PlayerAnimationShowcase{
        none,
        test,
        rifleIdle,
        reloadRifle,
        reloadRifleMoving,
        rifleSprint,
        pistolIdle,
        pistolSprint
    }

    [Header("Third Person Animations")]
    [SerializeField] PlayerAnimationShowcase animationToShow;
    [SerializeField] bool forceAnimation = false;

    Animator animator;

    void Start() {
        animator = GetComponent<Animator>();
        animator.SetBool("AnimationShowcase", true);
        UpdateAnimation(animationToShow);   
    }

    void Update(){
        if(forceAnimation){
            UpdateAnimation(animationToShow);
            forceAnimation = false;
        }
    }

    void UpdateAnimation(PlayerAnimationShowcase animation){
        switch(animation){
            case PlayerAnimationShowcase.reloadRifle:
                animator.SetFloat("Movement", 0);
                animator.SetTrigger("ReloadRifle");
                animator.SetBool("Rifle", true);
                break;
            case PlayerAnimationShowcase.reloadRifleMoving:
                animator.SetFloat("Movement", 3);
                animator.SetTrigger("ReloadRifle");
                animator.SetBool("Rifle", true);
                break;
            case PlayerAnimationShowcase.rifleSprint:
                animator.SetFloat("Movement", 3);
                animator.SetBool("Sprint", true);
                animator.SetBool("Rifle", true);
                break;
            case PlayerAnimationShowcase.pistolIdle:
                animator.SetBool("Pistol", true);
                break;
            case PlayerAnimationShowcase.pistolSprint:
                animator.SetFloat("Movement", 3);
                animator.SetBool("Sprint", true);
                animator.SetBool("Pistol", true);
                break;
            case PlayerAnimationShowcase.test:
                animator.SetBool("Test", true);
                break;
            default:
                break;
        }
    }
}
