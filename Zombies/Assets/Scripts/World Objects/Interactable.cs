using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] bool isPurchasable = false;
    [SerializeField] int cost = 0;
    [Space]
    [SerializeField] string interactionText = "";

    public virtual void Interact(){
        Debug.Log("interacted");     
    }

    public int GetInteractionCost(){
        return cost;
    }

    public string GetInteractionText(){
        return interactionText;
    }

    public bool GetPurchasable(){
        return isPurchasable;
    }
}
