using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerGUI : MonoBehaviour
{
    [Header("Main GUI")]
    [SerializeField] TMP_Text pointText;
    [SerializeField] TMP_Text clipAmmoText;
    [SerializeField] TMP_Text remainingAmmoText;
    [SerializeField] TMP_Text interactText;

    private void Start() {
        //interactText.enabled = false;
    }

    public void UpdateDisplayedPoints(int pointsToAdd){
        pointText.text = pointsToAdd.ToString();
    }

    public void UpdateDisplayedAmmo(int newClip, int newRemainingAmmo){
        clipAmmoText.text = newClip.ToString();
        remainingAmmoText.text = newRemainingAmmo.ToString();
    }

    public void InterationPopup(string text, bool show){
        interactText.text = text;
        //interactText.enabled = show;
    }
}
