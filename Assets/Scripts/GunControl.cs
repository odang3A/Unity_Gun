using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControl : MonoBehaviour
{
    // gun type, info
    string gunName;
    Vector3 hipLoc;     // hip location
    Vector3 aimLoc;     // aim location
    float damage;       // damage amount (knockback force)
    float fireRate;     // fire rate
    float reloadTime;  // reload time
    float accuracy;       // accuracy while hip fireing
    float recoil;       // recoil
    bool isSelectiveFire;   // is selective fire
    int capacity;       // capacity
    int currAmmo;       // current ammo amount

    bool isEquip = false;   // is equiped
    bool isFireable = false;    // is fireable
    bool isReloading = false;   // is reloading
    IEnumerator reloadCoroutine; // reload coroutine
    bool isAiming = false;      // is aiming
    int fireMode;       // fire mode    // -1: single, 1: auto
    int currMag;        // current bullets in mag
    float nextTimeToFire = 0f;  // time to fire
    Transform FPSCamera; // FPSCamera (players eye)
    HUDControl hudControl;  // HUD
    BoxCollider[] gunColliders; // guns colliders
    Rigidbody gunRb;    // guns rigidbody
    Animator gunAnimator;   // guns animator

    // effects
    public ParticleSystem muzzleFlash;  // muzzle flash effect
    public GameObject hitImpact;        // hit impact effect


    void Start()
    {
        gunColliders = GetComponents<BoxCollider>();    // get guns colliders
        gunRb = GetComponent<Rigidbody>();              // get guns rigidbody
        gunAnimator = GetComponent<Animator>(); // get guns animator
        gunAnimator.enabled = false; // stops animation
    }

    // Update is called once per frame
    void Update()
    {
        if (isEquip)
        {
            control();  // main control of the guns (fire, aim, fire mode)


            // 실행순서 주의할것

            if (Input.GetKeyDown("x"))      // unequip guns
            {
                unequip();
            }
        }
    }

    void control()  // main control
    {   
        // change fire mode
        if (Input.GetKeyDown("b") && isSelectiveFire)
        {
            fireMode *= -1;     // -1: single, 1: auto
            hudControl.changeFireMode(fireMode);    // update fire mode HUD
        }
        
        // check fire mode
        switch (fireMode)
        {   // -1: single, 1: auto
            case -1:
                isFireable = Input.GetMouseButtonDown(0);   // single
                break;
            case 1:
                isFireable = Input.GetMouseButton(0) && Time.time >= nextTimeToFire;    // auto
                break;
            default:
                break;
        }

        // fire
        if (isFireable && currMag > 0)   // if able to fire
        {
            if (isReloading)    // if was reloading
            {
                gunAnimator.SetBool("isReloading", isReloading = false);    // cancle reload animation
                if (reloadCoroutine != null) { StopCoroutine(reloadCoroutine); }    // cancle reload
            }
            fire();
        }

        // aim
        if (Input.GetMouseButtonDown(1) && currMag > 0)    // enable aim
        {
            forceToIdle();                  // force to idle
            isAiming = true;
        }
        if (Input.GetMouseButtonUp(1))      // disable aim
        {
            isAiming = false;
        }
        aim(isAiming);                      // set aim

        // reload
        if (!isReloading && (currMag < capacity + 1))   // if able to reload
        {
            if (Input.GetKeyDown("r") || currMag <= 0)       // reload gun
                {
                    forceToIdle();              // force to idle
                    isReloading = true;         // set isReloading true
                    reloadCoroutine = reload();
                    StartCoroutine(reloadCoroutine);   // start reload coroutine
                }
        }
    }

    void fire()     // fire
    {
        Vector3 spread = Vector3.zero;

        currMag--;  // pop one bullet
        hudControl.updateAmmo(currMag, currAmmo);   // update Ammo HUD

        nextTimeToFire = Time.time + 1f / fireRate;     // fire rate

        muzzleFlash.Play(); // play muzzle flash effects

        if (FPSCamera.GetComponent<Camera>().fieldOfView == 60f)  // if hip fireing apply accuracy
        {
            spread = Random.insideUnitSphere/3f * (1f - accuracy);
            Debug.Log(spread);
        }

        RaycastHit hit;
        if (Physics.Raycast(FPSCamera.position, FPSCamera.forward + spread, out hit))   // if hit something
        {
            Debug.Log("We hit " + hit.collider.name + ": " + hit.point);    // hit log

            if (hit.rigidbody)   // knockback
            {
                //hit.rigidbody.AddExplosionForce(damage, hit.point, 5, 0.2f, ForceMode.Impulse);
                hit.rigidbody.AddForce(transform.forward * damage, ForceMode.Impulse);  // knockback by damage
            }

            // play hit Impact effects
            GameObject impactObj = Instantiate(hitImpact, hit.point, Quaternion.LookRotation(hit.normal));  // instantiate hit impact effect
            Destroy(impactObj, 1);  // destroy hit impact effect
        }

        // start fire animation
        gunAnimator.SetTrigger("Fire");
    }

    void aim(bool isAimed)  // aim
    {
        // control with animation
        // transform.localPosition = (isAimed ? aimLoc : hipLoc);  // gun to aim location
        gunAnimator.SetBool("isAiming", isAimed);   // toggle idle and aim animation
        if(transform.localPosition == aimLoc && isAimed)
        {
            FPSCamera.GetComponent<Camera>().fieldOfView = 30f;   // zoom in
            gunAnimator.SetBool("isAimed", true);   // set is aimed
        }
        else if (!isAimed)
        {
            FPSCamera.GetComponent<Camera>().fieldOfView = 60f;   // zoom out
            gunAnimator.SetBool("isAimed", false);  // set is not aimed
        }
        hudControl.showCrosshair(!isAimed);  // disable crosshair HUD
    }

    IEnumerator reload()   // reload
    {
        gunAnimator.SetBool("isReloading", isReloading);   // start playing reload animation
        yield return new WaitForSeconds(reloadTime);  // wait for reload time

        gunAnimator.SetBool("isReloading", isReloading = false);  // end playing reload animation
        currMag = capacity + 1;                  // fill mag
        hudControl.updateAmmo(currMag, currAmmo);   // update ammo
    }

    void forceToIdle()
    {
        gunAnimator.SetBool("isReloading", isReloading = false);    // cancle reload animation
        if (reloadCoroutine != null) { StopCoroutine(reloadCoroutine); }    // cancle reload
        gunAnimator.SetBool("isAiming", isAiming = false);      // cancle aiming animation
    }

    public void unequip()   // unequip guns
    {
        forceToIdle();
        isEquip = false;    // unequip
        FPSCamera.GetComponent<Camera>().fieldOfView = 60f;     // reset zoom
        hudControl.showCrosshair(true);
        hudControl.showGunInfo(isEquip);    // disable guninfo UHD
        transform.parent = transform.root.parent;   // set parent to null
        foreach (BoxCollider gunCollider in gunColliders)   // enable colliders
        {
            gunCollider.enabled = true;
        }

        gunRb.useGravity = true;    // set physics
        gunAnimator.enabled = false;     // stops animation
    }

    public void SetGunType(string name, Vector3 hipLoc, Vector3 aimLoc, float damage, float fireRate, float reloadTime, float accuracy, bool isSelectiveFire, int defaultFireMode, int capacity)
    {
        gunName = name;         // name
        this.hipLoc = hipLoc;   // hip location
        this.aimLoc = aimLoc;   // aim location
        this.damage = damage;   // damage (knockback force)
        this.fireRate = fireRate;   // fire rate
        this.reloadTime = reloadTime;   // reloadTime
        this.accuracy = accuracy;       // accuracy while hip fireing
        this.isSelectiveFire = isSelectiveFire; // is selective fire
        fireMode = defaultFireMode;     // default fire mode
        this.capacity = capacity;       // gun capacity
        currAmmo = this.capacity + 1;   // default ammo
        currMag = this.capacity + 1;    // fill mag
    }


    public void Selected(Transform selector)    // equip
    {
        isEquip = true;
        FPSCamera = selector;
        hudControl = FPSCamera.parent.Find("HUD").GetComponent<HUDControl>();

        // disable colliders
        foreach (BoxCollider gunCollider in gunColliders)
        {
            gunCollider.enabled = false;
        }

        // set gun location, rotation, physics
        transform.SetParent(FPSCamera);
        // controle with animation
        //transform.localPosition = hipLoc;
        //transform.localRotation = Quaternion.identity;
        gunRb.velocity = Vector3.zero;
        gunRb.angularVelocity = Vector3.zero;
        gunRb.useGravity = false;
        gunAnimator.enabled = true; // starts animation

        // update HUD
        hudControl.showGunInfo(isEquip);        // show gun info UHD
        hudControl.updateAmmo(currMag, currAmmo);   // update ammo HUD
        hudControl.changeFireMode(fireMode);    // set fire mode HUD
    }
}
