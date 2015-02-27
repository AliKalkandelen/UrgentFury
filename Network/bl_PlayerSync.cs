////////////////////////////////////////////////////////////////////////////////
//////////////////// bl_PlayerSync.cs///////////////////////////////////////////
////////////////////use this for the sincronizer pocision , rotation, states,/// 
///////////////////etc ...   via photon/////////////////////////////////////////
////////////////////////////////Briner Games////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PhotonView))]
public class bl_PlayerSync : bl_PhotonHelper
{
    /// <summary>
    /// the player's team is not ours
    /// </summary>
    [HideInInspector]
    public string RemoteTeam;
    /// <summary>
    /// the current state of the current weapon
    /// </summary>
    public string WeaponState;
    /// <summary>
    /// the object to which the player looked
    /// </summary>
    public Transform HeatTarget;
    /// <summary>
    /// smooth interpolation amount
    /// </summary>
    public float SmoothingDelay = 8f;
    /// <summary>
    /// list all remote weapons
    /// </summary>
    public List<bl_NetworkGun> NetworkGuns = new List<bl_NetworkGun>();

    [SerializeField]
    PhotonTransformViewPositionModel m_PositionModel = new PhotonTransformViewPositionModel();

    [SerializeField]
    PhotonTransformViewRotationModel m_RotationModel = new PhotonTransformViewRotationModel();

    [SerializeField]
    PhotonTransformViewScaleModel m_ScaleModel = new PhotonTransformViewScaleModel();

    PhotonTransformViewPositionControl m_PositionControl;
    PhotonTransformViewRotationControl m_RotationControl;
    PhotonTransformViewScaleControl m_ScaleControl;

    bool m_ReceivedNetworkUpdate = false;
    [Space(5)]
   //Script Needed
    [Header("Necessary script")]
    public bl_GunManager GManager;
    public bl_PlayerAnimations m_PlayerAnimation;
    public bl_UpperAnimations m_Upper;

//private
    private bl_PlayerMovement Controller;
    private GameObject CurrenGun;
    private bl_PlayerSettings Settings;
    private bl_PlayerDamageManager PDM;
    private bl_DrawName DrawName;

#pragma warning disable 0414
    [SerializeField]
    bool ObservedComponentsFoldoutOpen = true;
#pragma warning disable 0414

