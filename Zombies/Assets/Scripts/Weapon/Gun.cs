using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Gun : Weapon
{
    [Header("Gun")]
    [SerializeField] int maxAmmoCount = 0;
    [SerializeField] int clipSize = 0;
    [Space]
    [SerializeField] int currentClipCount = 0;
    [SerializeField] int currentReserve = 0;
    [Space]
    [SerializeField] float fireRate = 0.05f;
    [SerializeField] float adsRate = 5f;
    [SerializeField] int bulletsPerShot = 1;
    [SerializeField] float reloadTime = 1f;
    [SerializeField] float bulletRange = 1000f;
    [SerializeField] bool isAutomatic = false;
    [Space]
    [SerializeField] float adsX = 0f;
    [SerializeField] float adsY = 0f;
    [SerializeField] float adsZ = 0f;
    [Space]
    [SerializeField] GameObject decalPrefab = null;
    [Space]
    [SerializeField] float swayIntensity = 1f;
    [SerializeField] float swaySmoothing = 2f;
    [SerializeField] float adsFOVSpeed = 5f;
    [Space]
    [SerializeField] LayerMask ignoreZone = 8;
    [Space]
    
    InputManager inputManager;
    Camera cam;
    CinemachineVirtualCamera vcam;
    
    Inventory inventory;

    bool reloading;
    bool triggerHeld;

    float timeBetweenShot;
    float defaultFOV;

    Quaternion rotationOrigin;

    Vector3 hipLocation;

    private void Awake()
    {    
        cam = Camera.main;
        vcam = FindObjectOfType<CinemachineVirtualCamera>();
        inventory = GetComponentInParent<Inventory>();
        hipLocation = transform.localPosition;
        defaultFOV = cam.fieldOfView;
    }

    private void Start()
    {
        inputManager = InputManager.Instance;

        currentClipCount = clipSize;
        currentReserve = maxAmmoCount - clipSize;
        rotationOrigin = transform.localRotation;

        if(inventory != null)
            inventory.UpdateBulletCount(currentClipCount, currentReserve);
    }

    private void Update()
    {
        UpdateSway();

        if(isAutomatic)
            triggerHeld = inputManager.PlayerTriggerHeld();

        AimGun(inputManager.PlayerAimHeld());

        // Autofire for when trigger is held
        if(triggerHeld && !reloading && isAutomatic){
            if(timeBetweenShot > 0){
                timeBetweenShot -= Time.deltaTime;
            }
            else if(timeBetweenShot <= 0){
                if(currentClipCount > 0)
                    Shoot();
                else
                    TryReload();
            }
        }else if(!triggerHeld && timeBetweenShot != 0){
            timeBetweenShot = fireRate;
        }

        // Single-fire
        if (inputManager.FireButtonPressed() && !reloading && !isAutomatic)
        {
            if(currentClipCount > 0)
                Shoot();
            else
                TryReload();
        }

        if(inputManager.ReloadButtonPressed() && !reloading){
            TryReload();
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, bulletRange, ~ignoreZone))
        {
            // Debug.Log(hit.collider + " was hit.");
            Zombie zombie = hit.transform.GetComponent<Zombie>();
            if(zombie != null){
                if(zombie.TryKill(weaponDamage)){
                    inventory.AddKillPoints();
                }
            } else{
                GameObject bulletDecal = Instantiate(decalPrefab, hit.point + hit.normal * 0.001f, Quaternion.identity) as GameObject;
                bulletDecal.transform.LookAt(hit.point + hit.normal);
                Destroy(bulletDecal, 7f);
            }
        }

        currentClipCount -= bulletsPerShot;
        inventory.UpdateBulletCount(currentClipCount, currentReserve);

        timeBetweenShot = fireRate;
    }

    void TryReload(){
        if(currentReserve > 0 && !reloading){
            reloading = true;
            StartCoroutine(WaitForReload());
            //StartCoroutine(LowerWeapon());
        }
    }

    IEnumerator WaitForReload(){
        yield return new WaitForSeconds(reloadTime);
        
        // if your current clip isnt empty
        if(currentClipCount > 0){
            // check if there is more in your reserve than their is in a single clip
            if((currentReserve + currentClipCount) >= clipSize){
                currentReserve -= (clipSize - currentClipCount);
                currentClipCount = clipSize;
                
            }else{
                // if there isnt enough to fill a full clip, add the remainder of the reserve
                currentClipCount = currentClipCount + currentReserve;
                currentReserve = 0;
            }
        }else{
            // if your current clip is empty & their is enough in your reserve to fill the remainder of the clip
            if(currentReserve >= clipSize){
                currentClipCount = clipSize;
                currentReserve -= clipSize;
            }
            // if there isnt enough to fill a full clip
            else{
                currentClipCount = currentClipCount + currentReserve;
                currentReserve = 0;
            }
        }

        //StartCoroutine(RaiseWeapon());
        inventory.UpdateBulletCount(currentClipCount, currentReserve);
        reloading = false;
    }  

    IEnumerator LowerWeapon(){
        while(transform.position.x != -0.4f){
            transform.position -= new Vector3(1, 1) * (Time.deltaTime * 10f);
            yield return null;
        }
    }

    IEnumerator RaiseWeapon(){
        while(transform.position.x != 0.15f){
            transform.position += new Vector3(1, 1) * (Time.deltaTime * 10f);
            yield return null;
        }
    }

    void UpdateSway(){
        Vector2 mouseRotation = inputManager.GetMouseDelta();
        Quaternion adjacentX = Quaternion.AngleAxis(-swayIntensity * mouseRotation.x, Vector3.up);
        Quaternion adjacentY = Quaternion.AngleAxis(swayIntensity * mouseRotation.y, Vector3.right);
        Quaternion targetRotation = rotationOrigin * adjacentX * adjacentY;

        if(!inputManager.PlayerAimHeld())
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * swaySmoothing);
        else
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * (swaySmoothing * 4));
    }

    void AimGun(bool isAiming){
        if(isAiming){
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(adsX, adsY, adsZ), Time.deltaTime * adsRate);
            vcam.m_Lens.FieldOfView = Mathf.Lerp(vcam.m_Lens.FieldOfView, (defaultFOV / 1.5f), adsFOVSpeed);         
        }else if(transform.localPosition != hipLocation){
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, hipLocation, Time.deltaTime * adsRate);
            vcam.m_Lens.FieldOfView = Mathf.Lerp(vcam.m_Lens.FieldOfView, defaultFOV, adsFOVSpeed); 
        }
    }


    #region Return Values
    
    public int ReturnCurrentClipCount(){
        return currentClipCount;
    }

    public int ReturnCurrentAmmoReserve(){
        return currentReserve;
    }

    #endregion
}
