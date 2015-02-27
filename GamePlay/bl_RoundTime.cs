/////////////////////////////////////////////////////////////////////////////////
///////////////////////////////bl_RoundTime.cs///////////////////////////////////
///////////////Use this to manage time in rooms//////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////
////////////////////////////////Briner Games/////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable; //Replace default Hashtables with Photon hashtables
using UnityEngine.UI;

public class bl_RoundTime : MonoBehaviour {

    public GUISkin Style;
    /// <summary>
    /// mode of the round room
    /// </summary>
    public RoundStyle m_RoundStyle;
    /// <summary>
    /// expected duration in round (automatically obtained)
    /// </summary>
    public int RoundDuration;
    public bl_GameManager m_Manager = null;
    [HideInInspector]
    public float CurrentTime;
    [System.Serializable]
    public class UI_
    {
        public Text TimeText;
    }
    public UI_ UI;
    //private
    private const string StartTimeKey = "RoomTime";       // the name of our "start time" custom property.
    private float m_Reference;
    private int m_countdown = 10;
    private bool isFinish = false;
    private bl_SettingPropiertis m_propiertis;
    private bl_RoomMenu RoomMenu;

    void Awake()
    {
        if (!PhotonNetwork.connected)
        {
            Application.LoadLevel(0);
            return;
        }

        GetTime();
        m_propiertis = this.GetComponent<bl_SettingPropiertis>();
        RoomMenu = this.GetComponent<bl_RoomMenu>();
    }
    /// <summary>
    /// get the current time and verify if it is correct
    /// </summary>
    void GetTime()
    {
        RoundDuration = (int)PhotonNetwork.room.customProperties[PropiertiesKeys.TimeRoomKey];
        if (PhotonNetwork.isMasterClient)
        {
            m_Reference = (float)PhotonNetwork.time;

            Hashtable startTimeProp = new Hashtable();  // only use ExitGames.Client.Photon.Hashtable for Photon
            startTimeProp.Add(StartTimeKey, m_Reference);
            PhotonNetwork.room.SetCustomProperties(startTimeProp);
        }
        else
        {
            m_Reference = (float)PhotonNetwork.room.customProperties[StartTimeKey];
        }
    }

    void FixedUpdate()
    {
        float t_time = RoundDuration - ((float)PhotonNetwork.time - m_Reference);
        if (t_time > 0)
        {
            CurrentTime = t_time;
        }
        else if (t_time <= 0.001 && GetTimeServed == true)//Round Finished
        {
            CurrentTime = 0;
            
            bl_EventHandler.OnRoundEndEvent();
            if (!isFinish)
            {
                isFinish = true;
                RoomMenu.isFinish = true;
                InvokeRepeating("countdown", 1, 1);
            }
        }
        else//even if I do not photonnetwork.time then obtained to regain time
        {
            Refresh();
        }
    }

    void OnGUI()
    {
        GUI.skin = Style;
        //Display Time Round
        int normalSecons = 60;
        float remainingTime = Mathf.CeilToInt(CurrentTime);
        int m_Seconds = Mathf.FloorToInt(remainingTime % normalSecons);
        int m_Minutes = Mathf.FloorToInt((remainingTime / normalSecons) % normalSecons);
        string t_time = bl_UtilityHelper.GetTimeFormat(m_Minutes, m_Seconds);

        //OnGUI version
        /*GUILayout.BeginArea(new Rect(Screen.width / 2 - 100, 0, 200, 35));
        GUILayout.Box("Remaing \n"+t_time,TimeStyle,GUILayout.Height(35));
        GUILayout.EndArea();*/

        if (UI.TimeText != null)
        {
            UI.TimeText.text = "<size=9>Remaing</size> \n" + t_time;
        }

        if (isFinish)
        {
            if (m_RoundStyle == RoundStyle.OneMacht)
            {
              GUI.Label(new Rect(Screen.width / 2 - 30, Screen.height / 2 + 25, 225, 60), "<size=15>Return to Lobby in</size> \n <size=30><color=orange>    " + bl_UtilityHelper.GetDoubleChar(m_countdown) + "</color></size>");
            }
            else if (m_RoundStyle == RoundStyle.Rounds)
            {
                GUI.Label(new Rect(Screen.width / 2 - 30, Screen.height / 2 + 25, 225, 60), "<size=15>Next Round in </size>\n <size=30><color=orange>    " + bl_UtilityHelper.GetDoubleChar(m_countdown) + "</color></size>");
            }
        }
    }

    public GUIStyle TimeStyle
    {
        get
        {
            if (Style != null)
            {
                return Style.customStyles[1];
            }
            else
            {
                return null;
            }
        }
    }
    /// <summary>
    /// with this fixed the problem of the time lag in the Photon
    /// </summary>
    void Refresh()
    {
        if (PhotonNetwork.isMasterClient)
        {
            m_Reference = (float)PhotonNetwork.time;

            Hashtable startTimeProp = new Hashtable();  // only use ExitGames.Client.Photon.Hashtable for Photon
            startTimeProp.Add(StartTimeKey, m_Reference);
            PhotonNetwork.room.SetCustomProperties(startTimeProp);
        }
        else
        {
            m_Reference = (float)PhotonNetwork.room.customProperties[StartTimeKey];
        }
    }

    void countdown()
    {
        m_countdown--;
        if (m_countdown <= 0)
        {
            FinishGame();
            CancelInvoke("countdown");
            m_countdown = 10;
        }
    }

    void FinishGame()
    {
        Screen.lockCursor = false;
        if (m_RoundStyle == RoundStyle.OneMacht)
        {
            if (PhotonNetwork.connected)
            {
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                Application.LoadLevel(0);
            }
        }
        if (m_RoundStyle == RoundStyle.Rounds)
        {
            GetTime();
            if (m_propiertis)
            {
                m_propiertis.SettingPropiertis();
            }
            isFinish = false;          
            if (m_Manager == null)
                return;

                m_Manager.DestroyPlayer();
            if (RoomMenu != null)
            {
                RoomMenu.isFinish = false;
                RoomMenu.isPlaying = false;
                RoomMenu.showMenu = true;
                Screen.lockCursor = false;
            }
        }
    }

    bool GetTimeServed
    {
        get
        {
            bool m_bool = false ;
            if (Time.timeSinceLevelLoad > 7)
            {
                m_bool = true;
            }
            return m_bool;
        }
    }

}
