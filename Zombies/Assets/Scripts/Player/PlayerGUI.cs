using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerGUI : MonoBehaviourPunCallbacks
{
    [Header("Main GUI")]
    [SerializeField] TMP_Text pointText;
    [SerializeField] TMP_Text clipAmmoText;
    [SerializeField] TMP_Text remainingAmmoText;
    [SerializeField] TMP_Text interactText;

    public void UpdateDisplayedPoints(int pointsToAdd){
        if(photonView.IsMine)
            pointText.text = pointsToAdd.ToString();
    }

    public void UpdateDisplayedAmmo(int newClip, int newRemainingAmmo){
        if(photonView.IsMine){
            clipAmmoText.text = newClip.ToString();
            remainingAmmoText.text = newRemainingAmmo.ToString();
        }
    }

    public void InterationPopup(string text, bool show){
        if(photonView.IsMine)
            interactText.text = text;
    }
}
