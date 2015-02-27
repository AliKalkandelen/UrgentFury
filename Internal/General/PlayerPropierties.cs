//////////////////////////////////////////////////////////////////////////////
// PlayerProperties.cs
//
// this facilitates access to properties 
// more authoritatively for each photon player, ej: PhotonNetwork.player.GetKills();
//
//                       LovattoStudio
//////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

static class PlayerProperties
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    /// <param name="ScoreToAdd"></param>
    public static void PostScore(this PhotonPlayer player, int ScoreToAdd = 0)
    {
        int current = player.GetPlayerScore();
        current = current + ScoreToAdd;

        Hashtable score = new Hashtable();  // using PUN's implementation of Hashtable
        score[PropiertiesKeys.ScoreKey] = current;

        player.SetCustomProperties(score);  // this locally sets the score and will sync it in-game asap.
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static int GetPlayerScore(this PhotonPlayer player)
    {
        int s = 0;

        if (player.customProperties.ContainsKey(PropiertiesKeys.ScoreKey))
        {
            s = (int)player.customProperties[PropiertiesKeys.ScoreKey];
            return s;
        }

        return s;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static int GetKills(this PhotonPlayer p)
    {
        int k = 0;
        if (p.customProperties.ContainsKey(PropiertiesKeys.KillsKey))
        {
            k = (int)p.customProperties[PropiertiesKeys.KillsKey];
            return k;
        }
        return k;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public static int GetDeaths(this PhotonPlayer p)
    {
        int d = 0;
        if (p.customProperties.ContainsKey(PropiertiesKeys.DeathsKey))
        {
            d = (int)p.customProperties[PropiertiesKeys.DeathsKey];
            return d;
        }
        return d;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="p"></param>
    /// <param name="kills"></param>
    public static void PostKill(this PhotonPlayer p, int kills)
    {
        int current = p.GetKills();
        current = current + kills;

        Hashtable score = new Hashtable();  // using PUN's implementation of Hashtable
        score[PropiertiesKeys.KillsKey] = current;

        p.SetCustomProperties(score);  // this locally sets the score and will sync it in-game asap.
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="p"></param>
    /// <param name="deaths"></param>
    public static void PostDeaths(this PhotonPlayer p, int deaths)
    {
        int current = p.GetDeaths();
        current = current + deaths;

        Hashtable score = new Hashtable();  // using PUN's implementation of Hashtable
        score[PropiertiesKeys.DeathsKey] = current;

        p.SetCustomProperties(score);  // this locally sets the score and will sync it in-game asap.
    }
}