    protected override void Awake()
    {
        base.Awake();

        if (!PhotonNetwork.connected)
            Destroy(this);

        //FirstUpdate = false;
        if (!this.isMine)
        {
            if (HeatTarget.gameObject.activeSelf == false)
            {
                HeatTarget.gameObject.SetActive(true);
            }
        }

        m_PositionControl = new PhotonTransformViewPositionControl(m_PositionModel);
        m_RotationControl = new PhotonTransformViewRotationControl(m_RotationModel);
        m_ScaleControl = new PhotonTransformViewScaleControl(m_ScaleModel);
        Controller = this.GetComponent<bl_PlayerMovement>();
        Settings = this.GetComponent<bl_PlayerSettings>();
        PDM = this.GetComponent<bl_PlayerDamageManager>();
        DrawName = this.GetComponent<bl_DrawName>();
    }
    /// <summary>
    /// serialization method of photon
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        m_PositionControl.OnPhotonSerializeView(transform.localPosition, stream, info);
        m_RotationControl.OnPhotonSerializeView(transform.localRotation, stream, info);
        m_ScaleControl.OnPhotonSerializeView(transform.localScale, stream, info);
        if (isMine == false && m_PositionModel.DrawErrorGizmo == true)
        {
            DoDrawEstimatedPositionError();
        }
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(gameObject.name);
            stream.SendNext(HeatTarget.position);
            stream.SendNext(HeatTarget.rotation);
            stream.SendNext(Controller.state);
            stream.SendNext(Controller.grounded);
            stream.SendNext(GManager.CurrentGun.GunID);
            stream.SendNext(Settings.m_Team.ToString());
            stream.SendNext(WeaponState);
        }
        else
        {
            //Network player, receive data
            RemotePlayerName = (string)stream.ReceiveNext();
            HeadPos = (Vector3)stream.ReceiveNext();
            HeadRot = (Quaternion)stream.ReceiveNext();
            m_state = (int)stream.ReceiveNext();
            m_grounded = (bool)stream.ReceiveNext();
            CurNetGun = (int)stream.ReceiveNext();
            RemoteTeam = (string)stream.ReceiveNext();
            m_Upper.m_state = (string)stream.ReceiveNext();
            m_ReceivedNetworkUpdate = true;
        }
    }

    private Vector3 HeadPos = Vector3.zero;// Head Look to
    private Quaternion HeadRot = Quaternion.identity;
    private int m_state;
    private bool m_grounded;
    private string RemotePlayerName = string.Empty;
    private int CurNetGun;
    
    public void Update()
    {
        ///if the player is not ours, then
         if( photonView == null || isMine == true || isConnected == false )
        {
            return;
        }

         UpdatePosition();
         UpdateRotation();
         UpdateScale();
         GetTeamRemote();

            this.HeatTarget.position = Vector3.Lerp(this.HeatTarget.position, HeadPos, Time.deltaTime * this.SmoothingDelay);
            this.HeatTarget.rotation = HeadRot;
            m_PlayerAnimation.state = m_state;//send the state of player local for remote animation
            m_PlayerAnimation.grounded = m_grounded;

            if (this.gameObject.name != RemotePlayerName)
            {
                gameObject.name = RemotePlayerName;
            }
            if (GetGameMode == GameMode.TDM ||
                GetGameMode == GameMode.CTF)
            {
                //Determine if remote player is teamMate or enemy
                if (RemoteTeam == (string)PhotonNetwork.player.customProperties[PropiertiesKeys.TeamKey])
                {
                    TeamMate();
                }
                else
                {
                    Enemy();
                }
            }
            else if (GetGameMode == GameMode.FFA)
            {
                Enemy();
            }
            //Get the current gun ID local and sync with remote
            foreach (bl_NetworkGun guns in NetworkGuns)
            {
                if (guns.WeaponID == CurNetGun)
                {
                    guns.gameObject.SetActive(true);
                    CurrenGun = guns.gameObject;
                }
                else
                {
                    guns.gameObject.SetActive(false);
                }
            }    
    }
    /// <summary>
    /// use this function to set all details for enemy
    /// </summary>
    void Enemy()
    {
        PDM.DamageEnabled = true;
        DrawName.enabled = false;
    }
    /// <summary>
    /// use this function to set all details for teammate
    /// </summary>
    void TeamMate()
    {
        PDM.DamageEnabled = false;
        DrawName.enabled = true;
    }
    /// <summary>
    /// public method to send the RPC shot synchronization
    /// </summary>
    /// <param name="m_type"></param>
    /// <param name="t_spread"></param>
    public void IsFire(string m_type,float t_spread)
    {
        photonView.RPC("FireSync", PhotonTargets.Others, new object[] {m_type,t_spread});
    }
    /// <summary>
    /// Synchronise the shot with the current remote weapon
    /// send the information necessary so that fire
    /// impact in the same direction as the local
    /// </summary>
    /// <param name="m_type"></param>
    /// <param name="m_spread"></param>
    [RPC]
    void FireSync(string m_type,float m_spread)
    {
        if (CurrenGun)
        {
            if (m_type == bl_Gun.weaponType.Machinegun.ToString())
            {
                CurrenGun.GetComponent<bl_NetworkGun>().Fire(m_spread);
            }
            else if (m_type == bl_Gun.weaponType.Shotgun.ToString())
            {
                CurrenGun.GetComponent<bl_NetworkGun>().Fire(m_spread);//if you need add your custom fire shotgun in networkgun
            }
            else if (m_type == bl_Gun.weaponType.Sniper.ToString())
            {
                CurrenGun.GetComponent<bl_NetworkGun>().Fire(m_spread);//if you need add your custom fire sniper in networkgun
            }
            else if (m_type == bl_Gun.weaponType.Burst.ToString())
            {
                CurrenGun.GetComponent<bl_NetworkGun>().Fire(m_spread);//if you need add your custom fire burst in networkgun
            }
            else if (m_type == bl_Gun.weaponType.Launcher.ToString())
            {               
               CurrenGun.GetComponent<bl_NetworkGun>().GrenadeFire(m_spread);//if you need add your custom fire launcher in networkgun
            }

        }
    }

    void GetTeamRemote()
    {
        if (RemoteTeam == Team.Recon.ToString())
        {
            Settings.m_Team = Team.Recon;
        }
        else if (RemoteTeam == Team.Delta.ToString())
        {
            Settings.m_Team = Team.Delta;
        }
        else
        {
            Settings.m_Team = Team.All;
        }
    }
    void UpdatePosition()
    {
        if (m_PositionModel.SynchronizeEnabled == false || m_ReceivedNetworkUpdate == false)
        {
            return;
        }

        transform.localPosition = m_PositionControl.UpdatePosition(transform.localPosition);
    }

    void UpdateRotation()
    {
        if (m_RotationModel.SynchronizeEnabled == false || m_ReceivedNetworkUpdate == false)
        {
            return;
        }

        transform.localRotation = m_RotationControl.GetRotation(transform.localRotation);
    }

    void UpdateScale()
    {
        if (m_ScaleModel.SynchronizeEnabled == false || m_ReceivedNetworkUpdate == false)
        {
            return;
        }

        transform.localScale = m_ScaleControl.GetScale(transform.localScale);
    }
    void DoDrawEstimatedPositionError()
    {
        Vector3 targetPosition = m_PositionControl.GetNetworkPosition();

        Debug.DrawLine(targetPosition, transform.position, Color.red, 2f);
        Debug.DrawLine(transform.position, transform.position + Vector3.up, Color.green, 2f);
        Debug.DrawLine(targetPosition, targetPosition + Vector3.up, Color.red, 2f);
    }
    /// <summary>
    /// These values are synchronized to the remote objects if the interpolation mode
    /// or the extrapolation mode SynchronizeValues is used. Your movement script should pass on
    /// the current speed (in units/second) and turning speed (in angles/second) so the remote
    /// object can use them to predict the objects movement.
    /// </summary>
    /// <param name="speed">The current movement vector of the object in units/second.</param>
    /// <param name="turnSpeed">The current turn speed of the object in angles/second.</param>
    public void SetSynchronizedValues(Vector3 speed, float turnSpeed)
    {
        m_PositionControl.SetSynchronizedValues(speed, turnSpeed);
    }
}

