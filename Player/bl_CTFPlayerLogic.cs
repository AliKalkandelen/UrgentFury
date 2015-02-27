using UnityEngine;
using System.Collections;

public class bl_CTFPlayerLogic : Photon.MonoBehaviour {

    public Team m_PlayerTeam = Team.All;
    public Transform FlagPosition;
    public bool isLocal;
    //private
    private bl_PlayerSettings m_settings;

    void Start()
    {
        m_settings = this.transform.GetComponent<bl_PlayerSettings>();
        m_PlayerTeam = m_settings.m_Team;
        isLocal = photonView.isMine;
    }
}
