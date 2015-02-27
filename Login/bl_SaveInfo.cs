﻿using UnityEngine;
using System.Collections;
using System.Text;
using System.Security;

public class bl_SaveInfo : MonoBehaviour {

    public string SaveInfoPHP_Url = "";
    public string SecretKey = "";
    [Space(5)]
    public string m_UserName = "";
    public int m_Kills = 0;
    public int m_Deaths = 0;
    public int Score = 0;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
       
        DontDestroyOnLoad(gameObject);
    }
    /// <summary>
    /// 
    /// </summary>
    void OnEnable()
    {
        bl_LoginManager.OnLogin += this.GetInfo;
    }
    /// <summary>
    /// 
    /// </summary>
    void OnDisable()
    {
        bl_LoginManager.OnLogin -= this.GetInfo;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="n"></param>
    /// <param name="k"></param>
    /// <param name="d"></param>
    /// <param name="s"></param>
    void GetInfo(string n, int k, int d, int s)
    {
        m_UserName = n;
        m_Kills = k;
        m_Deaths = d;
        Score = s;
    }

    /// <summary>
    /// If we connect then is avaible for update
    /// get curent player propiertis info for send
    /// </summary>
    public void SaveInfo()
    {
        if (!PhotonNetwork.connected || PhotonNetwork.player == null)
        {
            Debug.LogWarning("Can not get the information from the Player");
            return;
        }

        int k = PhotonNetwork.player.GetKills();
        int d = PhotonNetwork.player.GetDeaths();
        int s = PhotonNetwork.player.GetPlayerScore();

        StartCoroutine(Save(k,d,s));

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="k"></param>
    /// <param name="d"></param>
    /// <param name="s"></param>
    /// <returns></returns>
    IEnumerator Save(int k,int d, int s)
    {
        //Used for security check for authorization to modify database
        string hash = Md5Sum(m_UserName + SecretKey).ToLower();

        //Get old Player Info and Update with new info
        //for update DB info
        int t_kills = m_Kills + k;
        int t_deaths = m_Deaths + d;
        int t_score = Score + s;
        //Assigns the data we want to save
        //Where -> Form.AddField("name" = matching name of value in SQL database
        WWWForm mForm = new WWWForm();
        mForm.AddField("name", m_UserName); // adds the player name to the form
        mForm.AddField("kills", t_kills); // adds the kill total to the form
        mForm.AddField("deaths", t_deaths); // adds the death Total to the form
        mForm.AddField("score",t_score); // adds the score Total to the form
        mForm.AddField("hash", hash); // adds the security hash for Authorization

        //Creates instance of WWW to runs the PHP script to save data to mySQL database
        WWW www = new WWW(SaveInfoPHP_Url, mForm);
        Debug.Log("Processing...");
        yield return www;

        if (www.error == null)
        {
            Debug.Log("Saved Info Successfully.");
            //Update Vals Locals
            m_Kills = t_kills;
            m_Deaths = t_deaths;
            Score = t_score;

        }
    }

    /// <summary>
    /// Md5s Security Features
    /// </summary>
    public string Md5Sum(string input)
    {
        System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hash = md5.ComputeHash(inputBytes);

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++) { sb.Append(hash[i].ToString("X2")); }
        return sb.ToString();
    }
}
