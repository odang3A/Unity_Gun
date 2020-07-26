using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AKControl : MonoBehaviour
{
    GunControl gunControl;

    // gun stats
    string gunName = "AK";
    Vector3 AKHipLoc = new Vector3(0.5f, -0.32f, 1f);
    Vector3 AKAimLoc = new Vector3(0f, -0.266f, 1f);
    float AKDamage = 10f;
    float AKFireRate = 10; // pre sec
    bool isSelectiveFire = true;
    int AKCapacity = 30;

    // Start is called before the first frame update
    void Start()
    {
        gunControl = transform.GetComponent<GunControl>();
        gunControl.SetGunType(gunName, AKHipLoc, AKAimLoc, AKDamage, AKFireRate, isSelectiveFire, 1, AKCapacity);
    }
    
}
