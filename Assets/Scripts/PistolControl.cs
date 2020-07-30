using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolControl : MonoBehaviour
{
    GunControl gunControl;

    // gun stats
    string gunName = "pistol";
    Vector3 pistolHipLoc = new Vector3(0.4f, -0.3f, 0.6f);
    Vector3 pistolAimLoc = new Vector3(0f, -0.155f, 0.6f);
    float pistolDamage = 10f;
    float pistolFireRate = 0; // single
    float pistolReloadTime = 1.5f;
    float pistolAccuracy = 0.75f;
    bool isSelectiveFire = false;
    int pistolCapacity = 12;

    // Start is called before the first frame update
    void Start()
    {
        gunControl = transform.GetComponent<GunControl>();
        gunControl.SetGunType(gunName, pistolHipLoc, pistolAimLoc, pistolDamage, pistolFireRate, pistolReloadTime, pistolAccuracy, isSelectiveFire, -1, pistolCapacity);
    }
    
}
