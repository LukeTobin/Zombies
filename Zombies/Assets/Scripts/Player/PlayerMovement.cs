using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private bool sprintingPlayer;
    
    [SerializeField] float playerSpeed = 1.8f;
    [SerializeField] float sprintMultiplier = 1.4f;
    [SerializeField] float adsMovementPenalty = 0.4f;
    [SerializeField] float jumpHeight = 1.0f;
    [SerializeField] float gravityValue = -9.81f;
    [Space]
    [SerializeField] Transform weaponParent;

    InputManager inputManager;
    Camera camera;
    CinemachineVirtualCamera virtualCamera;
    Transform cameraTransform;

    Vector3 weaponParentOrigin;
    Vector3 targetWeaponBobPosition;
    float idleCounter;
    float movementCounter;

    float defaultFOV;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        camera = Camera.main;
        cameraTransform = camera.transform;
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        weaponParentOrigin = weaponParent.localPosition;
        defaultFOV = virtualCamera.m_Lens.FieldOfView;
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        sprintingPlayer = inputManager.PlayerIsSprinting();

        Vector2 movement = inputManager.GetPlayerMovement();
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0f;
        
        if(inputManager.PlayerAimHeld())
            controller.Move(move * Time.deltaTime * (playerSpeed * adsMovementPenalty));
        else if(sprintingPlayer){
            if(move.x != 0 || move.y != 0)
                virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, (defaultFOV * 1.1f), Time.deltaTime * 5f);
            controller.Move(move * Time.deltaTime * (playerSpeed * sprintMultiplier));
        }         
        else{
            virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, defaultFOV, Time.deltaTime * 5f);
            controller.Move(move * Time.deltaTime * playerSpeed);
        }
        
        /*if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }*/

        // Changes the height position of the player..
        if (inputManager.PlayerJumpedThisFrame() && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // Weapon Bobbing
        bool aiming = inputManager.PlayerAimHeld();
        if(movement.x != 0 || movement.y != 0 && !sprintingPlayer && !aiming){
            Headbob(movementCounter, 0.010f, 0.010f);
            movementCounter += Time.deltaTime * 4;
            idleCounter = 0;
            weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 6f);
        }else if(movement.x != 0 || movement.y != 0 && sprintingPlayer && !aiming){
            Headbob(movementCounter, 0.02f, 0.012f);
            movementCounter += Time.deltaTime * 6;
            idleCounter = 0;
            weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 10f);
        }else if(movement.x != 0 || movement.y != 0 && aiming){
            Headbob(movementCounter, 0.002f, 0.002f);
            movementCounter += Time.deltaTime * 2;
            idleCounter = 0;
            weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 4f);
        }else if(aiming){
            Headbob(idleCounter, 0.0005f, 0.0005f);
            idleCounter += Time.deltaTime;
            movementCounter = 0;
            weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 2f);
        }else{
            Headbob(idleCounter, 0.005f, 0.005f);
            idleCounter += Time.deltaTime;
            movementCounter = 0;
            weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 2f);
        }
    } 

    void Headbob(float c, float xIntensity, float yIntensity){
        targetWeaponBobPosition = weaponParentOrigin + new Vector3(Mathf.Cos(c) * xIntensity, Mathf.Sin(c * 2) * yIntensity, 0); 
    }
}
