//////////////////////////////////////////////////////////////////
//bl_Mono.cs
//
//This a simple base class
//to us serve as an extension of Photon.Monobehaviour default
//                   Lovatto Studio
//////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;

public class bl_PhotonHelper : Photon.PunBehaviour {

    protected string myTeam = string.Empty;
    protected GameMode mGameMode = GameMode.FFA;
    /// <summary>
    /// 
    /// </summary>
    protected virtual void Awake()
    {
        if (!PhotonNetwork.connected)
            return;

        myTeam = (string)PhotonNetwork.player.customProperties[PropiertiesKeys.TeamKey];
        if ((string)PhotonNetwork.room.customProperties[PropiertiesKeys.GameModeKey] == GameMode.FFA.ToString())
        {
            mGameMode = GameMode.FFA;
        }
        else if ((string)PhotonNetwork.room.customProperties[PropiertiesKeys.GameModeKey] == GameMode.TDM.ToString())
        {
            mGameMode = GameMode.TDM;
        }
        else if ((string)PhotonNetwork.room.customProperties[PropiertiesKeys.GameModeKey] == GameMode.CTF.ToString())
        {
            mGameMode = GameMode.CTF;
        }
        else
        {
            mGameMode = GameMode.FFA;
        }
    }
    /// <summary>
    /// Find a player gameobject by the viewID 
    /// </summary>
    /// <param name="view"></param>
    /// <returns></returns>
    public GameObject FindPlayerRoot(int view)
    {
        PhotonView m_view = PhotonView.Find(view);

        if (m_view != null)
        {
            return m_view.gameObject;
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    ///  get a photonView by the viewID
    /// </summary>
    /// <param name="view"></param>
    /// <returns></returns>
    public PhotonView FindPlayerView(int view)
    {
        PhotonView m_view = PhotonView.Find(view);

        if (m_view != null)
        {
            return m_view;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public Transform Root
    {
        get
        {
            return transform.root;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public Transform Parent
    {
        get
        {
            return transform.parent;
        }
    }

    /// <summary>
    /// True if the PhotonView is "mine" and can be controlled by this client.
    /// </summary>
    /// <remarks>
    /// PUN has an ownership concept that defines who can control and destroy each PhotonView.
    /// True in case the owner matches the local PhotonPlayer.
    /// True if this is a scene photonview on the Master client.
    /// </remarks>
    public bool isMine
    {
        get
        {
            return (this.photonView.ownerId == PhotonNetwork.player.ID) || (!this.photonView.isOwnerActive && PhotonNetwork.isMasterClient);
        }
    }
    /// <summary>
    /// Get Photon.connect
    /// </summary>
    public bool isConnected
    {
        get
        {
            return PhotonNetwork.connected;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public GameObject FindPhotonPlayer(PhotonPlayer p)
    {
        GameObject player = GameObject.Find(p.name);
        if (player == null)
        {
            return null;
        }
          return player;
    }
    /// <summary>
    /// Get the team of players
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public string GetTeam(PhotonPlayer p)
    {
        if (p == null || !isConnected)
            return null;

            string t = (string)p.customProperties[PropiertiesKeys.TeamKey];
            return t;
    }
    /// <summary>
    /// Get current gamemode
    /// </summary>
    public GameMode GetGameMode
    {
        get
        {
            if (!isConnected || PhotonNetwork.room == null)
                return GameMode.TDM;

            return mGameMode;
        }
    }

    public string LocalName
    {
        get
        {
            if (PhotonNetwork.player != null && isConnected)
            {
                string n = PhotonNetwork.player.name;
                return n;
            }
            else
            {
                return "None";
            }
        }
    }
}