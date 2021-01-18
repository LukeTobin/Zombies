using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Drop", menuName="Drops")]
public class Drop : ScriptableObject
{
    public string objectId;
    public float dropChance;   
}
