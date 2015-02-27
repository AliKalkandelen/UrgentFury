////////////////////////////////////////////////////////////////////////////////
//////////////////// bl_InstanceEventKit.cs                                  ///
////////////////////Use this to instantiate a prefabs (Kit) called.          ///
////////////////////////////////Briner Games////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class bl_InstanceEventKit : MonoBehaviour
{
    /// <summary>
    /// key to instantiate MedKit
    /// </summary>
    public KeyCode MedKitKey = KeyCode.H;
    /// <summary>
    /// key to instantiate AmmoKit
    /// </summary>
    public KeyCode AmmoKey = KeyCode.J;
    /// <summary>
    /// Medkit Prefabs for instantiate
    /// </summary>
    public GameObject Kit;
    /// <summary>
    /// Reference poncision where the kit will be instantiated
    /// </summary>
    public Transform InstancePoint;
    public AudioClip SpawnSound;
    /// <summary>
    /// number of kits available to instantiate.
    /// </summary>
    public int MedkitAmount = 3;
    public int AmmoKitAmount = 3;
    /// <summary>
    /// force when it is instantiated prefabs
    /// </summary>
    public float ForceImpulse = 500;
    [Space(5)]
    public GUISkin Skin;
    public Sprite MedkitIcon;
    public Sprite AmmoIcon;
    public string MedKitMsn = "Medkit";
    public string AmmoKitMsn = "AmmoKit";
    //private
    private Image m_KitIcon;
    private Text m_KitAmountText;
    private PlayerClass m_class = PlayerClass.Assault;

    void Start()
    {
        m_KitIcon = GameObject.Find("KitIcon").GetComponent<Image>();
        m_KitAmountText = GameObject.Find("KitsAmountText").GetComponent<Text>();
        m_class = bl_RoomMenu.m_playerclass;
    }
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(MedKitKey) && Kit != null && MedkitAmount > 0 && m_class == PlayerClass.Engineer)
        {
            MedkitAmount--;
            GameObject kit = Instantiate(Kit, InstancePoint.position, Quaternion.identity) as GameObject;
            kit.GetComponent<bl_CallEventKit>().m_type = bl_CallEventKit.KitType.Medit;
            kit.GetComponent<bl_CallEventKit>().m_text = MedKitMsn;
            kit.rigidbody.AddForce(transform.forward * ForceImpulse);
            if(SpawnSound){
                AudioSource.PlayClipAtPoint(SpawnSound, this.transform.position, 1.0f);
            }
            
        }
        if (Input.GetKeyDown(MedKitKey) && Kit != null && MedkitAmount > 0 && m_class == PlayerClass.Support)
        {
            MedkitAmount--;
            GameObject kit = Instantiate(Kit, InstancePoint.position, Quaternion.identity) as GameObject;
            kit.GetComponent<bl_CallEventKit>().m_type = bl_CallEventKit.KitType.Medit;
            kit.GetComponent<bl_CallEventKit>().m_text = MedKitMsn;
            kit.rigidbody.AddForce(transform.forward * ForceImpulse);
            if (SpawnSound)
            {
                AudioSource.PlayClipAtPoint(SpawnSound, this.transform.position, 1.0f);
            }
        }

        if (Input.GetKeyDown(AmmoKey) && Kit != null && AmmoKitAmount > 0 && m_class == PlayerClass.Assault)
        {
            AmmoKitAmount--;
            GameObject kit = Instantiate(Kit, InstancePoint.position, Quaternion.identity) as GameObject;
            kit.GetComponent<bl_CallEventKit>().m_type = bl_CallEventKit.KitType.Ammo;
            kit.GetComponent<bl_CallEventKit>().m_text = AmmoKitMsn;
            kit.rigidbody.AddForce(transform.forward * ForceImpulse);
            if (SpawnSound)
            {
                AudioSource.PlayClipAtPoint(SpawnSound, this.transform.position, 1.0f);
            }
        }
        if (Input.GetKeyDown(AmmoKey) && Kit != null && AmmoKitAmount > 0 && m_class == PlayerClass.Recon)
        {
            AmmoKitAmount--;
            GameObject kit = Instantiate(Kit, InstancePoint.position, Quaternion.identity) as GameObject;
            kit.GetComponent<bl_CallEventKit>().m_type = bl_CallEventKit.KitType.Ammo;
            kit.GetComponent<bl_CallEventKit>().m_text = AmmoKitMsn;
            kit.rigidbody.AddForce(transform.forward * ForceImpulse);
            if (SpawnSound)
            {
                AudioSource.PlayClipAtPoint(SpawnSound, this.transform.position, 1.0f);
            }
        }

        if (m_class == PlayerClass.Engineer || m_class == PlayerClass.Support)
        {
            if (m_KitIcon != null)
            {
                m_KitIcon.sprite = MedkitIcon;
            }
            if (m_KitAmountText != null)
            {
                m_KitAmountText.text = "x" + MedkitAmount;
            }
        }
        //Only if one of two class show 
        if (m_class == PlayerClass.Assault || m_class == PlayerClass.Recon)
        {
            if (m_KitIcon != null)
            {
                m_KitIcon.sprite = AmmoIcon;
            }
            if (m_KitAmountText != null)
            {
                m_KitAmountText.text = "x" + AmmoKitAmount;
            }
        }
	}

    //OnGUI Version
   /* void OnGUI()
    {
        if (MedkitIcon == null)
            return;
        if (AmmoIcon == null)
            return;
        GUI.skin = Skin;
        //Only if one of two class show 
        if (bl_RoomMenu.m_playerclass == PlayerClass.Engineer || bl_RoomMenu.m_playerclass == PlayerClass.Support)
        {
            GUI.DrawTexture(new Rect(Screen.width - 53, Screen.height - 110, 50, 50), MedkitIcon);
            GUI.Label(new Rect(Screen.width - 77, Screen.height - 85, 50, 50), "x<size=25>" + MedkitAmount + "</size>");
        }
        //Only if one of two class show 
        if (bl_RoomMenu.m_playerclass == PlayerClass.Assault || bl_RoomMenu.m_playerclass == PlayerClass.Recon)
        {
            GUI.DrawTexture(new Rect(Screen.width - 53, Screen.height - 110, 50, 50), AmmoIcon);
            GUI.Label(new Rect(Screen.width - 77, Screen.height - 85, 50, 50), "x<size=25>" + AmmoKitAmount + "</size>");
        }


        
    }*/
}
