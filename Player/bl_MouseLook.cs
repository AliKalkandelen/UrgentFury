//////////////////////////////////////////////////////////////////////////////
// bl_MouseLook.cs
// Performs Basic Mouse Look Functionalities 
/////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

public class bl_MouseLook : MonoBehaviour {

    private bl_RoomMenu menu;
    public bool isPlayer = true;
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    public float offsetY = 0F;

    float rotationX = 0F;
    private GameObject cmra = null;

    public float rotationY = 0F;
    //private
    private bl_Gun m_Gun;
    public float sX;
    public float sY;
    Quaternion originalRotation;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        // Make the rigid body not change rotation
        if (rigidbody) rigidbody.freezeRotation = true;

        menu = GameObject.Find("_GameManager").GetComponent<bl_RoomMenu>();
        cmra = GameObject.FindWithTag("MainCamera");
        // Make the rigid body not change rotation
        if (rigidbody)
            rigidbody.freezeRotation = true;
        originalRotation = transform.localRotation;

        if (PlayerPrefs.HasKey("sensitive"))
        {
            sensitivityX = PlayerPrefs.GetFloat("sensitive");
            sensitivityY = PlayerPrefs.GetFloat("sensitive");
        }
        else
        {
            sensitivityX = 6;
            sensitivityY = 6;
        }
        sX = sensitivityX;
        sY = sensitivityY;
        if(isPlayer)
        m_Gun = this.transform.root.GetComponentInChildren<bl_GunManager>().CurrentGun;
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (cmra == null)
        {
            GetCurrentCamera();
            return;
        }
        if (!Screen.lockCursor)
        {
            sensitivityX = menu.m_sensitive;
            sensitivityY = menu.m_sensitive;
            sX = sensitivityX;
            sY = sensitivityY;
        }
        else
        {
            if (axes == RotationAxes.MouseXAndY)
            {
                // Read the mouse input axis
                rotationX += Input.GetAxis("Mouse X") * sensitivityX * 1000 * cmra.camera.fieldOfView;
                rotationY += (Input.GetAxis("Mouse Y") * sensitivityY * 1000 * cmra.camera.fieldOfView + offsetY);

                rotationX = bl_UtilityHelper.ClampAngle(rotationX, minimumX, maximumX);
                rotationY = bl_UtilityHelper.ClampAngle(rotationY, minimumY, maximumY);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);

                transform.localRotation = originalRotation * xQuaternion * yQuaternion;

            }
            else if (axes == RotationAxes.MouseX)
            {
                rotationX += Input.GetAxis("Mouse X") * sensitivityX /5;
                rotationX = bl_UtilityHelper.ClampAngle(rotationX, minimumX, maximumX);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                transform.localRotation = originalRotation * xQuaternion;
            }
            else
            {
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY/5 + offsetY;
                rotationY = bl_UtilityHelper.ClampAngle(rotationY, minimumY, maximumY);

                Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
                transform.localRotation = originalRotation * yQuaternion;
            }
            offsetY = 0F;
        }
        if (m_Gun && isPlayer)
        {
            if (m_Gun.isAmed)
            {
                sensitivityX = sX / 2;
                sensitivityY = sY / 2;
            }
            else
            {
                sensitivityX = sX;
                sensitivityY = sY;
            }
        }
    }

    /// <summary>
    /// if we Dont have the camera, then seek one.
    /// </summary>
    void GetCurrentCamera()
    {
        if (Camera.main != null)
        {
            cmra = Camera.main.gameObject;
        }
        else if (Camera.current != null)
        {
            cmra = Camera.current.gameObject;
        }
        else
        {
            cmra = this.gameObject;
        }
    }
}