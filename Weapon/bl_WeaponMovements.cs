/////////////////////////////////////////////////////////////////////////////////
///////////////////////bl_WeaponMovements.cs/////////////////////////////////////
/////////////Use this to manage the movement of the gun when running/////////////
/////////////////////////////////////////////////////////////////////////////////
////////////////////////////////Briner Games/////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

public class bl_WeaponMovements : MonoBehaviour
{
    private bl_PlayerMovement controller;
    [Space(5)]
    [Header("Weapon On Run Postion")]
    [Tooltip("Weapon Position and Position On Run")]
    public Vector3 moveTo;
    [Tooltip("Weapon Rotation and Position On Run")]
    public Vector3 rotateTo;
    [Space(5)]
    [Header("Weapon On Run and Reload Postion")]
    [Tooltip("Weapon Position and Position On Run and Reload")]
    public Vector3 moveToReload;
    [Tooltip("Weapon Rotation and Position On Run and Reload")]
    public Vector3 rotateToReload;
    [Space(5)]
    public float swayIn = 2f;
    /// <summary>
    /// Time to return to position origin
    /// </summary>
    public float swayOut = 5f;
    /// <summary>
    /// Speed of Sway movement
    /// </summary>
    public float swaySpeed = 1f;
    //private
    private Transform myTransform; 
    private float vel;
    private Quaternion DefaultRot;
    private Vector3 DefaultPos;
    private bl_Gun Gun;

    public  void Awake()
    {
        this.myTransform = this.transform;
        DefaultRot = myTransform.localRotation;
        DefaultPos = myTransform.localPosition;
        controller = this.transform.root.GetComponent<bl_PlayerMovement>();
        Gun = transform.parent.GetComponent<bl_Gun>();
    }

    void Update()
    {
        this.vel = this.controller.velMagnitude;
        if (((this.vel > 4f) && this.controller.grounded) && this.controller.run && !Gun.isFiring && !Gun.isAmed)
        {
            if (Gun.isReloading)
            {
                Quaternion quaternion2 = Quaternion.Euler(this.rotateToReload);
                this.myTransform.localRotation = Quaternion.Slerp(this.myTransform.localRotation, quaternion2, Time.deltaTime * this.swayIn);
                this.myTransform.localPosition = Vector3.Lerp(this.myTransform.localPosition, this.moveToReload, Time.deltaTime * this.swayIn);
            }
            else
            {

                Quaternion quaternion2 = Quaternion.Euler(this.rotateTo);
                this.myTransform.localRotation = Quaternion.Slerp(this.myTransform.localRotation, quaternion2, Time.deltaTime * this.swayIn);
                this.myTransform.localPosition = Vector3.Lerp(this.myTransform.localPosition, this.moveTo, Time.deltaTime * this.swayIn);
            }
        }
        else
        {
            this.myTransform.localRotation = Quaternion.Slerp(this.myTransform.localRotation, DefaultRot, Time.deltaTime * this.swayOut);
            this.myTransform.localPosition = Vector3.Lerp(this.myTransform.localPosition, DefaultPos, Time.deltaTime * this.swayOut);
        }
    }
}

