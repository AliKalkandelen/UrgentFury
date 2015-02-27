using UnityEngine;
using System.Collections;

public class bl_FlagInfo : bl_FlagBase {


    public static bl_FlagInfo RedFlag;

    public static bl_FlagInfo BlueFlag;

    //Flag GUI
    public Color IconColor;
    public Texture2D FlagIcon;
    public Transform IconTarget;
    public Vector2 IconSize = new Vector2(7, 7);

    public static bl_FlagInfo GetFlag(Team team)
    {
        if (team == Team.Delta)
        {
            return BlueFlag;
        }

        return RedFlag;
    }


    public static Team GetOtherTeam(Team team)
    {
        if (team == Team.Delta)
        {
            return Team.Delta;
        }

        return Team.Recon;
    }

    public Team Team;

    public float ReturnTime;

    float m_ReturnTimer;
    Vector3 m_HomePosition;
    bl_CTFPlayerLogic m_CarryingPlayer;

    void Awake()
    {
        m_HomePosition = transform.position;

        if (Team == Team.Recon)
        {
            BlueFlag = this;
        }
        else
        {
            RedFlag = this;
        }
    }

    void LateUpdate()
    {
        HandleFlagDrop();
        UpdatePosition();
        HandleFlagCapture();
        UpdateReturnTimer();
    }

    void UpdateReturnTimer()
    {
        if (m_ReturnTimer == -1f)
        {
            return;
        }

        m_ReturnTimer -= Time.deltaTime;

        if (m_ReturnTimer <= 0f)
        {
            m_ReturnTimer = -1f;
            ReturnFlag();
        }
    }

    void UpdatePosition()
    {
        if (m_CarryingPlayer == null)
        {
            return;
        }

        transform.position = m_CarryingPlayer.FlagPosition.position;
    }

    /// <summary>
    /// If the local player died, send the drop flag event to all players
    /// </summary>
    void HandleFlagDrop()
    {
        if (m_CarryingPlayer == null)
        {
            return;
        }
        bl_PlayerDamageManager playerhealth = m_CarryingPlayer.GetComponent<bl_PlayerDamageManager>();
        if (playerhealth.health <= 0)
        {
            DropFlag();
        }
    }

    void HandleFlagCapture()
    {
        if (m_CarryingPlayer == null)
        {
            return;
        }

        if (GetFlag(GetOtherTeam(Team)).IsHome() == true &&
            Vector3.Distance(transform.position, GetFlag(GetOtherTeam(Team)).transform.position) < 5f)
        {
            CaptureFlag();
        }
    }
    /// <summary>
    /// Determines whether this instance is at the home base
    /// </summary>
    /// <returns></returns>
    public bool IsHome()
    {
        return transform.position == m_HomePosition;
    }

    void DropFlag()
    {
        if (PhotonNetwork.offlineMode == true)
        {
            OnDrop(transform.position);
        }
        else
        {
            if (PhotonNetwork.isMasterClient == true)
            {
                PhotonView.RPC("OnDrop", PhotonTargets.AllBuffered, new object[] { transform.position });
            }
        }
    }

    void ReturnFlag()
    {
        if (PhotonNetwork.offlineMode == true)
        {
            OnReturn();
        }
        else
        {
            if (PhotonNetwork.isMasterClient == true)
            {
                PhotonView.RPC("OnReturn", PhotonTargets.AllBuffered);
            }
        }
    }

    void CaptureFlag()
    {
        if (PhotonNetwork.offlineMode == true)
        {
            OnCapture(PhotonNetwork.player);
        }
        else
        {        
                PhotonView.RPC("OnCapture", PhotonTargets.AllBuffered,PhotonNetwork.player);
        }
    }

    [RPC]
    void OnDrop(Vector3 position)
    {
        m_CarryingPlayer = null;
        transform.position = position;
        m_ReturnTimer = ReturnTime;
    }

    [RPC]
    void OnCapture(PhotonPlayer m_actor)
    {
        m_CarryingPlayer = null;
        transform.position = m_HomePosition;

        //Only the player who captures the flag, updates the properties
        if (PhotonNetwork.player == m_actor)
        {
            bl_EventHandler.KillEvent(PhotonNetwork.player.name, "", "Has capture Flag", (string)PhotonNetwork.player.customProperties[PropiertiesKeys.TeamKey], 777, 15);
            IncreaseScore();
        }
    }

    [RPC]
    void OnReturn()
    {
        transform.position = m_HomePosition;
    }

    void IncreaseScore()
    {
        //We need to know which property we have to change, blue or red
        string property = PropiertiesKeys.Team1Score;

        if ((string)PhotonNetwork.player.customProperties[PropiertiesKeys.TeamKey] == Team.Recon.ToString())
        {
            property = PropiertiesKeys.Team2Score;
        }
        ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable();

        if (PhotonNetwork.room.customProperties.ContainsKey(property) == true)
        {
            int score = (int)PhotonNetwork.room.customProperties[property];
            score++;
            //if the property does exist, we just add one to the old value
            newProperties[property] =  score;
        }
        PhotonNetwork.room.SetCustomProperties(newProperties);
        //Add Point for personal score

         int t_score = Random.Range(150, 200);//Add your custom bonus
         PhotonNetwork.player.PostScore(t_score);
    }

    public override bool CanBePickedUpBy(bl_CTFPlayerLogic logic)
    {
        //If the flag is at its home position, only the enemy team can grab it
        if (IsHome() == true)
        {
            return logic.m_PlayerTeam != Team;
        }

        //If another player is already carrying the flag, no one else can grab it
        if (m_CarryingPlayer != null)
        {
            return false;
        }

        return true;
    }

    public override void OnPickup(bl_CTFPlayerLogic logic)
    {
        if (logic.m_PlayerTeam == Team)
        {
            if (IsHome() == false)
            {
                ReturnFlag();
            }
        }
        else
        {
            m_CarryingPlayer = logic;
        }
    }

    void OnGUI()
    {
        GUI.color = IconColor;
        if (Camera.main)
        {
            Vector3 vector = Camera.main.WorldToScreenPoint(this.IconTarget.position);
            if (vector.z > 0)
            {
                    GUI.DrawTexture(new Rect(vector.x - 5, (Screen.height - vector.y) - 7, 13+IconSize.x, 13+IconSize.y), this.FlagIcon);
            }
        }
        GUI.color = Color.white;
    }
}