using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Points")]
    [SerializeField] int currentPoints = 0;
    [SerializeField] int allPoints = 0;

    [Header("References")]
    [SerializeField] Inventory inventory = null;

    [Header("Multiplayer Objects")]
    [SerializeField] GameObject[] myViewOnly = null;
    [SerializeField] GameObject[] teamViewOnly = null;
    [SerializeField] Camera camToDisable = null;
    [SerializeField] SkinnedMeshRenderer[] meshRenderForTeamOnly = null;
    
    [Header("Debug")]
    [SerializeField] float value;
    [SerializeField] float real_value;
    [Space]
    [SerializeField] float cachedRotAngle = 0;

    InputManager inputManager;
    ZoneManager zoneManager;
    PlayerGUI gui;
    PlayerHealth health;
    
    Interactable interaction = null;

    float mouseScrollY;

    public void OnPhotonSerializeView(PhotonStream p_stream, PhotonMessageInfo p_message){
        if(p_stream.IsWriting){
            //cachedRotAngle = camToDisable.transform.localRotation.y;
            //p_stream.SendNext((Quaternion)currentRotation);
        }else{
            //readRotation = (Quaternion)p_stream.ReceiveNext();
        }
    }

    void Start(){
        if(photonView.IsMine){
            foreach(GameObject check in myViewOnly)
                check.SetActive(true);

            if(teamViewOnly != null){
                foreach(GameObject check in teamViewOnly)
                    check.SetActive(false);
            }

            if(camToDisable != null){
                camToDisable.enabled = true;
                camToDisable.GetComponent<AudioListener>().enabled = true;
            }

            if(meshRenderForTeamOnly != null){
                foreach(SkinnedMeshRenderer check in meshRenderForTeamOnly){
                    check.enabled = false;
                }
            }
            
        }else if(!photonView.IsMine){
            foreach(GameObject check in myViewOnly)
                check.SetActive(false);

            if(teamViewOnly != null){
                foreach(GameObject check in teamViewOnly)
                    check.SetActive(true);
            }

            if(camToDisable != null){
                camToDisable.enabled = false;
                camToDisable.GetComponent<AudioListener>().enabled = false;
            }

            if(meshRenderForTeamOnly != null){
                foreach(SkinnedMeshRenderer check in meshRenderForTeamOnly){
                    check.enabled = true;
                }
            }     
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
        if(!photonView.IsMine){ 
            return;
        }

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
        weapon.PlayWeaponTakeAudio();
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
