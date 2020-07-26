using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDControl : MonoBehaviour
{
    public UnityEngine.UI.Image Crosshair;  // crosshair image
    public Text Ammo;       // ammo
    public Text FireMode;   // fire mode

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void updateAmmo(int currMag, int currAmmo)   // update ammo
    {
        Ammo.text = "" + currMag + "/" + currAmmo;
    }

    public void changeFireMode(int mode)    // change fire mode
    {
        switch (mode)   // -1: single, 1: auto
        {
            case -1:
                FireMode.text = "SINGLE";
                break;
            case 1:
                FireMode.text = "AUTO";
                break;
            default:
                break;
        }
    }

    public void showGunInfo(bool isEquip)   // show gun info if equiped (ammo, fire mode)
    {
        Ammo.enabled = isEquip;
        FireMode.enabled = isEquip;
    }

    public void showCrosshair(bool isAimed) // show crosshair if not aiming
    {
        Crosshair.enabled = !isAimed;
    }

}
