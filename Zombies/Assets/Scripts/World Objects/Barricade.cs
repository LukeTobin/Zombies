using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barricade : Interactable
{
    [SerializeField] int barriers = 5;

    public override void Interact()
    {
        if(barriers < 5){
            barriers++;
        }
    }

    public int BarriersRemaning(){
        return barriers;
    }
}
