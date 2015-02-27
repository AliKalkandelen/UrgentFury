/////////////////////////////////////////////////////////////////////////////////
//////////////////////////////bl_AirInstantiator.cs//////////////////////////////
//////////////the instantiator kits, place the top of the scene//////////////////
/////////////////////////////////////////////////////////////////////////////////
////////////////////////////////Briner Games/////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(AudioSource))]
public class bl_AirInstantiator : Photon.MonoBehaviour {
    /// <summary>
    /// Kit Instance Effect
    /// </summary>
    public GameObject m_Instance;
    /// <summary>
    /// Speed Kit flying effect
    /// </summary>
    public float m_SpeedIntance = 100;
    /// <summary>
    /// when activated, record this in the event
    /// </summary>
    void OnEnable()
    {
        bl_EventHandler.OnKitAir += this.SendNewPoint;
    }
    /// <summary>
    /// when disabled, quit this in the event
    /// </summary>
    void OnDisable()
    {
        bl_EventHandler.OnKitAir -= this.SendNewPoint;
    }
    /// <summary>
    /// This is called by an internal event
    /// </summary>
    /// <param name="m_position"></param>
    /// <param name="t_type"></param>
    public void SendNewPoint(Vector3 m_position,int t_type)
    {
        photonView.RPC("SyncKitCall", PhotonTargets.All, m_position,t_type);
    }

    [RPC]
    void SyncKitCall(Vector3 m_position,int t_type)
    {
        GameObject newInstance = Instantiate(m_Instance, this.transform.position, Quaternion.identity) as GameObject;
        newInstance.GetComponent<bl_AirInstance>().SepUp(m_position, m_SpeedIntance,t_type);
        audio.Play();
    }
}
