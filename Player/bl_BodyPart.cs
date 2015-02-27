///////////////////////////////////////////////////////////////////////////////////////
// bl_BodyPart.cs
//
// This script receives the information of the damage done by another player
// place it on a gameobject containing a collider in the hierarchy of the remote player
// use "bl_BodyPartManager.cs" to automatically configure                            
//                                 Lovatto Studio
///////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class bl_BodyPart : MonoBehaviour {

    /// <summary>
    /// so far as the damage is multiplied in this part
    /// </summary>
    public float multiplier = 1f;
    public bl_PlayerDamageManager HealtScript;
    /// <summary>
    /// Its this the heat? if yes them is heatshot :)
    /// </summary>
    public bool TakeHeatShot = false;

    /// <summary>
    /// Use this for recive damage local and sync for all other
    /// </summary>
    /// <param name="t_damage"></param>
    /// <param name="t_from"></param>
    /// <param name="t_weapon"></param>
    /// <param name="t_direction"></param>
    public void GetDamage(float t_damage, string t_from, string t_weapon,Vector3 t_direction,int weapon_ID = 0)
    {
        float m_TotalDamage = t_damage + multiplier;

        bl_OnDamageInfo e = new bl_OnDamageInfo();
        e.mDamage = m_TotalDamage;
        e.mDirection = t_direction;
        e.mWeapon = t_weapon;
        e.mHeatShot = TakeHeatShot;
        e.mWeaponID = weapon_ID;
        e.mFrom = t_from;

        if (HealtScript != null)
        {
            HealtScript.GetDamage(e);
        }
    }
}
