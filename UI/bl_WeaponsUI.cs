using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;//import UI refernce directionary

public class bl_WeaponsUI : MonoBehaviour {

    public GUISkin Skin;
    public Color AmmoNormal = new Color(1,1,1,1);
    public Color AmmoLow = new Color(0.8f,0,0,1);
    public int isLowBullet = 5;
    public int isLowBulletSniper = 1;
    public float BulletFactorReconpen = 3.87f;
    public string PickAmmoMsn = "Pick Up Ammo";
    public string PickMedKitMsn = "Pick Up MedKit";
    public float TimeToShowNotify = 15;
    //Private 
    //UI Unity 4.6
    private Text AmmoTextUI = null;
    private bl_Gun CurrentGun;
    private bl_GunManager GManager;
    private float BulletLeft;
    private int Clips;
    private Color m_color = new Color(0,0,0,0);
    private List<string> Notifications = new List<string>();
    private List<float> Noti_Time = new List<float>();

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        GManager = this.GetComponent<bl_GunManager>();
        AmmoTextUI = GameObject.Find("Ammo_UI").GetComponentInChildren<Text>();
    }
    /// <summary>
    /// 
    /// </summary>
    void OnEnable()
    {
        bl_EventHandler.OnKitAmmo += this.OnPickUpAmmo;
        bl_EventHandler.OnPickUp += this.OnPicUpMedKit;
    }
    /// <summary>
    /// 
    /// </summary>
    void OnDisable()
    {
        bl_EventHandler.OnKitAmmo -= this.OnPickUpAmmo;
        bl_EventHandler.OnPickUp -= this.OnPicUpMedKit;
    }
    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        CurrentGun = GManager.CurrentGun;
        if (BulletLeft != CurrentGun.bulletsLeft)
        {
            BulletLeft = Mathf.Lerp(BulletLeft, CurrentGun.bulletsLeft, Time.deltaTime * BulletFactorReconpen);
        }
       // BulletLeft = CurrentGun.bulletsLeft;
        Clips = CurrentGun.numberOfClips;

        if (BulletLeft <= isLowBullet && CurrentGun.typeOfGun == bl_Gun.weaponType.Machinegun || BulletLeft <= isLowBullet && CurrentGun.typeOfGun == bl_Gun.weaponType.Burst || BulletLeft <= isLowBullet && CurrentGun.typeOfGun == bl_Gun.weaponType.Pistol)
        {
            m_color = Color.Lerp(m_color, AmmoLow, (Seno(6.0f, 0.1f, 0.0f) * 5) + 0.5f);
        }
        else if (CurrentGun.typeOfGun == bl_Gun.weaponType.Shotgun && BulletLeft <= isLowBulletSniper || CurrentGun.typeOfGun == bl_Gun.weaponType.Sniper && BulletLeft <= isLowBulletSniper)
        {
            m_color = Color.Lerp(m_color, AmmoLow, (Seno(6.0f, 0.1f, 0.0f) * 5) + 0.5f);
        }
        else
        {
            m_color = Color.Lerp(m_color, AmmoNormal, (Seno(6.0f, 0.1f, 0.0f) * 5) + 0.5f);
        }

        if (AmmoTextUI != null)
        {
            AmmoTextUI.text = bl_UtilityHelper.GetThreefoldChar(BulletLeft) + "/<size=10>" + Clips + "</size>";
            AmmoTextUI.color = m_color;
        }

        if (Noti_Time.Count > 0)
        {
            for (int i = 0; i < Noti_Time.Count; i++)
            {
                Noti_Time[i] -= Time.deltaTime;
                if (Noti_Time[i] <= 0.0f)
                {
                    Notifications.RemoveAt(i);
                    Noti_Time.RemoveAt(i);
                }
            }
        }
    }

/// <summary>
/// 
/// </summary>
    
    void OnGUI()
    {
        GUI.skin = Skin;
        /*
        GUI.color = m_color;
        bl_UtilityHelper.ShadowLabel(new Rect(Screen.width, Screen.height - 50, -200, 50), "<size=50>" + bl_UtilityHelper.GetThreefoldChar(BulletLeft) + "</size>" + " / " + Clips, BoxStyle);
        GUI.color = Color.white;*/
        NotifyUI();
    }
    /// <summary>
    /// 
    /// </summary>
    void NotifyUI()
    {
        GUILayout.BeginArea(new Rect(5, Screen.height / 2 - 150, 200, 300));
        if (Notifications.Count > 0)
        {
            for (int i = 0; i < Notifications.Count; i++)
            {
               GUI.color = new Color(1,1,1,Noti_Time[i]);
                GUILayout.Box(Notifications[i]);
                GUI.color = Color.white;
            }
            
        }
        GUILayout.EndArea();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="m_clips"></param>
    void OnPickUpAmmo(int m_clips)
    {
        Notifications.Add(PickAmmoMsn + " <size=20><color=orange>"+m_clips+"</color></size>");
        Noti_Time.Add(TimeToShowNotify);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="t_amount"></param>
    void OnPicUpMedKit(float t_amount)
    {
        Notifications.Add(PickMedKitMsn + " <size=20><color=orange>" + t_amount + "</color></size>");
        Noti_Time.Add(TimeToShowNotify);
    }
    /// <summary>
    /// 
    /// </summary>
    public GUIStyle BoxStyle
    {
        get
        {
            return Skin.customStyles[2];
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static float Seno(float rate, float amp, float offset = 0.0f)
    {
        return (Mathf.Cos((Time.time + offset) * rate) * amp);
    }
}