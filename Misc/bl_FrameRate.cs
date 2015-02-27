///////////////////////////////////////////////////////////////////////////////////////
// bl_FrameRate.cs
//
// Help us get the current Frame Rate the game
// place it in the scena and adds the UI Text
//                           
//                                 Lovatto Studio
///////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;

public class bl_FrameRate : MonoBehaviour
{
    public float accum;
    public int frames;
    public Text TextUI = null;
    //Privates
    private string framerate;
    private float timeleft;
    private float updateInterval = 0.5f;

    void Start()
    {
        this.timeleft = this.updateInterval;
    }

    void Update()
    {
        this.timeleft -= Time.deltaTime;
        this.accum += Time.timeScale / Time.deltaTime;
        this.frames++;
        if (this.timeleft <= 0)
        {
            float num = this.accum / this.frames;
            this.framerate = string.Empty + num.ToString("000");
            this.timeleft = this.updateInterval;
            this.accum = 0;
            this.frames = 0;
        }
        if (TextUI != null)
        {
            TextUI.text = "FPS: <color=orange>" + this.framerate + "</color>";
        }
        else
        {
            Destroy(this);
        }
             
    }
}