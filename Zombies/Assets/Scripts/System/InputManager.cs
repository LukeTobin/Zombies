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

    private void Awake()
    {
        if(_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        playerControls = new PlayerControls();
        Cursor.visible = false;
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
}
