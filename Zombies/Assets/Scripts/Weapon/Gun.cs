using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Gun : Weapon
{
    #region Variables

    #region Public variables

    [Header("Gun")]
    [Header("Ammo")]
    [SerializeField] int maxAmmoCount = 0;
    [SerializeField] int clipSize = 0;
    [Space]
    [SerializeField] int currentClipCount = 0;
    [SerializeField] int currentReserve = 0;
    [Space]
    [Header("Bullets")]
    [SerializeField] float fireRate = 0.05f;
    [SerializeField] float adsRate = 1.5f;
    [SerializeField] int bulletsPerShot = 1;
    [SerializeField] float bulletRange = 1000f;
    [SerializeField] float reloadTime = 1f;
    [SerializeField] bool isAutomatic = false;
    [Space]
    [Header("Recoil")]
    [SerializeField] float bulletSpread = 20f;
    [SerializeField] float adsSpreadControl = 3f;
    [SerializeField] float maxRecoilVertical = -20f;
    [SerializeField] float maxRecoilHorizontal = 20f;
    [SerializeField] float recoilSpeed = 2f;
    [SerializeField] float recoilStabilizeSpeed = 2f;
    [SerializeField] Vector2 adsRecoilControl = new Vector2(1, 1);
    [Space]
    [SerializeField] float adsX = 0f;
    [SerializeField] float adsY = 0f;
    [SerializeField] float adsZ = 0f;
    [Space]
    [Header("Effects")]
    [SerializeField] GameObject decalPrefab = null;
    [SerializeField] ParticleSystem bulletFire = null;
    [SerializeField] ParticleSystem bloodParticles = null;
    [SerializeField] ParticleSystem impactParticles = null;
    [Space]
    [SerializeField] AudioSource bulletSFX = null;
    [Space]
    [Header("Weapon Sway")]
    [SerializeField] float swayIntensity = 0.8f;
    [SerializeField] float swaySmoothing = 1.6f;
    [SerializeField] float adsFOVSpeed = 0.03f;
    [Space]
    [Header("Requires")]
    [Tooltip("Ignore Zone, Weapon Buy's and any Collider based objects that don't need to be hit by bullets.")]
    [SerializeField] LayerMask ignoreZone = 8;
    [Space]
    
    #endregion

    #region Private Variables

    InputManager inputManager;
    Camera cam;
    CinemachineVirtualCamera vcam;
    
    Inventory inventory;

    bool reloading;
    bool triggerHeld;
    bool aimHeld;

    float timeBetweenShot;
    float defaultFOV;
    float recoil = 0f;
    float tempMaxRecoilHorizontal;

    Quaternion rotationOrigin;

    Vector3 hipLocation;
    Vector3 hipAngle;

    #endregion
    
    #endregion

    #region Callbacks

    private void Awake()
    {    
        cam = Camera.main;
        inventory = GetComponentInParent<Inventory>();
        vcam = inventory.ReturnCinemachineVCam();

        hipLocation = transform.localPosition;
        hipAngle = transform.localEulerAngles;
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

        bulletSFX.UnPause();
    }

    private void Update()
    {
        // Apply Gun Sway
        UpdateSway();

        // Check if player is aiming down sights & if so aim the weapon
        aimHeld = inputManager.PlayerAimHeld();
        AimGun(aimHeld);

        // Check if you're holding the fire trigger
        if(isAutomatic)
            triggerHeld = inputManager.PlayerTriggerHeld();
        

        // Autofire for when trigger is held
        if(triggerHeld && !reloading && isAutomatic){
            if(timeBetweenShot > 0){
                timeBetweenShot -= Time.deltaTime;
            }
            else if(timeBetweenShot <= 0){
                if(currentClipCount > 0){
                    Shoot();
                }
                else
                    TryReload();
            }
        }else if(!triggerHeld && timeBetweenShot != 0){
            timeBetweenShot = fireRate;
        }

        // Single-fire
        if (inputManager.FireButtonPressed() && !reloading && !isAutomatic)
        {
            if(currentClipCount > 0){
                Shoot();
            }
            else
                TryReload();
        }

        // Apply any needed Recoil for when firing
        Recoiling();

        // Check if player wants to reload
        if(inputManager.ReloadButtonPressed() && !reloading){
            TryReload();
        }
    }

    #endregion

    #region Private Methods

    void Shoot()
    {
        // Create Raycast & Forward Vector
        RaycastHit hit;
        Vector3 forwardVector = Vector3.forward;

        // Apply Weapon Recoil
        StartRecoil(0.2f, maxRecoilHorizontal);

        // Play Audio for Firing
        if(bulletSFX != null)
            bulletSFX.Play();


        // Adjust forward vector based on bullet spread
        if(!aimHeld){
            Vector3 deviation3D = Random.insideUnitCircle * bulletSpread;
            Quaternion rot = Quaternion.LookRotation(Vector3.forward * bulletRange + deviation3D);
            forwardVector = cam.transform.rotation * rot * Vector3.forward;
        }
        else{
            Vector3 deviation3D = Random.insideUnitCircle * (bulletSpread / adsSpreadControl);
            Quaternion rot = Quaternion.LookRotation(Vector3.forward * bulletRange + deviation3D);
            forwardVector = cam.transform.rotation * rot * Vector3.forward;
        }

        // Check if raycast hit anything
        if(Physics.Raycast(cam.transform.position, forwardVector, out hit, bulletRange, ~ignoreZone))
        {
            Zombie zombie = hit.transform.GetComponent<Zombie>();
            if(zombie != null){
                if(bloodParticles != null){
                    GameObject blood = ObjectPool.SharedInstance.GetPooledObject(ObjectPool.ObjectType.BloodFX);
                    if(blood != null){
                        blood.transform.position = hit.point;
                        blood.transform.rotation = transform.rotation;
                        blood.SetActive(true);

                        StartCoroutine(RemoveEffect(blood));
                    }
                }
                if(zombie.TryKill(weaponDamage)){
                    inventory.AddKillPoints();
                }
            } else{
                GameObject bulletDecal = Instantiate(decalPrefab, hit.point + hit.normal * 0.001f, Quaternion.identity) as GameObject;
                bulletDecal.transform.LookAt(hit.point + hit.normal);
                Destroy(bulletDecal, 7f);
            }
        }

        // Finish shot and update relevent information
        currentClipCount -= bulletsPerShot;
        inventory.UpdateBulletCount(currentClipCount, currentReserve);

        timeBetweenShot = fireRate;
    }

    void StartRecoil(float _recoil, float _maxRecoil_x)
    {
        // in seconds
        recoil = _recoil;
        tempMaxRecoilHorizontal = Random.Range(-_maxRecoil_x, _maxRecoil_x);
    }

    void Recoiling()
    {
        if (recoil > 0f){
            if(aimHeld){
                // Get Euler Angle and dampen towards the target rotation
                float tempMaxRecoilVetical = maxRecoilVertical / adsRecoilControl.y;
                tempMaxRecoilHorizontal /= adsRecoilControl.x;

                // Filled in (y,x) to get intended results
                Quaternion maxRecoil = Quaternion.Euler(tempMaxRecoilVetical, tempMaxRecoilHorizontal, 0f);
                transform.localRotation = Quaternion.Slerp (transform.localRotation, maxRecoil, Time.deltaTime * recoilSpeed);
            }else{
                // Get Euler Angle and dampen towards the target rotation
                Quaternion maxRecoil = Quaternion.Euler(maxRecoilVertical, tempMaxRecoilHorizontal, 0f);              
                transform.localRotation = Quaternion.Slerp (transform.localRotation, maxRecoil, Time.deltaTime * recoilSpeed);
            }
            recoil -= Time.deltaTime;
        } else{
            recoil = 0f;
            transform.localRotation = Quaternion.Slerp (transform.localRotation, Quaternion.identity, Time.deltaTime * (recoilSpeed * recoilStabilizeSpeed));
        }
    }

    void TryReload(){
        if(currentReserve > 0 && !reloading){
            reloading = true;
            StartCoroutine(WaitForReload());
            //StartCoroutine(LowerWeapon());
        }
    }

    void UpdateSway(){
        Vector2 mouseRotation = inputManager.GetMouseDelta();

        if(aimHeld){
            Quaternion adjacentX = Quaternion.AngleAxis((-swayIntensity/4) * mouseRotation.x, Vector3.up);
            Quaternion adjacentY = Quaternion.AngleAxis((swayIntensity/4) * mouseRotation.y, Vector3.right);
            Quaternion targetRotation = rotationOrigin * adjacentX * adjacentY;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * (swaySmoothing * 6));
        }       
        else{
            Quaternion adjacentX = Quaternion.AngleAxis(-swayIntensity * mouseRotation.x, Vector3.up);
            Quaternion adjacentY = Quaternion.AngleAxis(swayIntensity * mouseRotation.y, Vector3.right);
            Quaternion targetRotation = rotationOrigin * adjacentX * adjacentY;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * swaySmoothing);
        }
    }

    void AimGun(bool isAiming){
        if(isAiming){
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, new Vector3(adsX, adsY, adsZ), (Time.deltaTime * adsRate));
            vcam.m_Lens.FieldOfView = Mathf.Lerp(vcam.m_Lens.FieldOfView, (defaultFOV / 1.5f), adsFOVSpeed);         
        }else if(transform.localPosition != hipLocation){
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, hipLocation, Time.deltaTime * adsRate);
            vcam.m_Lens.FieldOfView = Mathf.Lerp(vcam.m_Lens.FieldOfView, defaultFOV, adsFOVSpeed); 
        }
    }

    #endregion

    #region Enumerations
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

    IEnumerator RemoveEffect(GameObject efx){
        yield return new WaitForSeconds(1f);
        ObjectPool.SharedInstance.ReturnPooledObject(efx, ObjectPool.ObjectType.BloodFX);
    }

    #endregion

    #region Public returnable variables
    
    public int ReturnCurrentClipCount(){
        return currentClipCount;
    }

    public int ReturnCurrentAmmoReserve(){
        return currentReserve;
    }

    #endregion
}
