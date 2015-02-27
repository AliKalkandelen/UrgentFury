/////////////////////////////////////////////////////////////////////////////////
//////////////////////////////bl_AmmoKit.cs//////////////////////////////////////
///////put one of these in each scene to handle Items////////////////////////////
/////////////////////////////////////////////////////////////////////////////////
////////////////////////////////Briner Games/////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bl_ItemManager : Photon.MonoBehaviour
{ 
    /// <summary>
    /// instantiated reference necessary for synchronization
    /// </summary>
    public static int CurrentCount = 0;
    /// <summary>
    /// list of objects waiting to turn off again
    /// </summary>
    public List<GameObject> m_Objects = new List<GameObject>();
    /// <summary>
    /// timeouts to reactivate the gameobject (assigne auto)
    /// </summary>
    public List<float> m_info = new List<float>();
    /// <summary>
    /// list of all components within this transform (assigne auto)
    /// </summary>
    public bl_MedicalKit[] m_allmedkit;
    /// <summary>
    /// time to revive, defaults Kits
    /// </summary>
    public float TimeToRespawn = 15;
    public AudioClip PickSound;
    //private
    private int each_id = 0;
    private List<bl_MedicalKit> store = new List<bl_MedicalKit>();

    void Awake()
    {
        m_allmedkit = transform.GetComponentsInChildren<bl_MedicalKit>();
        //automatically place the id
        if (m_allmedkit.Length > 0)
        {
            foreach (bl_MedicalKit medit in m_allmedkit)
            {
                medit.m_id = each_id;
                each_id++;
                store.Add(medit);
            }
        }
    }

    void Update()
    {
        if (m_info.Count <= 0)
            return;
        //time management to revive
            for (int i = 0; i < m_info.Count; i++)
            {
                m_info[i] -= Time.deltaTime;
                if (m_info[i] <= 0)
                {
                    EnableAgain(m_Objects[i]);
                    m_info.Remove(m_info[i]);
                    m_Objects.Remove(m_Objects[i]);
                }
            }
        
    }
    /// <summary>
    /// Call this to temporarily disable a Items
    /// </summary>
    /// <param name="t_id">Item to disable</param>
    public void DisableNew(int t_id)
    {
        photonView.RPC("DisableGO", PhotonTargets.AllBuffered, t_id);
    }
    /// <summary>
    /// Enabled again the current finished item
    /// </summary>
    /// <param name="t_obj"></param>
    void EnableAgain(GameObject t_obj)
    {
        t_obj.SetActive(true);
    }
    /// <summary>
    /// called this when need destroy a item
    /// </summary>
    /// <param name="t_name">Item Name</param>
    public void DestroyGO(string t_name){
        photonView.RPC("DestroyGOSync", PhotonTargets.All, t_name);
    }

    [RPC]
    void DestroyGOSync(string GOname)
    {
        Destroy(GameObject.Find(GOname).gameObject);
    }

    [RPC]
    void DisableGO(int m_id)
    {
        if (m_allmedkit.Length <= 0)
            return;

        foreach (bl_MedicalKit med in m_allmedkit)
        {
            if (med.m_id == m_id)
            {
                if (PickSound)
                {
                    AudioSource.PlayClipAtPoint(PickSound, med.transform.position, 1.0f);
                }
                m_Objects.Add(med.gameObject);
                m_info.Add(TimeToRespawn);
                med.gameObject.SetActive(false);
            }
        }
        
    }
}
