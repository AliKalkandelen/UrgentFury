using UnityEngine;
using System.Collections;

public abstract class bl_FlagBase : MonoBehaviour {


    public abstract bool CanBePickedUpBy(bl_CTFPlayerLogic logic);

    public abstract void OnPickup(bl_CTFPlayerLogic logic);

    PhotonView m_PhotonView;

    protected PhotonView PhotonView
    {
        get
        {
            if (m_PhotonView == null)
            {
                m_PhotonView = PhotonView.Get(this);
            }

            return m_PhotonView;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            bl_CTFPlayerLogic logic = collider.gameObject.GetComponent<bl_CTFPlayerLogic>();
            if (CanBePickedUpBy(logic) == true)
            {
                PickupObject(logic);
            }
        }
    }

    void PickupObject(bl_CTFPlayerLogic logic)
    {
            PhotonView.RPC("OnPickup", PhotonTargets.AllBuffered, PhotonNetwork.player,logic.photonView.viewID );         
    }

    [RPC]
    protected void OnPickup(PhotonPlayer m_actor,int m_view)
    {
        PhotonView view = PhotonView.Find(m_view);
        if (view != null)
        {
            bl_CTFPlayerLogic logic = view.GetComponent<bl_CTFPlayerLogic>();
            if (CanBePickedUpBy(logic) == true)
            {
                OnPickup(logic);
                if (PhotonNetwork.player == m_actor)
                {
                    bool t_send = false;//Prevent call two or more events
                    if (!t_send)
                    {
                        t_send = true;
                        Team oponentTeam;
                        if ((string)PhotonNetwork.player.customProperties[PropiertiesKeys.TeamKey] == Team.Delta.ToString())
                        {
                            oponentTeam = Team.Recon;
                        }
                        else
                        {
                            oponentTeam = Team.Delta;
                        }
                        bl_EventHandler.KillEvent(PhotonNetwork.player.name, "", "Obtained at the " + oponentTeam.ToString() + " flag", (string)PhotonNetwork.player.customProperties[PropiertiesKeys.TeamKey], 777, 15);

                    }
                }
            }
        }
                  
        }
    
}