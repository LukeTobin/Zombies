using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    [Header("Gun")]
    [SerializeField] int maxAmmoCount = 0;
    [SerializeField] int clipSize = 0;
    [Space]
    [SerializeField] int currentClipCount = 0;
    [SerializeField] int currentReserve = 0;
    [Space]
    [SerializeField] int bulletsPerShot = 1;
    [SerializeField] float reloadTime = 1f;
    [SerializeField] float bulletRange = 1000f;
    [SerializeField] bool isAutomatic = false;
    [Space]
    [SerializeField] LayerMask ignoreZone = 8;
    
    InputManager inputManager;
    Camera cam;
    Inventory inventory;

    bool reloading;

    private void Awake()
    {    
        cam = Camera.main;
        inventory = GetComponentInParent<Inventory>();
    }

    private void Start()
    {
        inputManager = InputManager.Instance;

        currentClipCount = clipSize;
        currentReserve = maxAmmoCount - clipSize;

        inventory.UpdateBulletCount(currentClipCount, currentReserve);
    }

    private void Update()
    {
        if (inputManager.FireButtonPressed() && !reloading)
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
            Debug.Log(hit.collider + " was hit.");
            Zombie zombie = hit.transform.GetComponent<Zombie>();
            if(zombie != null){
                if(zombie.TryKill(weaponDamage)){
                    inventory.AddKillPoints();
                }
            }
        }

        currentClipCount -= bulletsPerShot;
        inventory.UpdateBulletCount(currentClipCount, currentReserve);
    }

    void TryReload(){
        if(currentReserve > 0){
            reloading = true;
            StartCoroutine(WaitForReload());
            //StartCoroutine(LowerWeapon());
        }
    }

    IEnumerator WaitForReload(){
        yield return new WaitForSeconds(reloadTime);
        
        if(currentClipCount > 0){
            if((currentReserve + currentClipCount) >= clipSize){
                currentReserve -= (clipSize - currentClipCount);
                currentClipCount = clipSize;
                
            }else{
                currentClipCount = currentClipCount + currentReserve;
                currentReserve = 0;
            }
        }else{
            if(currentReserve >= clipSize){
                currentClipCount = clipSize;
                currentReserve -= clipSize;
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
}
