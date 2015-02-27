using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bl_CManager : MonoBehaviour {

    /// <summary>
    /// the object that contains all objects that can customize
    /// </summary>
    public Transform m_Manager;
    /// <summary>
    /// Skin for UI
    /// </summary>
    public GUISkin Skin;
    /// <summary>
    /// Scena be charged when we leave
    /// </summary>
    public string LevelToLoad;
    /// <summary>
    /// if the player.prefabs contains Keys
    /// </summary>
    [HideInInspector]
    public bool Have_Name = false;
    /// <summary>
    /// List all objects that can customize
    /// </summary>
    public List<bl_Customizer> AllCustom = new List<bl_Customizer>();
    public Texture2D Vignette;

    protected string Cryted_Name = string.Empty;


    void Awake()
    {
        if (PlayerPrefs.HasKey("Customizer"))
        {
            Have_Name = true;
            Cryted_Name = PlayerPrefs.GetString("Customizer");
            Active_Custom(Cryted_Name);
        }
        else
        {
            Have_Name = false;
            Enabled_Firts();
        }
    }


    /// <summary>
    /// activate the object in the list with the name of the information
    /// </summary>
    /// <param name="t_active"></param>
    void Active_Custom(string t_active)
    {
        foreach (bl_Customizer c in AllCustom)
        {
            c.gameObject.SetActive(false);
            if (c.Go_Name == t_active)
            {
                c.gameObject.SetActive(true);
            }
        }

    }

    /// <summary>
    /// if not have any information, activate the first item in the list
    /// </summary>
    void Enabled_Firts()
    {
        if (m_Manager.childCount > 0)
        {
            for (int i = 0; i < m_Manager.childCount; i++)
            {
                m_Manager.GetChild(i).gameObject.SetActive(false);
            }

            m_Manager.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("has no child in this Manager: " + m_Manager.name);
        }
    }

    void OnGUI()
    {
        GUI.skin = Skin;
        GUILayout.BeginArea(new Rect(0, Screen.height - 35, Screen.width, 45));
        GUILayout.BeginHorizontal();
        for (int i = 0; i < AllCustom.Count; i++)
        {
            if (GUILayout.Button(AllCustom[i].gameObject.name,GUILayout.Height(35)))
            {
                Active_Custom(AllCustom[i].gameObject.name);
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
        if (Vignette)
        {
            GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),Vignette);
        }
    }
}
