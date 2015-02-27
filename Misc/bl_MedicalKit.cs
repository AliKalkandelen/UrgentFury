/////////////////////////////////////////////////////////////////////////////////
//////////////////// bl_MedicalKit.cs     /////////////////////////////////////// 
////////////////////Use this to create new internal events of MedKit Pick Up  ///
/////////////////////////////////////////////////////////////////////////////////
////////////////////////////////Briner Games/////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

public class bl_MedicalKit : MonoBehaviour {
    /// <summary>
    /// Tag of our Player
    /// </summary>
    public string PlayerTag = "Player";
    /// <summary>
    /// Add this amount more to health
    /// </summary>
    public float m_amount = 25;
    /// <summary>
    /// Id od Current Kit (this asigne auto)
    /// </summary>
    public int m_id = 0;

    private bool Alredy = false;
    private int typekit = 0;
    private bl_ItemManager m_manager;

    void Awake()
    {
        if (this.transform.root.GetComponent<bl_ItemManager>() != null)//if this default kit 
        {
            m_manager = this.transform.root.GetComponent<bl_ItemManager>();
            typekit = 1;
        }else
        if (GameObject.FindWithTag("ItemManager") != null)//if this kit instance
        {
            this.transform.parent = GameObject.FindWithTag("ItemManager").transform;
            m_manager = GameObject.FindWithTag("ItemManager").GetComponent<bl_ItemManager>();
            typekit = 2;
            gameObject.name = "Kit" + bl_ItemManager.CurrentCount;
            bl_ItemManager.CurrentCount++;
        }
        else//if any destroy this
        {
            Debug.LogError("need to have a ItemManager in the scena");
            Destroy(this.gameObject);
        }
        
    }

    void OnTriggerEnter( Collider m_other)
    {
        if (m_other.transform.tag == PlayerTag)
        {
            bl_PlayerDamageManager pdm = m_other.transform.root.GetComponent<bl_PlayerDamageManager>();
            if (pdm.health < pdm.maxHealth)
            {
                if (typekit == 1)
                {
                    //Prevent sum more than one
                    if (!Alredy)
                    {
                        Alredy = true;
                        bl_EventHandler.PickUpEvent(m_amount);//Call new internal event
                    }
                    if (m_manager != null)
                    {
                        m_manager.DisableNew(m_id);
                    }
                } if (typekit == 2)
                {
                    //Prevent sum more than one
                    if (!Alredy)
                    {
                        Alredy = true;
                        bl_EventHandler.PickUpEvent(m_amount);//Call new internal event
                    }
                    if (m_manager != null)
                    {
                        m_manager.DestroyGO(this.transform.name);
                    }
                }
            }
        }
    }
}
