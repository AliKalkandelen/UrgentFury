using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class bl_ChatRoom : bl_PhotonHelper {

    public GUISkin m_Skin;
    [Space(5)]
    public Text ChatText;
    public bool IsVisible = true;
    public bool WithSound = true;
    public int MaxMsn = 7;
    public List<string> messages = new List<string>();
    private string inputLine = "";
    [Space(5)]
    public AudioClip MsnSound;

    public static readonly string ChatRPC = "Chat";
    private float m_alpha = 2f;
    private bool isChat = false;

    void Start()
    {
        Refresh();
    }

    public void OnGUI()
    {
        if (ChatText == null)
            return;

        if (m_alpha > 0.0f && !isChat)
        {
            m_alpha -= Time.deltaTime / 2;
        }
        else if (isChat)
        {
            m_alpha = 10;
        }

        Color t_color = ChatText.color;
        t_color.a = m_alpha;
        ChatText.color = t_color;

        GUI.skin = m_Skin;
        GUI.color = new Color(1, 1, 1, m_alpha);
        if (!this.IsVisible || PhotonNetwork.connectionStateDetailed != PeerState.Joined)
        {
            return;
        }

        if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
        {
            if (!string.IsNullOrEmpty(this.inputLine) && isChat && Screen.lockCursor)
            {
                this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
                this.inputLine = "";
                GUI.FocusControl("");
                isChat = false;
                return; // printing the now modified list would result in an error. to avoid this, we just skip this single frame
            }
            else if (!isChat && Screen.lockCursor)
            {
                GUI.FocusControl("ChatInput");
                isChat = true;
            }
            else
            {
                if (isChat)
                {
                    Closet();
                }
            }
        }

        GUI.SetNextControlName("");
        GUILayout.BeginArea(new Rect(Screen.width / 2 - 150, Screen.height - 35, 300, 50));
        GUILayout.BeginHorizontal();
        GUI.SetNextControlName("ChatInput");
        inputLine = GUILayout.TextField(inputLine);
        if (GUILayout.Button("Send", "box", GUILayout.ExpandWidth(false)))
        {
            this.photonView.RPC("Chat", PhotonTargets.All, this.inputLine);
            this.inputLine = "";
            GUI.FocusControl("");
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

    }

    void Closet()
    {
        isChat = false;
        GUI.FocusControl("");
    }
    /// <summary>
    /// Sync Method
    /// </summary>
    /// <param name="newLine"></param>
    /// <param name="mi"></param>
    [RPC]
    public void Chat(string newLine, PhotonMessageInfo mi)
    {
        m_alpha = 7;
        string senderName = "anonymous";

        if (mi != null && mi.sender != null)
        {
            if (!string.IsNullOrEmpty(mi.sender.name))
            {
                senderName = mi.sender.name;
            }
            else
            {
                senderName = "player " + mi.sender.ID;
            }
        }

        this.messages.Add("[" + senderName + "]: " + newLine);
        if (MsnSound != null && WithSound)
        {
            audio.PlayOneShot(MsnSound);
        }
        if (messages.Count > MaxMsn)
        {
            messages.RemoveAt(0);
        }

        ChatText.text = "";
        foreach (string m in messages)
            ChatText.text += m + "\n";
    }
    /// <summary>
    /// Local Method
    /// </summary>
    /// <param name="newLine"></param>
    public void AddLine(string newLine)
    {
        m_alpha = 7;
        this.messages.Add(newLine);
        if (messages.Count > MaxMsn)
        {
            messages.RemoveAt(0);
        }
    }

    public void Refresh()
    {
        ChatText.text = "";
        foreach (string m in messages)
            ChatText.text += m + "\n";
    }
}
