using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Points")]
    [SerializeField] int currentPoints = 0;
    [SerializeField] int allPoints = 0;

    [Header("References")]
    [SerializeField] Inventory inventory = null;

    InputManager inputManager;
    ZoneManager zoneManager;
    PlayerGUI gui;
    PlayerHealth health;
    
    Interactable interaction = null;

    float mouseScrollY;

    void Start(){
        inputManager = InputManager.Instance;
        zoneManager = ZoneManager.Instance;

        gui = GetComponentInChildren<PlayerGUI>();
        health = GetComponentInChildren<PlayerHealth>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        AddPoints(500);
    }

    private void Update() {
        // CHECK FOR INTERACTIONS
        if(interaction != null){
            if(inputManager.InteractButtonPressed()){
                int costOfInteraction = interaction.GetInteractionCost();
                if(costOfInteraction <= currentPoints){
                    interaction.Interact(this);
                    RemovePoints(costOfInteraction);

                    interaction = null;
                    gui.InterationPopup("", false);
                } else{
                    // highlight point function
                    Debug.Log("Insufficent Funds");
                }          
            }
        }

        // WEAPON SWAPPING
        mouseScrollY = inputManager.GetMouseScrollY();
        if(mouseScrollY > 0 || mouseScrollY < 0){
            if(inventory != null)
                inventory.SwapWeapon(mouseScrollY);
        }

        // WEAPON SWAY
        
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

    public void AddWeaponToInventory(Weapon weapon){
        inventory.AddNewWeapon(weapon);
    }

    public int GetCurrentPoints(){
        return currentPoints;
    }

    public int GetAllPoints(){
        return allPoints;
    }

    public void TakeDamage(float damage){
        health.TakeDamage(damage);
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
