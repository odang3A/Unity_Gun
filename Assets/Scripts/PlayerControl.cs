using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    Rigidbody rb;

    public Camera FPSCamera;
    
    public float moveForce = 50;
    public float dragForce = 0.1f;
    public float jumpForce = 5;
    public int extraJumps = 1;

    // moves
    float inW, inA, inS, inD = 0;
    int jumpCnt = 0;
    Vector3 moveDir;
    Vector3 dragDir;
    List<Collision> stepping = new List<Collision>();

    // interaction
    float range = 2.5f;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        // move input
        inW = (Input.GetKey("w") ? 1 : 0);
        inA = (Input.GetKey("a") ? 1 : 0);
        inS = (Input.GetKey("s") ? 1 : 0);
        inD = (Input.GetKey("d") ? 1 : 0);

        // jump input
        if (Input.GetKeyDown(KeyCode.Space) && jumpCnt <= extraJumps)
        {
            if(stepping.Count == 0)
            { 
                jumpCnt++;
            }
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        // sight rotation
        sightRotation();

        // interact
        interact();
    }

    void FixedUpdate()
    {
        // move
        moveDir = (inW * transform.forward + inA * -transform.right + inS * -transform.forward + inD * transform.right);
        moveDir /= (moveDir.magnitude == 0 ? 1 : get_XZ_Magnitude(moveDir) );
        rb.AddForce(moveDir * moveForce, ForceMode.Force);

        // drag
        dragDir = -rb.velocity;
        dragDir.y = 0;
        rb.AddForce(dragDir * dragForce, ForceMode.Impulse);
    }


    void interact()
    {
        if (Input.GetKeyDown("f"))
        {
            RaycastHit target;
            if (Physics.Raycast(FPSCamera.transform.position, FPSCamera.transform.forward, out target, range))
            {
                Debug.Log(target.transform.name);

                if (target.transform.tag == "Gun")
                {
                    if (FPSCamera.transform.childCount > 0)
                    {
                        FPSCamera.transform.GetChild(0).GetComponent<GunControl>().unequip();
                    }

                    GunControl gunControl = target.transform.GetComponent<GunControl>();
                    gunControl.Selected(FPSCamera.transform);
                }
            }
        }
    }

    void OnCollisionEnter(Collision target)
    {
        List<ContactPoint> contacts = new List<ContactPoint>();
        bool canStepOn = false;
        target.GetContacts(contacts);
        foreach(ContactPoint point in contacts)
        {
            if(point.normal.y >= 0.8f)
            {
                jumpCnt = 0;
                canStepOn = true;
            }
        }

        if (canStepOn)
        {
            stepping.Add(target);
        }

        //Debug.Log("Enter " + target.gameObject.name);
    }

    void OnCollisionExit(Collision target)
    {
        if (stepping.Contains(target))
        {
            jumpCnt++;
            stepping.Remove(target);

            //Debug.Log("hh: " + jumpCnt);
        }

        //Debug.Log("Exit: " + target.gameObject.name);
    }

    float get_XZ_Magnitude(Vector3 v)
    {   // get xz magnitude
        v.y = 0;
        return v.magnitude;
    }

    void sightRotation()
    {   // sight rotation
        float mouseY, temp_X;

        transform.Rotate(0, Input.GetAxis("Mouse X"), 0, Space.World);

        mouseY = -Input.GetAxis("Mouse Y");
        temp_X = FPSCamera.transform.eulerAngles.x + mouseY;
        if (temp_X > 280.0f || temp_X < 80)
        {
            FPSCamera.transform.Rotate(mouseY, 0, 0, Space.Self);
            
        }
    }
}
