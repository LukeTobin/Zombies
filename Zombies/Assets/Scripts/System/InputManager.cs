using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    private static InputManager _instance;
    public static InputManager Instance
    {
        get { return _instance; }
    }

    PlayerControls playerControls;

    // held buttons
    bool isSprinting = false;
    bool triggerHeld = false;
    bool aimHeld = false;

    float mouseScrollY;

    private void Awake()
    {
        if(_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        playerControls = new PlayerControls();
        Cursor.visible = false;

        playerControls.Player.Sprint.performed += _ => SetIsSprinting(true);
        playerControls.Player.Sprint.canceled += _ => SetIsSprinting(false);

        playerControls.Player.Fire.performed += _ => SetTriggerHeld(true);
        playerControls.Player.Fire.canceled += _ => SetTriggerHeld(false);

        playerControls.Player.Aim.performed += _ => SetAimHeld(true);
        playerControls.Player.Aim.canceled += _ => SetAimHeld(false);

        playerControls.Player.WeaponSwapping.performed += x => mouseScrollY = x.ReadValue<float>();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public Vector3 GetPlayerMovement()
    {
        return playerControls.Player.Movement.ReadValue<Vector2>();
    }

    public Vector3 GetMouseDelta()
    {
        return playerControls.Player.Look.ReadValue<Vector2>();
    }

    public bool PlayerJumpedThisFrame()
    {
        return playerControls.Player.Jump.triggered;
    }

    public bool FireButtonPressed()
    {
        return playerControls.Player.Fire.triggered;
    }

    public bool ReloadButtonPressed(){
        return playerControls.Player.Reload.triggered;
    }

    public bool InteractButtonPressed(){
        return playerControls.Player.Interact.triggered;
    }

    public bool PlayerIsSprinting(){
        return isSprinting;
    }

    public bool PlayerTriggerHeld(){
        return triggerHeld;
    }

    public bool PlayerAimHeld(){
        return aimHeld;
    }

    public float GetMouseScrollY(){
        return mouseScrollY;
    }



    // Private Methods
    void SetIsSprinting(bool cond){
        isSprinting = cond;
    }

    void SetTriggerHeld(bool cond){
        triggerHeld = cond;
    }

    void SetAimHeld(bool cond){
        aimHeld = cond;
    }
}
