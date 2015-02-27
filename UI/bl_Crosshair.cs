using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class bl_Crosshair : MonoBehaviour
{

    public Image crosshair_bottom;
    public Image crosshair_left;
    public Image crosshair_right;
    public Image crosshair_top;
    [Range(0.1f, 20f)]
    public float movementScale = 1f;
    public float FadeSpeed = 8f;
    public GameObject CrossRoot;
    [Space(5)]
    public bl_GameManager GManager;
    //private
    private float alpha;
    private float right_x;
    private float top_y;
    private float left_x;
    private float bottom_y;
    private Color mColor = new Color(1, 1, 1, 1);

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        if (this.crosshair_top != null)
        {
            this.top_y = this.crosshair_top.rectTransform.anchoredPosition.y;
        }
        if (this.crosshair_bottom != null)
        {
            this.bottom_y = this.crosshair_bottom.rectTransform.anchoredPosition.y;
        }
        if (this.crosshair_left != null)
        {
            this.left_x = this.crosshair_left.rectTransform.anchoredPosition.x;
        }
        if (this.crosshair_right != null)
        {
            this.right_x = this.crosshair_right.rectTransform.anchoredPosition.x;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void Active()
    {
        if (bl_GameManager.isAlive == false || mCurrentGun.typeOfGun == bl_Gun.weaponType.Launcher)
        {
            CrossRoot.SetActive(false);
        }
        else
        {
            CrossRoot.SetActive(true);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (GManager == null)
            return;
        if (m_GunManager == null)
            return;
        if (mCurrentGun == null)
            return;
        if (mCharacterController == null)
            return;


        Active();
        FadeUpdate();

        Vector2 mPosition;
        float vel = mCharacterController.velocity.magnitude;
        float mSpread = (mCurrentGun.spread * 0.5f) * this.movementScale;
        if (this.crosshair_top != null)
        {
            mPosition = this.crosshair_top.rectTransform.anchoredPosition;
            mPosition.y = (this.top_y + mSpread) + vel;
            this.crosshair_top.rectTransform.anchoredPosition = mPosition;
        }
        if (this.crosshair_bottom != null)
        {
            mPosition = this.crosshair_bottom.rectTransform.anchoredPosition;
            mPosition.y = (this.bottom_y - mSpread) - vel;
            this.crosshair_bottom.rectTransform.anchoredPosition = mPosition;
        }
        if (this.crosshair_left != null)
        {
            mPosition = this.crosshair_left.rectTransform.anchoredPosition;
            mPosition.x = (this.left_x - mSpread) - vel;
            this.crosshair_left.rectTransform.anchoredPosition = mPosition;
        }
        if (this.crosshair_right != null)
        {
            mPosition = this.crosshair_right.rectTransform.anchoredPosition;
            mPosition.x = (this.right_x + mSpread) + vel;
            this.crosshair_right.rectTransform.anchoredPosition = mPosition;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void FadeUpdate()
    {
        if (mCurrentGun.isAmed)
        {
            if (alpha != 0)
            {
                alpha = Mathf.Lerp(alpha, 0f, Time.deltaTime * FadeSpeed);
            }
        }
        else
        {
            if (alpha != 1f)
            {
                alpha = Mathf.Lerp(alpha, 1f, Time.deltaTime * FadeSpeed);
            }
        }

        mColor.a = alpha;
        crosshair_bottom.color = mColor;
        crosshair_left.color = mColor;
        crosshair_right.color = mColor;
        crosshair_top.color = mColor;
    }

    /// <summary>
    /// 
    /// </summary>
    private bl_GunManager weaponManager;
    private bl_GunManager m_GunManager
    {
        get
        {
            bl_GunManager gm = null;
            if (weaponManager == null)
            {
                if (GManager.OurPlayer != null)
                {
                    gm = GManager.OurPlayer.GetComponentInChildren<bl_GunManager>();
                }
                if (gm != null)
                {
                    weaponManager = gm;
                }
            }
            else
            {
                gm = weaponManager;
            }
            return gm;
        }
    }
    /// <summary>
    /// 
    /// </summary>

    private bl_Gun mCurrentGun
    {
        get
        {
            bl_Gun gun;

            gun = m_GunManager.GetCurrentWeapon();
            return gun;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private CharacterController mCharacterController
    {
        get
        {
            return GManager.OurPlayer.transform.root.GetComponent<CharacterController>();
        }
    }

}