/////////////////////////////////////////////////////////////////////////////////
///////////////////////////bl_GunManager.cs//////////////////////////////////////
/////////////Use this to manage all weapons Player///////////////////////////////
/////////////////////////////////////////////////////////////////////////////////
////////////////////////////////Briner Games/////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bl_GunManager : MonoBehaviour {
    /// <summary>
    /// all the Guns of game
    /// </summary>
    public List<bl_Gun> AllGuns = new List<bl_Gun>();
    /// <summary>
    /// weapons that the player take equipped
    /// </summary>
    public List<bl_Gun> PlayerEquip = new List<bl_Gun>();

    /// <summary>
    /// ID the weapon to take to start
    /// </summary>
    public int m_Current = 0;
    /// <summary>
    /// weapon used by the player currently
    /// </summary>
    public bl_Gun CurrentGun;
    /// <summary>
    /// time it takes to switch weapons
    /// </summary>
    public float SwichTime = 1;
    /// <summary>
    /// Can Swich Now?
    /// </summary>
    public bool CanSwich;
    [Space(5)]
    public Animator m_HeatAnimator;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        //when player instance select player class select in bl_RoomMenu
        switch (bl_RoomMenu.m_playerclass)
        {
            case PlayerClass.Assault:
                PlayerEquip[0] = AllGuns[m_AssaultClass.primary];
                PlayerEquip[1] = AllGuns[m_AssaultClass.secondary];
                PlayerEquip[2] = AllGuns[m_AssaultClass.Special];
                break;
            case PlayerClass.Recon:
                PlayerEquip[0] = AllGuns[m_ReconClass.primary];
                PlayerEquip[1] = AllGuns[m_ReconClass.secondary];
                PlayerEquip[2] = AllGuns[m_ReconClass.Special];
                break;
            case PlayerClass.Engineer:
                PlayerEquip[0] = AllGuns[m_EngineerClass.primary];
                PlayerEquip[1] = AllGuns[m_EngineerClass.secondary];
                PlayerEquip[2] = AllGuns[m_EngineerClass.Special];
                break;
            case PlayerClass.Support:
                PlayerEquip[0] = AllGuns[m_SupportClass.primary];
                PlayerEquip[1] = AllGuns[m_SupportClass.secondary];
                PlayerEquip[2] = AllGuns[m_SupportClass.Special];
                break;
        }
        //Desactive all weapons in children and take the firts
        foreach (bl_Gun guns in AllGuns)
        {
            guns.gameObject.SetActive(false);
        }
        TakeWeapon(PlayerEquip[m_Current].gameObject);
    }
    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && CanSwich && m_Current != 0)
        {
           
            StartCoroutine(ChangeGun(PlayerEquip[m_Current].gameObject,PlayerEquip[0].gameObject));
             m_Current = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && CanSwich && m_Current != 1)
        {
            
            StartCoroutine(ChangeGun((PlayerEquip[m_Current].gameObject),PlayerEquip[1].gameObject));
            m_Current = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && CanSwich && m_Current != 2)
        {
            StartCoroutine(ChangeGun((PlayerEquip[m_Current].gameObject), PlayerEquip[2].gameObject));
            m_Current = 2;
        }
        CurrentGun = PlayerEquip[m_Current];
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="t_weapon"></param>
    void TakeWeapon(GameObject t_weapon)
    {
        t_weapon.SetActive(true);
        CanSwich = true;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bl_Gun GetCurrentWeapon()
    {
        if (CurrentGun == null)
        {
            return PlayerEquip[m_Current];
        }
        else
        {
            return CurrentGun;
        }
    }
    /// <summary>
    /// Corrutine to Change of Gun
    /// </summary>
    /// <param name="t_current"></param>
    /// <param name="t_next"></param>
    /// <returns></returns>
   public IEnumerator ChangeGun(GameObject t_current,GameObject t_next)
    {
        CanSwich = false;
        if (m_HeatAnimator != null)
        {
            m_HeatAnimator.SetBool("Swicht", true);
        }
        t_current.GetComponent<bl_Gun>().DisableWeapon();
        yield return new WaitForSeconds(SwichTime);
        foreach (bl_Gun guns in AllGuns)
        {
            if (guns.gameObject.activeSelf == true)
            {
                guns.gameObject.SetActive(false);
            }
        }
        TakeWeapon(t_next);
        if (m_HeatAnimator != null)
        {
            m_HeatAnimator.SetBool("Swicht", false);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="m_state"></param>
   public void heatReloadAnim(int m_state)
   {
       if (m_HeatAnimator == null)
           return;

       switch (m_state)
       {
           case 0:
               m_HeatAnimator.SetInteger("Reload", 0);
               break;
           case 1:
               m_HeatAnimator.SetInteger("Reload", 1);
               break;
           case 2:
               m_HeatAnimator.SetInteger("Reload", 2);
               break;
       }
   }

    
   [System.Serializable]
   public class AssaultClass
   {
       //ID = the number of Gun in the list AllGuns
       /// <summary>
       /// the ID of the first gun Equipped
       /// </summary>
       public int primary = 0;
       /// <summary>
       /// the ID of the secondary Gun Equipped
       /// </summary>
       public int secondary = 1;
       /// <summary>
       /// the ID the a special weapon
       /// </summary>
       public int Special = 2;
   }
   public AssaultClass m_AssaultClass;

   [System.Serializable]
   public class EngineerClass
   {
       //ID = the number of Gun in the list AllGuns
       /// <summary>
       /// the ID of the first gun Equipped
       /// </summary>
       public int primary = 0;
       /// <summary>
       /// the ID of the secondary Gun Equipped
       /// </summary>
       public int secondary = 1;
       /// <summary>
       /// the ID the a special weapon
       /// </summary>
       public int Special = 2;
   }
   public EngineerClass m_EngineerClass;
    //
   [System.Serializable]
   public class ReconClass
   {
       //ID = the number of Gun in the list AllGuns
       /// <summary>
       /// the ID of the first gun Equipped
       /// </summary>
       public int primary = 0;
       /// <summary>
       /// the ID of the secondary Gun Equipped
       /// </summary>
       public int secondary = 1;
       /// <summary>
       /// the ID the a special weapon
       /// </summary>
       public int Special = 2;
   }
   public ReconClass m_ReconClass;
    //
   [System.Serializable]
   public class SupportClass
   {
       //ID = the number of Gun in the list AllGuns
       /// <summary>
       /// the ID of the first gun Equipped
       /// </summary>
       public int primary = 0;
       /// <summary>
       /// the ID of the secondary Gun Equipped
       /// </summary>
       public int secondary = 1;
       /// <summary>
       /// the ID the a special weapon
       /// </summary>
       public int Special = 2;
   }
   public SupportClass m_SupportClass;
}
