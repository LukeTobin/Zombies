using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.UI.ModernUIPack;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] ModalWindowManager modalWindowManager;

    public void OpenModalWindow(){
        modalWindowManager.UpdateUI(); // Update UI
        modalWindowManager.OpenWindow(); // Open window
       // modalWindowManager.CloseWindow(); // Close window
       // modalWindowManager.AnimateWindow(); // Close/Open window automatically

    }

    public void CloseModalWindow(){
        modalWindowManager.UpdateUI(); // Update UI
        //modalWindowManager.OpenWindow(); // Open window
        modalWindowManager.CloseWindow(); // Close window
        modalWindowManager.AnimateWindow(); // Close/Open window automatically

    }
}
