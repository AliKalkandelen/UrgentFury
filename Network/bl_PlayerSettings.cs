////////////////////////////////////////////////////////////////////////////////
// bl_PlayerSettings.cs
//
// This script configures the required settings for the local and remote player
//
//                        Lovatto Studio
////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bl_PlayerSettings : bl_PhotonHelper
{
    /// <summary>
    /// The tag of Player for default is "Player"
    /// </summary>
    public const string LocalTag = "Player";
    /// <summary>
    /// please if you have this tag in the tag list, add
    /// </summary>
    public string RemoteTag = "Remote";
    public Team m_Team = Team.All;
    [Header("when the player is our disable these scripts")]
    public List<MonoBehaviour> Local_DisabledScripts = new List<MonoBehaviour>();
    [Header("when the player is Not our disable these scripts")]
    public List<MonoBehaviour> Remote_DisabledScripts = new List<MonoBehaviour>();
    [Header("when the player is our disable these GO")]
    public List<GameObject> Local_DesactiveObjects = new List<GameObject>();
    [Header("when the player is Not our disable these GO")]
    public List<GameObject> Remote_DesactiveObjects = new List<GameObject>();
    [System.Serializable]
    public class Messages_
    {
        public MonoBehaviour m_script;
        public string m_method;
    }
    /// <summary>
    /// If you do not want to turn off the entire script, just some functions then follow this example
    /// </summary>
    [Header("when player is not our, send a message to script")]
    public List<Messages_> m_SendMessage = new List<Messages_>();
    [System.Serializable]
    public class HandsLocal_
    {
        public Material SlevesMat;
        public Material GlovesMat;
        [Space(5)]
        public Texture2D SlevesDelta;
        public Texture2D GlovesDelta;
        [Space(5)]
        public Texture2D SlevesRecon;
        public Texture2D GlovesRecon;
        [Space(5)]
        public bool useEffect = true;
        public Color HandsInitColor = new Color(1, 1, 1, 1);
        public Color mBettewColor = new Color(0.1f, 0.1f, 1, 1);
    }
    public HandsLocal_ m_hands;
    //private
    
    /// <summary>
    /// 
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (this.isMine)
        {
            LocalPlayer();
        }
        else
        {
            RemotePlayer();
        }
    }

    /// <summary>
    /// We call this function only if we are Remote player
    /// </summary>
    public void RemotePlayer()
    {
        foreach (MonoBehaviour script in Remote_DisabledScripts)
        {
            Destroy(script);
        }
        foreach (GameObject obj in Remote_DesactiveObjects)
        {
            obj.SetActive(false);
        }
        for (int i = 0; i < m_SendMessage.Count; i++)
        {
            m_SendMessage[i].m_script.SendMessageUpwards(m_SendMessage[i].m_method);
        }
        this.gameObject.tag = RemoteTag;

    }
    /// <summary>
    /// We call this function only if we are Local player
    /// </summary>
    public void LocalPlayer()
    {
        gameObject.name = PhotonNetwork.player.name;
        if (myTeam == Team.Delta.ToString())
        {
            m_Team = Team.Delta;
            if (m_hands.SlevesMat != null)
            {
                m_hands.SlevesMat.mainTexture = m_hands.SlevesDelta;
            }
            if (m_hands.GlovesMat != null)
            {
                m_hands.GlovesMat.mainTexture = m_hands.GlovesDelta;
            }
        }
        else if (myTeam == Team.Recon.ToString())
        {
            m_Team = Team.Recon;
            if (m_hands.SlevesMat != null)
            {
                m_hands.SlevesMat.mainTexture = m_hands.SlevesRecon;
            }
            if (m_hands.GlovesMat != null)
            {
                m_hands.GlovesMat.mainTexture = m_hands.GlovesRecon;
            }
        }
        else
        {
            m_Team = Team.All;
            if (m_hands.SlevesMat != null)
            {
                m_hands.SlevesMat.mainTexture = m_hands.SlevesRecon;
            }
            if (m_hands.GlovesMat != null)
            {
                m_hands.GlovesMat.mainTexture = m_hands.GlovesRecon;
            }
        }
        if (m_hands.GlovesMat != null && m_hands.GlovesMat.HasProperty("_Color")
            && m_hands.SlevesMat != null && m_hands.SlevesMat.HasProperty("_Color") && m_hands.useEffect)
        {
            StartCoroutine(StartEffect());
        }
        foreach (MonoBehaviour script in Local_DisabledScripts)
        {
            Destroy(script);
        }
        foreach (GameObject obj in Local_DesactiveObjects)
        {
            obj.SetActive(false);
        }
        this.gameObject.tag = LocalTag;
    }
    /// <summary>
    /// produce an effect of spawn
    /// with a loop 
    /// </summary>
    /// <returns></returns>
    IEnumerator StartEffect()
    {
        int loops = 8;// number of repeats
        for (int i = 0; i < loops; i++)
        {
            yield return new WaitForSeconds(0.25f);
            m_hands.GlovesMat.color = m_hands.mBettewColor;
            m_hands.SlevesMat.color = m_hands.mBettewColor;
            yield return new WaitForSeconds(0.25f);
            m_hands.GlovesMat.color = m_hands.HandsInitColor;
            m_hands.SlevesMat.color = m_hands.HandsInitColor;

        }
    }

}