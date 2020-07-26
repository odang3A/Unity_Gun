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
    bool isSelectiveFire;   // is selective fire
    int capacity;       // capacity
    int currAmmo;       // current ammo amount

    bool isEquip = false;   // is equiped
    bool isFireable = false;    // is fireable
    int fireMode;       // fire mode    // -1: single, 1: auto
    int currMag;        // current bullets in mag
    float nextTimeToFire = 0f;  // time to fire
    Transform FPSCamera; // FPSCamera (players eye)
    HUDControl hudControl;  // HUD
    BoxCollider[] gunColliders; // guns colliders
    Rigidbody gunRb;    // guns rigidbody

    // effects
    public ParticleSystem muzzleFlash;  // muzzle flash effect
    public GameObject hitImpact;        // hit impact effect


    void Start()
    {
        gunColliders = GetComponents<BoxCollider>();    // get guns colliders
        gunRb = GetComponent<Rigidbody>();              // get guns rigidbody
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

            if(Input.GetKeyDown("r") || currMag <= 0)
            {
                //StartCoroutine(reload());
                unequip();
                Debug.Log("tlqkf");
            }

        }
    }

    void control()
    {
        if (Input.GetKeyDown("b") && isSelectiveFire)   // change fire mode
        {
            fireMode *= -1;     // -1: single, 1: auto
            hudControl.changeFireMode(fireMode);    // update fire mode HUD
        }

        switch (fireMode)   // check fire mode
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


        if (isFireable && currMag > 0)   // fire
        {
            fire();
        }


        // aim
        aim(Input.GetMouseButton(1));   // set aim
    }

    void fire()
    {
        currMag--;  // 
        hudControl.updateAmmo(currMag, currAmmo);   // update Ammo HUD

        nextTimeToFire = Time.time + 1f / fireRate;     // fire rate

        muzzleFlash.Play(); // play muzzle flash effects

        RaycastHit hit;
        if (Physics.Raycast(FPSCamera.position, FPSCamera.forward, out hit))   // if hit something
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
    }

    void aim(bool isAimed)  // aim
    {   
        transform.localPosition = (isAimed ? aimLoc : hipLoc);  // gun to aim location
        FPSCamera.GetComponent<Camera>().fieldOfView = (isAimed ? 30f : 60f);   // zoom in
        hudControl.showCrosshair(isAimed);  // disable crosshair HUD
    }

    IEnumerator reload()   // reload
    {
        yield return new WaitForSeconds(1);
    }

    void unequip()   // unequip guns
    {
        aim(false); // disable aim
        isEquip = false;    // unequip
        hudControl.showGunInfo(isEquip);    // disable guninfo UHD
        transform.parent = transform.root.parent;   // set parent to null
        foreach (BoxCollider gunCollider in gunColliders)   // enable colliders
        {
            gunCollider.enabled = true;
        }
        gunRb.useGravity = true;    // set physics
    }

    public void SetGunType(string name, Vector3 hipLoc, Vector3 aimLoc, float damage, float fireRate, bool isSelectiveFire, int defaultFireMode, int capacity)
    {
        gunName = name;         // name
        this.hipLoc = hipLoc;   // hip location
        this.aimLoc = aimLoc;   // aim location
        this.damage = damage;   // damage (knockback force)
        this.fireRate = fireRate;   // fire rate
        this.isSelectiveFire = isSelectiveFire; // is selective fire
        fireMode = defaultFireMode;     // default fire mode
        this.capacity = capacity;       // gun capacity
        currAmmo = this.capacity + 1;   // default ammo
        currMag = this.capacity + 1;    // fill mag
    }


    public void Selected(Transform selector)
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
        transform.localPosition = hipLoc;
        transform.localRotation = Quaternion.identity;
        gunRb.angularVelocity = Vector3.zero;
        gunRb.useGravity = false;

        // update HUD
        hudControl.showGunInfo(isEquip);        // show gun info UHD
        hudControl.updateAmmo(currMag, currAmmo);   // update ammo HUD
        hudControl.changeFireMode(fireMode);    // set fire mode HUD
    }
}
