using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("Points")]
    [SerializeField] int currentPoints = 0;
    [SerializeField] int allPoints = 0;

    [Header("References")]
    [SerializeField] Inventory inventory = null;

    [Header("Multiplayer Objects")]
    [SerializeField] GameObject[] myView = null;
    [SerializeField] Camera camToDisable = null;
    [SerializeField] SkinnedMeshRenderer[] s_renderForTeam = null;
    [SerializeField] MeshRenderer[] renderForTeam = null;

    InputManager inputManager;
    ZoneManager zoneManager;
    PlayerGUI gui;
    PlayerHealth health;
    
    Interactable interaction = null;

    float mouseScrollY;

    void Start(){
        if(photonView.IsMine || GameManager.Instance.IsGameOffline()){
            foreach(GameObject check in myView)
                check.SetActive(true);

            if(camToDisable != null)
                camToDisable.enabled = true;

            foreach(SkinnedMeshRenderer check in s_renderForTeam)
                check.enabled = false;
          
            foreach(MeshRenderer check in renderForTeam)
                check.enabled = false;
        }else if(!photonView.IsMine){
            foreach(GameObject check in myView)
                check.SetActive(false);

            if(camToDisable != null)
                camToDisable.enabled = false;

            foreach(SkinnedMeshRenderer check in s_renderForTeam)
                check.enabled = true;

            foreach(MeshRenderer check in renderForTeam)
                check.enabled = true;
        }

        inputManager = InputManager.Instance;
        zoneManager = ZoneManager.Instance;

        gui = GetComponentInChildren<PlayerGUI>();
        health = GetComponentInChildren<PlayerHealth>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        AddPoints(500);
    }

    private void Update() {
        
        if(!photonView.IsMine)
            return;

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
    }

    [PunRPC]
    void Test(){
        Debug.Log("TESTED");
    }

    public void AddPoints(int pointsToAdd){
        if(!photonView.IsMine)
            return;

        allPoints += pointsToAdd;
        currentPoints += pointsToAdd;

        gui.UpdateDisplayedPoints(currentPoints);
    }

    public void RemovePoints(int pointsToRemove){
        if(!photonView.IsMine)
            return;

        currentPoints -= pointsToRemove;

        gui.UpdateDisplayedPoints(currentPoints);
    }

    public void AddWeaponToInventory(Weapon weapon){
        if(!photonView.IsMine)
            return;

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
        if(!photonView.IsMine)
            return;

        if(other.GetComponent<Zone>()){
            zoneManager.AddNewMainZone(other.GetComponent<Zone>());
        }
        
        if(other.GetComponent<Interactable>()){
            interaction = other.GetComponent<Interactable>();
            gui.InterationPopup(interaction.GetInteractionText(), true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(!photonView.IsMine)
            return;

        if(other.GetComponent<Zone>()){
            zoneManager.RemoveZones(other.GetComponent<Zone>());
        }

        interaction = null;
        gui.InterationPopup("", false);
    }
}
