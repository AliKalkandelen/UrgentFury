/////////////////////////////////////////////////////////////////////////////////
//////////////////// bl_EventHandler.cs/////////////////////////////////////////
////////////////////Use this to create new internal events///////////////////////
//this helps to improve the communication of the script through delegated events/
////////////////////////////////Briner Games/////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

public class bl_EventHandler
{
    //Call all script when Fall Events
    public delegate void FallEvent(float m_amount);
    public static FallEvent OnFall;
    //Call all script when Kill Events
    public delegate void KillFeedEvent(string kl, string kd, string hw, string m_team, int gid, float t);
    public static KillFeedEvent OnKillFeed;
    //Item EventPick Up
    public delegate void ItemsPickUp(float Amount);
    public static ItemsPickUp OnPickUp;
    //Call new Kit Air 
    public delegate void KitAir(Vector3 m_position,int type);
    public static KitAir OnKitAir;
    //Pick Up Ammo
    public delegate void AmmoKit(int Clips);
    public static AmmoKit OnKitAmmo;
    //On Kill Event
    public delegate void NewKill(string mtype,float m_score);
    public static NewKill OnKill;
    //On Round End
    public delegate void RoundEnd();
    public static RoundEnd OnRoundEnd;
    //small impact
    public delegate void SmallImpact();
    public static SmallImpact OnSmallImpact;
    //Receive Damage
    public delegate void GetDamage(bl_OnDamageInfo e);
    public static GetDamage OnDamage;
    /// <summary>
    /// Callet event when recive Fall Impact
    /// </summary>
    /// <param name="m_amount"></param>
    public static void EventFall(float m_amount)
    {
        if (OnFall != null)
            OnFall(m_amount);
    }
    /// <summary>
    /// Event Callet when recive a new keelfed message
    /// </summary>
    /// <param name="kl">Killer</param>
    /// <param name="kd">Killed</param>
    /// <param name="hw">How Kill</param>
    /// <param name="t_team">Player Killer Team</param>
    /// <param name="gid"> Gun ID</param>
    /// <param name="t">Time to Show</param>
    public static void KillEvent(string kl, string kd, string hw, string t_team, int gid, float t)
    {
        if (OnKillFeed != null)
            OnKillFeed(kl, kd, hw, t_team, gid, t);
    }
    /// <summary>
    /// Called event when pick up a medkit
    /// </summary>
    /// <param name="t_amount"> new healt amount</param>
    public static void PickUpEvent(float t_amount)
    {
        if (OnPickUp != null)
            OnPickUp(t_amount);
    }
    /// <summary>
    /// Called event when call a new kit 
    /// </summary>
    /// <param name="t_position">position where kit appear</param>
    /// <param name="type"></param>
    public static void KitAirEvent(Vector3 t_position,int type){
        if (OnKitAir != null)
            OnKitAir(t_position,type);
    }
    /// <summary>
    /// Called Event when pick up ammo
    /// </summary>
    /// <param name="clips">number of clips</param>
    public static void OnAmmo(int clips)
    {
        if (OnKitAmmo != null)
            OnKitAmmo(clips);
    }
    /// <summary>
    /// Called this when killed a new player
    /// </summary>
    /// <param name="t_amount"></param>
    public static void OnKillEvent(string t_type,float t_amount)
    {
        if (OnKill != null)
            OnKill(t_type,t_amount);
    }
    /// <summary>
    /// Call This when room is finish a round
    /// </summary>
    public static void OnRoundEndEvent()
    {
        if (OnRoundEnd != null)
            OnRoundEnd();
    }

    public static void OnSmallImpactEvent()
    {
        if (OnSmallImpact != null)
            OnSmallImpact();
    }

    public static void eDamage(bl_OnDamageInfo e)
    {
        if (OnDamage != null)
            OnDamage(e);
    }
}
