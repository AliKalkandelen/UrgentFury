using UnityEngine;
using System.Collections;

public class bl_LoadScene : MonoBehaviour {
    public string Level;
    public GUISkin Skin;
    public Texture2D Vignette;

    void OnGUI()
    {
        GUI.skin = Skin;

        if (GUI.Button(new Rect(Screen.width - 300, Screen.height - 75, 200, 35), "Return"))
        {
            Application.LoadLevel(Level);
        }
        if (Vignette)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Vignette);
        }
        GUI.TextArea(new Rect(2,10,200,100),"Change Weapon With <color=orange>[1,2,3]</color>","label");
    }
}
