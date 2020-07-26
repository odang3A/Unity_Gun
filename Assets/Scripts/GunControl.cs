using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControl : MonoBehaviour
{
    // gun type
    string gunName;
    Vector3 hipLoc;
    Vector3 aimLoc;
    float damage;
    float fireRate;
    bool isSelectiveFire;
    int capacity;

    bool isEquip = false;
    bool isFireable = false;
    int fireMode;   // -1: single, 1: auto
    int currAmmo;
    float nextTimeToFire = 0f;
    Transform FPSCamera; // FPSCamera
    HUDControl hudControl;
    BoxCollider[] gunColliders;
    Rigidbody gunRb;

    // effects
    public ParticleSystem muzzleFlash;
    public GameObject hitImpact;


    void Start()
    {
        gunColliders = GetComponents<BoxCollider>();
        gunRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isEquip)
        {
            control();

            // 실행순서 주의할것 // tlakrf

            if (Input.GetKeyDown("x"))
            {   // weapon down
                weaponDown();
            }

            if(Input.GetKeyDown("r") || currAmmo <= 0)
            {
                //StartCoroutine(reload());
                weaponDown();
                Debug.Log("tlqkf");
            }

        }
    }

    void control()
    {
        if (Input.GetKeyDown("b") && isSelectiveFire)
        {   // change fire mode
            fireMode *= -1;
        }

        switch (fireMode)   // check fire mode
        {   // -1: single, 1: auto
            case -1:
                isFireable = Input.GetMouseButtonDown(0);
                break;
            case 1:
                isFireable = Input.GetMouseButton(0) && Time.time >= nextTimeToFire;
                break;
            default:
                break;
        }


        if (isFireable && currAmmo > 0)
        {   // fire
            fire();
        }


        // aim
        transform.localPosition = (Input.GetMouseButton(1) ? aimLoc : hipLoc);
        FPSCamera.GetComponent<Camera>().fieldOfView = (transform.localPosition == aimLoc ? 30f : 60f);
        hudControl.hideCrosshair(transform.localPosition == aimLoc);
    }

    void fire()
    {
        currAmmo--;
        nextTimeToFire = Time.time + 1f / fireRate;

        muzzleFlash.Play(); // play muzzle flash effects

        RaycastHit hit;
        if (Physics.Raycast(FPSCamera.position, FPSCamera.forward, out hit))
        {   // if hit something
            Debug.Log("We hit " + hit.collider.name + ": " + hit.point);

            if (hit.rigidbody)
            {   // knockback
                //hit.rigidbody.AddExplosionForce(damage, hit.point, 5, 0.2f, ForceMode.Impulse);
                hit.rigidbody.AddForce(transform.forward * damage, ForceMode.Impulse);
            }

            // play hit Impact effects
            GameObject impactObj = Instantiate(hitImpact, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactObj, 1);
        }
    }

    IEnumerator reload()
    {
        yield return new WaitForSeconds(1);
    }

    void weaponDown()
    {   // weapon down
        isEquip = false;
        transform.parent = transform.root.parent;
        foreach (BoxCollider gunCollider in gunColliders)
        {
            gunCollider.enabled = true;
        }
        gunRb.useGravity = true;
    }

    public void SetGunType(string name, Vector3 hipLoc, Vector3 aimLoc, float damage, float fireRate, bool isSelectiveFire, int defaultFireMode, int capacity)
    {
        gunName = name;
        this.hipLoc = hipLoc;
        this.aimLoc = aimLoc;
        this.damage = damage;
        this.fireRate = fireRate;
        this.isSelectiveFire = isSelectiveFire;
        fireMode = defaultFireMode;
        this.capacity = capacity;
        currAmmo = this.capacity + 1;
    }


    public void Selected(Transform selector)
    {
        isEquip = true;
        FPSCamera = selector;
        hudControl = FPSCamera.parent.Find("HUD").GetComponent<HUDControl>();

        foreach (BoxCollider gunCollider in gunColliders)
        {
            gunCollider.enabled = false;
        }
        
        transform.SetParent(FPSCamera);
        transform.localPosition = hipLoc;
        transform.localRotation = Quaternion.identity;
        gunRb.angularVelocity = Vector3.zero;
        gunRb.useGravity = false;
    }
}
