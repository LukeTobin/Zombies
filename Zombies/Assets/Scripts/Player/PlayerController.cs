using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Points")]
    [SerializeField] int currentPoints = 0;
    [SerializeField] int allPoints = 0;

    InputManager inputManager;
    ZoneManager zoneManager;
    PlayerGUI gui;
    
    Interactable interaction = null;

    void Start(){
        inputManager = InputManager.Instance;
        zoneManager = ZoneManager.Instance;

        gui = GetComponentInChildren<PlayerGUI>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        AddPoints(500);
    }

    private void Update() {
        if(interaction != null){
            if(inputManager.InteractButtonPressed()){
                int costOfInteraction = interaction.GetInteractionCost();
                if(costOfInteraction <= currentPoints){
                    interaction.Interact();
                    RemovePoints(costOfInteraction);

                    interaction = null;
                    gui.InterationPopup("", false);
                } else{
                    // highlight point function
                    Debug.Log("Insufficent Funds");
                }          
            }
        }    
    }

    public void AddPoints(int pointsToAdd){
        allPoints += pointsToAdd;
        currentPoints += pointsToAdd;

        gui.UpdateDisplayedPoints(currentPoints);
    }

    public void RemovePoints(int pointsToRemove){
        currentPoints -= pointsToRemove;

        gui.UpdateDisplayedPoints(currentPoints);
    }

    public int GetCurrentPoints(){
        return currentPoints;
    }

    public int GetAllPoints(){
        return allPoints;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<Zone>()){
            zoneManager.AddNewMainZone(other.GetComponent<Zone>());
        }
        
        if(other.GetComponent<Interactable>()){
            interaction = other.GetComponent<Interactable>();
            gui.InterationPopup(interaction.GetInteractionText(), true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.GetComponent<Zone>()){
            zoneManager.RemoveZones(other.GetComponent<Zone>());
        }

        interaction = null;
        gui.InterationPopup("", false);
    }
}
