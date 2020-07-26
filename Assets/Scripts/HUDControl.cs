using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDControl : MonoBehaviour
{
    public UnityEngine.UI.Image Crosshair;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void hideCrosshair(bool isAimed)
    {
        Crosshair.enabled = !isAimed;
    }

    public void changeFireMode(int mode)
    {
        switch (mode)
        {   // -1: single, 1: auto
            case -1:
                break;
            case 1:
                break;
            default:
                break;
        }
    }
}
