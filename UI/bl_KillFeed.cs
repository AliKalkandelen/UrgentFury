using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class bl_KillFeed : Photon.MonoBehaviour {
    
    public GUISkin Skin; 
    /// <summary>
    /// Use Middle text or Weapon Icon
    /// </summary>
    public bool useTexture = true;
    /// <summary>
    /// maximum number of notifications that can be displayed
    /// </summary>
    public float MaxNotify = 5;
    public List<KillFeed> m_KillFeed = new List<KillFeed>();
    /// <summary>
    /// Weapon icons for middle text
    /// </summary>
    public List<Texture2D> GunIcons = new List<Texture2D>();
    /// <summary>
    /// Color of team delta
    /// </summary>
    public Color m_DeltaColor;
    /// <summary>
    /// Color of team recon
    /// </summary>
    public Color m_ReconColor;
    [System.Serializable]
    public class _UI
    {
        public Text ScoreLocal;
    }
    public _UI UI;
    //private
    private bl_SettingPropiertis setting;
    private List<string> KillNotify = new List<string>();
    private List<float> Notify_Time = new List<float>();

    void Awake()
    {
        setting = this.GetComponent<bl_SettingPropiertis>();
        if (PhotonNetwork.room != null)
        {
            OnJoined();
        }
    }

    void OnEnable()
    {
        bl_EventHandler.OnKillFeed += this.OnKillFeed;
        bl_EventHandler.OnKill += this.NewKill;
    }
    void OnDisable()
    {
        bl_EventHandler.OnKillFeed -= this.OnKillFeed;
        bl_EventHandler.OnKill -= this.NewKill;
    }

    void Update()
    {
        //Remove notifications corner
        for (int i = 0; i < m_KillFeed.Count; i++)
        {
            KillFeed t_KillFeed = m_KillFeed[i];
            t_KillFeed.m_Timer -= Time.deltaTime;
            if (t_KillFeed.m_Timer > 0)
            {
                m_KillFeed[i] = new KillFeed(t_KillFeed.m_Killer,t_KillFeed.m_Killed, t_KillFeed.m_HowKill,t_KillFeed.m_GunID, t_KillFeed.m_KillerColor, t_KillFeed.m_KilledColor,t_KillFeed.m_Timer);
            }
            else
            {
                m_KillFeed.RemoveAt(i);
            }
        }
        //Remove notifications locals
        for (int l = 0; l < Notify_Time.Count; l++)
        {
            Notify_Time[l] -= Time.deltaTime;
            if (Notify_Time[l] <= 0)
            {
                KillNotify.RemoveAt(l);
                Notify_Time.RemoveAt(l);
            }
        }
        if (UI.ScoreLocal != null)
        {
            UI.ScoreLocal.text = "<size=8>Score:</size> "+PhotonNetwork.player.customProperties[PropiertiesKeys.ScoreKey].ToString();
        }
    }

    void OnGUI()
    {
        GUI.skin = Skin;
        GUILayout.BeginArea(new Rect(Screen.width - Screen.width / 3.7f, 85, Screen.width / 3.7f, 400));
        //Show kill notificatin list
        foreach (KillFeed kf in m_KillFeed)
        {
            GUI.color = new Color(1, 1, 1, kf.m_Timer);
            GUILayout.BeginHorizontal(DegradadoStyle);
            GUILayout.FlexibleSpace();
            //Killer
            GUI.color = new Color(kf.m_KillerColor.r, kf.m_KillerColor.g, kf.m_KillerColor.b, kf.m_Timer);
            GUILayout.Label(kf.m_Killer);
            GUILayout.Space(5);
            //How Kill
            if (!useTexture)
            {
                GUI.color = new Color(1, 1, 1, kf.m_Timer);
                GUILayout.Label(kf.m_HowKill);
                GUILayout.Space(5);
            }
            else
            {
                if (kf.m_GunID != 777)// 777 is an improvised method to assume that we do not want to display the icon.
                {
                    GUI.color = new Color(1, 1, 1, kf.m_Timer);
                    GUILayout.Label(GunIcons[kf.m_GunID], GUILayout.Width(50), GUILayout.Height(30));
                    GUILayout.Space(5);
                }
                else
                {
                    GUI.color = new Color(1, 1, 1, kf.m_Timer);
                    GUILayout.Label(kf.m_HowKill);
                    GUILayout.Space(5);
                }
            }
            //Killed
            GUI.color = new Color(kf.m_KilledColor.r, kf.m_KilledColor.g, kf.m_KilledColor.b, kf.m_Timer);
            GUILayout.Label(kf.m_Killed);
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
            GUI.color = Color.white;
        }
        GUILayout.EndArea();
        LocalKillGUI();
    }
    /// <summary>
    /// Called this when a new kill event 
    /// </summary>
    /// <param name="t_Killer"></param>
    /// <param name="t_Killed"></param>
    /// <param name="t_HowKill"></param>
    /// <param name="t_team"></param>
    /// <param name="t_GunID"></param>
    /// <param name="t_Timer"></param>
   public void OnKillFeed(string t_Killer, string t_Killed, string t_HowKill, string t_team, int t_GunID, float t_Timer)
    {
        photonView.RPC("AddNewKillFeed", PhotonTargets.All, t_Killer, t_Killed, t_HowKill,t_team.ToString(), t_GunID, (int)t_Timer);
    }
    /// <summary>
    /// Player Joined? sync
    /// </summary>
    void OnJoined()
    {
        photonView.RPC("AddNewKillFeed", PhotonTargets.All, PhotonNetwork.player.name, Team.All.ToString(), "Joined", "", 777, 30);
    }

    [RPC]
    void AddNewKillFeed(string t_Killer,string t_Killed, string t_HowKill,string m_team, int t_GunID, int t_Timer)
    {
        Color KillerColor = new Color(1, 1, 1, 1);
        Color KilledColor = new Color(1, 1, 1, 1);

        if (setting.m_GameMode == GameMode.TDM || setting.m_GameMode == GameMode.CTF)
        {
            if (m_team == "Delta")
            {
                KillerColor = m_DeltaColor;
                KilledColor = m_ReconColor;
            }
            else if(m_team == "Recon")
            {
                KillerColor = m_ReconColor;
                KilledColor = m_DeltaColor;
            }else{
                KilledColor = Color.white;
                KillerColor = Color.white;
            }
        }
        m_KillFeed.Add(new KillFeed(t_Killer, t_Killed, t_HowKill, t_GunID, KillerColor, KilledColor, t_Timer));
       
        if (m_KillFeed.Count > MaxNotify)
        {
            m_KillFeed.RemoveAt(0);
        }
    }
    void LocalKillGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 300, 400));
        for (int i = 0; i < KillNotify.Count; i++)
        {
            GUI.color = new Color(1, 1, 1, Notify_Time[i]);
            GUILayout.Label(KillNotify[i]);
            GUI.color = Color.white;
        }
        GUILayout.EndArea();
    }
    /// <summary>
    /// Show a local ui when out killed other player
    /// </summary>
    /// <param name="m_type"></param>
    /// <param name="t_amount"></param>
    protected virtual void NewKill(string m_type, float t_amount)
    {
        KillNotify.Add(m_type+" <size=25><color=orange>"+t_amount+"</color></size>");
        Notify_Time.Add(7);
    }

    public GUIStyle DegradadoStyle
    {
        get
        {
            return Skin.customStyles[5];
        }
    }
}
