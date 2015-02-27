//////////////////////////////////////////////////////////////////////////////
// bl_DeathZone.cs
//
// -Put this script in an Object that itself contains one Collider component in trigger mode.
//  You can use this as a limit zones, where the player can not enter or stay.
//                          Lovatto Studio
//////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class bl_DeathZone : bl_PhotonHelper {
    /// <summary>
    /// Time maximum that may be prohibited in the area before dying
    /// </summary>
    public int TimeToDeat = 5;
    /// <summary>
    /// message that will appear in the UI when this within the zone
    /// </summary>
    public string CustomMessage = "you're in a zone prohibited \n returns to the playing area or die at \n";
    private bool mOn = false;
    public GameObject KillZoneUI = null;

    /// <summary>
    /// 
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        if (this.transform.collider != null)
        {
            transform.collider.isTrigger = true;
        }
        else
        {
            Debug.LogError("This Go " + gameObject.name + " need have a collider");
            Destroy(this);
        }
        if (KillZoneUI == null)
        {
            KillZoneUI = bl_GameManager.KillZone;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mCol"></param>
  void OnTriggerEnter(Collider mCol)
    {
        if (mCol.transform.tag == bl_PlayerSettings.LocalTag)//when is player local enter
        {
            bl_PlayerDamageManager pdm = mCol.transform.root.GetComponent<bl_PlayerDamageManager>();// get the component damage

            if (pdm != null && pdm.health > 0 && !mOn)
            {
                InvokeRepeating("regressive", 1, 1);
                if (KillZoneUI != null)
                {
                    KillZoneUI.SetActive(true);
                    Text mText = KillZoneUI.GetComponentInChildren<Text>();
                    mText.text = CustomMessage + "<color=red><size=25>" + TimeToDeat.ToString("00") + "</size>s</color>";
                }
                mOn = true;
            }

        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="mCol"></param>
    void OnTriggerExit(Collider mCol)
    {
        if (mCol.transform.tag == bl_PlayerSettings.LocalTag)// if player exit of zone then cancel countdown
        {
                CancelInvoke("regressive");
                TimeToDeat = 5; // restart time
                if (KillZoneUI != null)
                {
                    KillZoneUI.SetActive(false);
                }
                mOn = false;                
        }
    }
    /// <summary>
    /// Start CountDown when player is on Trigger
    /// </summary>
    void regressive()
    {
        TimeToDeat--;
        if (KillZoneUI != null)
        {
            Text mText = KillZoneUI.GetComponentInChildren<Text>();
            mText.text = CustomMessage + "<color=red><size=25>"+TimeToDeat.ToString("00")+"</size>s</color>";
        }
        if (TimeToDeat <= 0)
        {
            FindPlayerRoot(bl_GameManager.m_view).GetComponent<bl_PlayerDamageManager>().Suicide();
            CancelInvoke("regressive");
            TimeToDeat = 5;
            if (KillZoneUI != null)
            {
                KillZoneUI.SetActive(false);
            }
            mOn = false;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "DeathZone.psd", true);
    }
}