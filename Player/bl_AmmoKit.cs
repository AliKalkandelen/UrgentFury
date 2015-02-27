/////////////////////////////////////////////////////////////////////////////////
//////////////////////////////bl_AmmoKit.cs//////////////////////////////////////
//////////////////Use this to create new internal events of AmmoKit Pick Up///////
/////////////////////////////////////////////////////////////////////////////////
////////////////////////////////Briner Games/////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

public class bl_AmmoKit : MonoBehaviour {
    /// <summary>
    /// Tag of our Player
    /// </summary>
    public string PlayerTag = "Player";
    /// <summary>
    /// add this amount clip to player
    /// </summary>
    public int m_amount = 3;
    public AudioClip PickSound;
    //private
    private bl_ItemManager m_manager;
    private bool Ready = false;


    void Awake()
    {
        if (GameObject.FindWithTag("ItemManager").GetComponent<bl_ItemManager>() != null)
        {
            m_manager = GameObject.FindWithTag("ItemManager").GetComponent<bl_ItemManager>();
            gameObject.name = "Kit" + bl_ItemManager.CurrentCount;
            bl_ItemManager.CurrentCount++;
        }
        else
        {
            Debug.LogError("need to have a ItemManager in the scena");
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider m_other)
    {
        if (m_other.transform.tag == PlayerTag)
        {
            if (!Ready)
            {
                Ready = true;
                bl_EventHandler.OnAmmo(m_amount);
                if (PickSound)
                {
                    AudioSource.PlayClipAtPoint(PickSound, transform.position, 1.0f);
                }
            }
            m_manager.DestroyGO(this.gameObject.name);
        }
    }
}
