using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GlobalGUI : MonoBehaviour
{
    [SerializeField] TMP_Text roundText;

    public void UpdateDisplayedRound(int newRound){
        roundText.text = newRound.ToString();
    }
}
