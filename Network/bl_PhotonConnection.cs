using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class bl_PhotonConnection : Photon.MonoBehaviour {


    private LobbyState m_state = LobbyState.PlayerName;
    public List<GameObject> MenusUI = new List<GameObject>();
	private string playerName;
	private string hostName; //Name of room
    [Header("Photon")]
    public string AppID = string.Empty;
    public string AppVersion = "1.0";
    public int Port = 5055;
    [Space(5)]
    public GUISkin Skin;
    private float alpha = 2.0f;
	
	//OPTIONS
    private int m_currentQuality = 3;
    private float m_volume = 1.0f;
    private float m_sensitive = 15;
    private string[] m_stropicOptions = new string[] { "Disable", "Enable", "Force Enable" };
    private int m_stropic = 0;
    private bool GamePerRounds = false;
    private bool AutoTeamSelection = false;

    public bool ShowPhotonStatus = false;
    public bool ShowPhotonStatics = true;
	[HideInInspector]
    public bool loading = false;
    [Space(5)]
    public string[] GameModes;
    private int CurrentGameMode = 0;
	
	//Max players in game
    public int[] maxPlayers;
	private int players;
	//Room Time in seconds
    public int[] RoomTime;
    private int r_Time;
    //SERVERLIST
    private RoomInfo[] roomList;
    private bool listRefreshed = false;
    private Vector2 scroll;
    private bool FirstConnect = false;
    [Space(5)]
    [Header("Effects")]
    public AudioClip a_Click;
    public AudioClip backSound;
    [System.Serializable]
    public class AllScenes
    {
        public string m_name;
        public string m_SceneName;
        public Texture2D m_Preview;
    }
    public List<AllScenes> m_scenes = new List<AllScenes>();
    private int CurrentScene = 0;

	void Awake (){
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;

        // the following line checks if this client was just created (and not yet online). if so, we connect
        if (!PhotonNetwork.connected || PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated)
        {
            PhotonNetwork.ConnectUsingSettings(AppVersion);
        }
        hostName = "myRoom" + Random.Range(10, 999);
        // generate a name for this player, if none is assigned yet
        if (String.IsNullOrEmpty(playerName))
        {
            playerName = "Guest" + Random.Range(1, 9999);
        }
        GetPrefabs();
	}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="t_state"></param>
    /// <returns></returns>
	IEnumerator Fade ( LobbyState t_state  ){
		alpha = 0.0f;
        m_state = t_state;
		loading = true;
			while (alpha < 2.0f) {
				alpha += Time.deltaTime / 0.5f;
				yield return 0;		
			}

		loading = false;
	}
    /// <summary>
    /// 
    /// </summary>
	void OnGUI (){
		GUI.skin = Skin;
        if (m_state == LobbyState.ChangeServer)
        {
            SelectServer();
        }
        if (PhotonNetwork.connected && !PhotonNetwork.connecting )
        {
            FirstConnect = true;
            if (m_state == LobbyState.PlayerName)
            {
                GUI.Label(new Rect(Screen.width - 150, 20, 150, 30), "<b><size=14> Version 1.0f </size></b>");
            }

            if (ShowPhotonStatus)
            {
                GUI.Label(new Rect(0, 55, 500, 20), " <b><color=orange> Status:</color>  " + PhotonNetwork.connectionStateDetailed.ToString() + "</b>");
            }
            if (ShowPhotonStatics)
            {
                ShowStaticSever();
            }

            if (loading && m_state != LobbyState.PlayerName)
            {
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }

            //Everything after this line will be affected when we will change "alpha"	
            GUI.color = new Color(1.0f, 1.0f, 1.0f, alpha);

            if (m_state != LobbyState.PlayerName)
            {
                MainMenu();
            }
            if (m_state == LobbyState.PlayerName)
            {
                EnterName();
            }
            else if (m_state == LobbyState.Join)
            {
                ServerList();
            }
            else if (m_state == LobbyState.MainMenu)
            {
                ServerList();
            }
            else if (m_state == LobbyState.Host)
            {
                CreateServer();
            }
            else if (m_state == LobbyState.Settings)
            {
                Settings();
            }
            else if (m_state == LobbyState.Quit)
            {
                QuitGame();
            }

            if (m_state != LobbyState.PlayerName && m_state != LobbyState.MainMenu)
            {
                ButtonBack();
            }
        }
        else if(!FirstConnect)
        {
            if (PhotonNetwork.connecting)
            {
                GUI.Box(new Rect(Screen.width / 2 - 100, Screen.height / 2, 200, 60), "Connecting...", "window");
            }
            else
            {
                GUILayout.Label("Not connected. Check console output. (" + PhotonNetwork.connectionState + ")");
            }
        }
	}
    /// <summary>
    /// its for UI 4.6 (WIP)
    /// </summary>
    /// <param name="id"></param>
    public void ChangeMenu(int id)
    {
        switch (id)
        {
            case 0 :
                MenusUI[0].SetActive(true);
                MenusUI[1].SetActive(false);
                MenusUI[2].SetActive(false);
                MenusUI[3].SetActive(false);
                MenusUI[4].SetActive(false);
                MenusUI[5].SetActive(false);
                MenusUI[6].SetActive(false);
                break;
            case 1:
                MenusUI[0].SetActive(false);
                MenusUI[1].SetActive(true);
                MenusUI[2].SetActive(false);
                MenusUI[3].SetActive(false);
                MenusUI[4].SetActive(false);
                MenusUI[5].SetActive(false);
                MenusUI[6].SetActive(true);
                break;
            case 2:
                MenusUI[0].SetActive(false);
                MenusUI[1].SetActive(false);
                MenusUI[2].SetActive(true);
                MenusUI[3].SetActive(false);
                MenusUI[4].SetActive(false);
                MenusUI[5].SetActive(false);
                MenusUI[6].SetActive(true);
                break;
            case 3:
                MenusUI[0].SetActive(false);
                MenusUI[1].SetActive(false);
                MenusUI[2].SetActive(false);
                MenusUI[3].SetActive(true);
                MenusUI[4].SetActive(false);
                MenusUI[5].SetActive(false);
                MenusUI[6].SetActive(true);
                break;
            case 4:
                MenusUI[0].SetActive(false);
                MenusUI[1].SetActive(false);
                MenusUI[2].SetActive(false);
                MenusUI[3].SetActive(false);
                MenusUI[4].SetActive(true);
                MenusUI[5].SetActive(false);
                MenusUI[6].SetActive(true);
                break;
            case 5:
                MenusUI[0].SetActive(false);
                MenusUI[1].SetActive(false);
                MenusUI[2].SetActive(false);
                MenusUI[3].SetActive(false);
                MenusUI[4].SetActive(false);
                MenusUI[5].SetActive(true);
                MenusUI[6].SetActive(true);
                break;
        }
    }
    /// <summary>
    /// Menu For Enter Name for UI 4.6 WIP
    /// </summary>
	public void EnterName (){

        playerName = playerName.Replace("\n", "");
        GUILayout.BeginArea(new Rect(Screen.width / 2 - 175, Screen.height / 2 - 57, 375, 135), "<color=black>Player Name</color>","window");
        GUILayout.Space(10);
        GUILayout.BeginHorizontal("box");
        GUILayout.Label("Player Name : ");
        GUILayout.Space(5);
        GUI.SetNextControlName("user");
        playerName = playerName.Replace("\n", "");
        playerName = GUILayout.TextField(playerName, 20, GUILayout.Width(223), GUILayout.Height(33));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Quit",GUILayout.Width(100),GUILayout.Height(33)))
        {
            PlayAudioClip(backSound, transform.position, 1.0f);
            m_state = LobbyState.Quit;
        }
        GUILayout.Space(100);
        if (GUILayout.Button("Continue",GUILayout.Width(150),GUILayout.Height(33)))
        {
            if (playerName != string.Empty && playerName.Length > 3)
            {
                PlayAudioClip(a_Click, transform.position, 1.0f);
                StartCoroutine(Fade(LobbyState.MainMenu));
                StartCoroutine(RefresListIE());
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
	}
    /// <summary>
    /// Menu GUI for select a server to connect
    /// </summary>
    void SelectServer()
    {
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.Disconnect();
        }
        GUILayout.BeginArea(new Rect(Screen.width / 2 - 175, Screen.height / 2 - 150, 350, 300), "", "window");
        GUILayout.Space(10);
        GUILayout.Label("Available Regions");
        GUILayout.Space(10);
        if (GUILayout.Button("US (East Coast)",GUILayout.Height(35)))
        {
            if (AppID != string.Empty)
            {
                PhotonNetwork.ConnectToMaster("app-us.exitgamescloud.com", Port, AppID, AppVersion);
                PlayAudioClip(a_Click, transform.position, 1.0f);
                StartCoroutine(Fade(LobbyState.MainMenu));
            }
        }
        if (GUILayout.Button("EU (Amsterdam)", GUILayout.Height(35)))
        {
            if (AppID != string.Empty)
            {
                PhotonNetwork.ConnectToMaster("app-eu.exitgamescloud.com", Port, AppID, AppVersion);
                PlayAudioClip(a_Click, transform.position, 1.0f);
                StartCoroutine(Fade(LobbyState.MainMenu));
            }
        }
        if (GUILayout.Button("Asia (Singapore)", GUILayout.Height(35)))
        {
            if (AppID != string.Empty)
            {
                PhotonNetwork.ConnectToMaster("app-asia.exitgamescloud.com", Port, AppID, AppVersion);
                PlayAudioClip(a_Click, transform.position, 1.0f);
                StartCoroutine(Fade(LobbyState.MainMenu));
            }
        }
        if (GUILayout.Button("Japan (Tokyo)", GUILayout.Height(35)))
        {
            if (AppID != string.Empty)
            {
                PhotonNetwork.ConnectToMaster("app-jp.exitgamescloud.com", Port, AppID, AppVersion);
                PlayAudioClip(a_Click, transform.position, 1.0f);
                StartCoroutine(Fade(LobbyState.MainMenu));
            }
        }
        GUILayout.EndArea();
        GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 165, 400, 100), "Choose the server nearest your location\n since this depends on the <color=orange>Ping.</color>");
    }

    /// <summary>
    /// 
    /// </summary>
	void MainMenu (){
        GUILayout.BeginArea(new Rect(0,0,Screen.width,60));
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Search Room", GUILayout.Height(GetButtonSize(LobbyState.Join))))
        {
            m_state = LobbyState.Join;
            StartCoroutine(RefresListIE());
            PlayAudioClip(a_Click, transform.position, 1.0f);
        }
        if (GUILayout.Button("Host Room", GUILayout.Height(GetButtonSize(LobbyState.Host))))
        {
            m_state = LobbyState.Host;
            PlayAudioClip(a_Click, transform.position, 1.0f);
        }
        if (GUILayout.Button("Setting", GUILayout.Height(GetButtonSize(LobbyState.Settings))))
        {
            m_state = LobbyState.Settings;
            PlayAudioClip(a_Click, transform.position, 1.0f);
        }
        if (GUILayout.Button("Change Server", GUILayout.Height(GetButtonSize(LobbyState.ChangeServer))))
        {
            m_state = LobbyState.ChangeServer;
            PlayAudioClip(a_Click, transform.position, 1.0f);
        }
        if (GUILayout.Button("Quit", GUILayout.Height(GetButtonSize(LobbyState.Quit))))
        {
            m_state = LobbyState.Quit;
            PlayAudioClip(a_Click, transform.position, 1.0f);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
	}		
    /// <summary>
    /// 
    /// </summary>
	void CreateServer (){
		GUILayout.BeginArea( new Rect(Screen.width/2 - 225, Screen.height/2 - 165, 450, 400), "","window");
        GUILayout.Space(10);
        GUILayout.BeginVertical();
        GUILayout.Space(15);
			GUI.Box ( new Rect(30,35,150,30),"Host Name: ");
				hostName = hostName.Replace("\n", "");
				hostName = GUI.TextField ( new Rect(200,35,220,30), hostName, 20);
     

        //Max Player Select
                GUILayout.BeginHorizontal();
				GUI.Box ( new Rect(30,70,150,30),"Max Players: ");
					if (GUI.Button( new Rect(200,70,40,30), "<<", "Box")){
						if(players < maxPlayers.Length){ 
							players--;	
							if(players < 0) players = maxPlayers.Length - 1;
						}	
					}

				GUI.Box( new Rect(260,70,100,30), maxPlayers[players].ToString());
					if (GUI.Button( new Rect(380,70,40,30), ">>", "Box")){
						if(players < maxPlayers.Length){ 
							players++;
							if(players > (maxPlayers.Length - 1)) players = 0;	
								
						}	
					}
                    GUILayout.EndHorizontal();
        //Room Time select
                   GUILayout.BeginHorizontal();
                    GUI.Box(new Rect(30, 110, 150, 30), "Max Time: ");
                    if (GUI.Button(new Rect(200, 110, 40, 30), "<<", "Box"))
                    {
                        if (r_Time < RoomTime.Length)
                        {
                            r_Time--;
                            if (r_Time < 0)
                            {
                                r_Time = RoomTime.Length - 1;

                            }
                        }
                    }

                    GUI.Box(new Rect(260, 110, 100, 30), (RoomTime[r_Time] / 60) + " <size=12>Min</size>");
                    if (GUI.Button(new Rect(380, 110, 40, 30), ">>", "Box"))
                    {
                        if (r_Time < RoomTime.Length)
                        {
                            r_Time++;
                            if (r_Time > (RoomTime.Length - 1))
                            {
                                r_Time = 0;

                            }

                        }
                    }
                    GUILayout.EndHorizontal();
        //GameMode Select
                    GUILayout.BeginHorizontal();
                    GUI.Box(new Rect(30, 150, 150, 30), "Game Mode: ");
                    if (GUI.Button(new Rect(200, 150, 40, 30), "<<", "Box"))
                    {
                        if (CurrentGameMode < GameModes.Length)
                        {
                            CurrentGameMode--;
                            if (CurrentGameMode < 0)
                            {
                                CurrentGameMode = GameModes.Length - 1;

                            }
                        }
                    }

                    GUI.Box(new Rect(260, 150, 100, 30), GameModes[CurrentGameMode]);
                    if (GUI.Button(new Rect(380, 150, 40, 30), ">>", "Box"))
                    {
                        if (CurrentGameMode < GameModes.Length)
                        {
                            CurrentGameMode++;
                            if (CurrentGameMode > (GameModes.Length - 1))
                            {
                                CurrentGameMode = 0;
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (GUI.Button(new Rect(25, 190, 75, 30), "<<","Box"))
                    {
                        if (CurrentScene < m_scenes.Count)
                        {
                            CurrentScene--;
                            if (CurrentScene < 0)
                            {
                                CurrentScene = m_scenes.Count - 1;

                            }
                        }
                    }
                    GUI.DrawTexture(new Rect(100, 190, 250, 100), m_scenes[CurrentScene].m_Preview);
                    GUI.Box(new Rect(100, 265, 250, 25), m_scenes[CurrentScene].m_name);
                    if (GUI.Button(new Rect(350, 190, 75, 30), ">>", "Box"))
                    {
                        if (CurrentScene < m_scenes.Count)
                        {
                            CurrentScene++;
                            if (CurrentScene > (m_scenes.Count - 1))
                            {
                                CurrentScene = 0;
                            }
                        }
                    }
                    GUILayout.EndHorizontal();

                    GamePerRounds = GUI.Toggle(new Rect(265, 300, 200, 30), GamePerRounds, "Game Per Rounds");
                    AutoTeamSelection = GUI.Toggle(new Rect(30, 300, 200, 30), AutoTeamSelection, "Auto Team Selection");
                    

				GUILayout.BeginHorizontal();	
		//Create a new Room with Photon		
				if(GUI.Button( new Rect(150,330,175,50),"Create Game")) { 
					PhotonNetwork.player.name = playerName;
                    //Save Room properties for load in room
					ExitGames.Client.Photon.Hashtable roomOption = new ExitGames.Client.Photon.Hashtable();
                    roomOption[PropiertiesKeys.TimeRoomKey] = RoomTime[r_Time];
                    roomOption[PropiertiesKeys.GameModeKey] = GameModes[CurrentGameMode];
                    roomOption[PropiertiesKeys.SceneNameKey] = m_scenes[CurrentScene].m_SceneName;
                    roomOption[PropiertiesKeys.RoomRoundKey] = GamePerRounds ? "1" : "0";
                    roomOption[PropiertiesKeys.TeamSelectionKey] = AutoTeamSelection ? "1" : "0";

					string[] properties= new string[5];
                    properties[0] = PropiertiesKeys.TimeRoomKey;
                    properties[1] = PropiertiesKeys.GameModeKey;
                    properties[2] = PropiertiesKeys.SceneNameKey;
                    properties[3] = PropiertiesKeys.RoomRoundKey;
                    properties[4] = PropiertiesKeys.TeamSelectionKey;

                    PhotonNetwork.CreateRoom(hostName, new RoomOptions() { maxPlayers = maxPlayers[players], isVisible = true,isOpen = true,
                                                                           customRoomProperties = roomOption,
                                                                           cleanupCacheOnLeave = true,
                                                                           customRoomPropertiesForLobby = properties
                    }, null);
				}

				GUILayout.Space(20);	
			GUILayout.EndHorizontal();
            GUILayout.EndVertical();
		GUILayout.EndArea();	
	}
    /// <summary>
    /// 
    /// </summary>
	void ServerList (){
		GUILayout.BeginArea( new Rect(Screen.width/2 - 400, Screen.height/2 - 180, 800, 400),Skin.customStyles[3]);

        GUI.Box(new Rect(25, 10, 150, 35), "Host Name");
        GUI.Box(new Rect(180, 10, 140, 35), "Map Name");
        GUI.Box(new Rect(325, 10, 130, 35), "Game Mode");
        GUI.Box(new Rect(460, 10, 80, 35), "Players");
        GUI.Box(new Rect(545, 10, 60, 35), "Ping");
        //Refresh ServerList
        if (GUI.Button(new Rect(610, 10, 130, 35), "Refresh"))
        {
            StartCoroutine(RefresListIE());

        }
		
				scroll = GUI.BeginScrollView( new Rect(10,67,770,315),scroll,new Rect (0, 0, 220, 1000), false, true);
				
					if(listRefreshed && !loading){
						foreach( RoomInfo room in roomList){
							GUILayout.BeginHorizontal("Label");

								GUILayout.Box(room.name, GUILayout.Width(150), GUILayout.Height(30));							
								GUILayout.Box((string)room.customProperties[PropiertiesKeys.SceneNameKey], GUILayout.Width(140), GUILayout.Height(30));
								GUILayout.Box((string)room.customProperties[PropiertiesKeys.GameModeKey], GUILayout.Width(130), GUILayout.Height(30));		
								GUILayout.Box(room.playerCount + "/" + room.maxPlayers, GUILayout.Width(80), GUILayout.Height(30));
								GUILayout.Box(PhotonNetwork.GetPing().ToString(), GUILayout.Width(60), GUILayout.Height(30));
                                if (GUILayout.Button("Join Game", GUILayout.Width(120), GUILayout.Height(30)))
                                {
                                    if (room.playerCount < room.maxPlayers)
                                    {
                                        PhotonNetwork.JoinRoom(room.name);
                                        PhotonNetwork.playerName = playerName;
                                    }
                                }
							GUILayout.EndHorizontal();
						}

                        if (roomList.Length == 0 && !loading) GUI.Label(new Rect(320, 150, 200, 30), "No room created ... create one.");
					}
				GUI.EndScrollView();
		GUILayout.EndArea();
	}
	/// <summary>
	/// Menu GUI for settings
	/// </summary>
	void Settings (){
	
		GUILayout.BeginArea( new Rect(Screen.width/2 - 250, Screen.height/2 - 170, 500, 350),"","window");
        GUILayout.Space(10);
        GUILayout.Box("Setting");
        GUILayout.Label("Quality Level");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<<"))
        {
            if (m_currentQuality < QualitySettings.names.Length)
            {
                m_currentQuality--;
                if (m_currentQuality < 0)
                {
                    m_currentQuality = QualitySettings.names.Length - 1;

                }
            }
        }
        GUILayout.Box(QualitySettings.names[m_currentQuality]);
        if (GUILayout.Button(">>"))
        {
            if (m_currentQuality < QualitySettings.names.Length)
            {
                m_currentQuality++;
                if (m_currentQuality > (QualitySettings.names.Length - 1))
                {
                    m_currentQuality  = 0;
                }
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Label("Anisotropic Filtering");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("<<"))
        {
            if (m_stropic < m_stropicOptions.Length)
            {
                m_stropic--;
                if (m_stropic < 0)
                {
                    m_stropic = m_stropicOptions.Length - 1;

                }
            }
        }
        GUILayout.Box(m_stropicOptions[m_stropic]);
        if (GUILayout.Button(">>"))
        {
            if (m_stropic < m_stropicOptions.Length)
            {
                m_stropic++;
                if (m_stropic > (m_stropicOptions.Length - 1))
                {
                    m_stropic = 0;
                }
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Label("Sound Volume");
        GUILayout.BeginHorizontal();
        m_volume = GUILayout.HorizontalSlider(m_volume, 0.0f, 1.0f);
        GUILayout.Label((m_volume*100).ToString("000"),GUILayout.Width(30));
        GUILayout.EndHorizontal();
        GUILayout.Label("Sensitivity");
        GUILayout.BeginHorizontal();
        m_sensitive = GUILayout.HorizontalSlider(m_sensitive, 0.0f, 100.0f);
        GUILayout.Label(m_sensitive.ToString("000"), GUILayout.Width(30));
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Save"))
        {
            Save();
        }
        GUILayout.EndArea();
	}

	/// <summary>
	/// a simple window for quit of game (only work in Window o Mac Build)
    /// if you use Web Player, not need this
	/// </summary>
	void QuitGame (){

        GUILayout.BeginArea(new Rect(Screen.width / 2 - 175, Screen.height / 2 - 100, 300, 150), "Quit", "window");
        GUILayout.Space(35);
        GUILayout.Label("Are you sure you want to exit game?");
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Space(35);
        if (GUILayout.Button("Cancel", GUILayout.Width(100), GUILayout.Height(35)))
        {
            StartCoroutine(Fade(LobbyState.MainMenu));
        }
        GUILayout.Space(15);
        if (GUILayout.Button("Ok",GUILayout.Width(100),GUILayout.Height(35)))
        {
            Application.Quit();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
	}	
	
	void ButtonBack (){
		GUILayout.BeginArea ( new Rect(10, (Screen.height - 35) , 200, 100));
        if (GUILayout.Button("Back",GUILayout.Height(35)))
        {
				PlayAudioClip(backSound, transform.position, 1.0f);
				listRefreshed = false;
                StartCoroutine(Fade(LobbyState.MainMenu));
			}
		GUILayout.EndArea();
	}

    void ShowStaticSever()
    {
        GUILayout.BeginArea(new Rect(225,Screen.height - 35,Screen.width - 225 ,50));
        GUILayout.BeginHorizontal();
        GUILayout.Label("<color=orange>"+PhotonNetwork.countOfPlayersInRooms + "</color> Players in Rooms");
        GUILayout.Label("<color=orange>" + PhotonNetwork.countOfPlayersOnMaster + "</color> Players in Lobby");
        GUILayout.Label("<color=orange>" + PhotonNetwork.countOfPlayers + "</color> Players in Server");
        GUILayout.Label("<color=orange>" + PhotonNetwork.countOfRooms + "</color> Room are Create");
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
    void Save()
    {
        PlayerPrefs.SetFloat("volumen", m_volume);
        PlayerPrefs.SetFloat("sensitive", m_sensitive);
        PlayerPrefs.SetInt("quality", m_currentQuality);
        PlayerPrefs.SetInt("anisotropic", m_stropic);
        Debug.Log("Save Done!");
    }

	/// <summary>
    /// get the list of rooms available to join
	/// </summary>
	/// <returns></returns>
	IEnumerator RefresListIE (){
		loading = true;
			yield return new WaitForSeconds( 1.0f); 
			roomList = PhotonNetwork.GetRoomList();	
		loading = false;
		listRefreshed = true;
	}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="position"></param>
    /// <param name="volume"></param>
    /// <returns></returns>
	AudioSource PlayAudioClip ( AudioClip clip ,   Vector3 position ,   float volume  ){
		GameObject go= new GameObject ("One shot audio");
		go.transform.position = position;
		AudioSource source = go.AddComponent<AudioSource>();
		source.clip = clip;
		source.volume = volume;
		source.Play ();
		Destroy (go, clip.length);
		return source;
	}
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator MoveToGameScene()
    {
        //Wait for check
        while (PhotonNetwork.room == null)
        {
            yield return 0;
        }
        PhotonNetwork.isMessageQueueRunning = false;
        Application.LoadLevel((string)PhotonNetwork.room.customProperties[PropiertiesKeys.SceneNameKey]); 
    }
    // LOBBY EVENTS

    void OnJoinedLobby()
    {
        Debug.Log("We joined the lobby.");
    }

    void OnLeftLobby()
    {
        Debug.Log("We left the lobby.");
    }	
	
    // ROOMLIST

    void OnReceivedRoomList()
    {
        Debug.Log("We received a new room list, total rooms: " + PhotonNetwork.GetRoomList().Length);
    }

    void OnReceivedRoomListUpdate()
    {
        Debug.Log("We received a room list update, total rooms now: " + PhotonNetwork.GetRoomList().Length);
    }
	
	void OnJoinedRoom (){
        Debug.Log("We have joined a room.");
        StartCoroutine(MoveToGameScene());
	}	
	void OnFailedToConnectToPhoton ( DisconnectCause cause  ){
        Debug.LogWarning("OnFailedToConnectToPhoton: " + cause);
		loading = false;
    }
    void OnConnectionFail(DisconnectCause cause)
    {
		GUI.Label(new Rect(Screen.width /2, Screen.height /2, 150, 30), "<b><size=14> The Connection to the server has failed. Check your internet </size></b>");
        Debug.LogWarning("OnConnectionFail: " + cause);
    }

    void GetPrefabs()
    {
        if (PlayerPrefs.HasKey("volumen"))
        {
            m_volume = PlayerPrefs.GetFloat("volumen");
        }
        if (PlayerPrefs.HasKey("sensitive"))
        {
            m_sensitive = PlayerPrefs.GetFloat("sensitive");
        }
        if (PlayerPrefs.HasKey("quality"))
        {
            m_currentQuality = PlayerPrefs.GetInt("quality");
        }
        if (PlayerPrefs.HasKey("anisotropic"))
        {
            m_stropic = PlayerPrefs.GetInt("anisotropic");
        }
    }

    private int GetButtonSize(LobbyState t_state)
    {

        if (m_state == t_state)
        {
            return 55;
        }
        else
        {
            return 40;
        }
    }
}

