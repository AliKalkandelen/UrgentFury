using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class bl_Login : MonoBehaviour {

    public string LoginPHP_URL = "";
    public string NextLevel = "";
    [Space(5)]
    public string m_User = "";
    public string m_Password = "";
    public bool KeepMe = false;
    [Space(5)]
    public Toggle mToggle = null;
    public InputField m_UserInput = null;
    public InputField m_PassInput = null;

    //Private
    private bool isLogin = false;
    private bool AlredyLogin = false;

    /// <summary>
    /// Get Name in Init
    /// </summary>
    void Awake()
    {
        if (mToggle != null)
        {
            if (PlayerPrefs.GetString(bl_LoginManager.SavedUser) != string.Empty)
            {
                mToggle.isOn = true;
                if (m_User != null)
                {
                    m_UserInput.text = PlayerPrefs.GetString(bl_LoginManager.SavedUser);
                }
            }
            else
            {
                mToggle.isOn = false;
            }
        }
    }

	// Update is called once per frame
    void Update()
    {
        if (mToggle != null)
        {
            KeepMe = mToggle.isOn;
        }
        if (m_UserInput != null)
        {
            m_User = m_UserInput.text;
        }
        if (m_PassInput != null)
        {
            m_Password = m_PassInput.text;
        }
    }

    /// <summary>
    /// Login detect if user and password is not emty
    /// and startcorrutine for  connection with DB
    /// </summary>
    public void Login()
    {
        if (isLogin || AlredyLogin)
            return;

        if (m_User != string.Empty && m_Password != string.Empty)
        {
            StartCoroutine(LoginProcess());
        }
        else
        {
            if (m_User == string.Empty)
            {
                Debug.LogWarning("User Name is Empty");
            }
            if (m_Password == string.Empty)
            {
                Debug.LogWarning("Password is Empty");
            }
            bl_LoginManager.UpdateDescription("complete all fields");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator LoginProcess()
    {
        if (isLogin || AlredyLogin)
            yield return null;

        isLogin = true;
        bl_LoginManager.UpdateDescription("Login...");
        // Create instance of WWWForm
        WWWForm mForm = new WWWForm();
        //sets the mySQL query to the amount of rows to load
        mForm.AddField("name", m_User);
        mForm.AddField("password", m_Password);
        //Creates instance to run the php script to access the mySQL database
        WWW www = new WWW(LoginPHP_URL, mForm);
        yield return www;

        string result = www.text;
        string[] mSplit = result.Split(","[0]);

        if (mSplit[0] == "Login Done")
        {
            Debug.Log("Login " + mSplit[1]);
            AlredyLogin = true;
            //get vals
            string name = mSplit[1];
            int kills = int.Parse(mSplit[2]);
            int deaths = int.Parse(mSplit[3]);
            int score = int.Parse(mSplit[4]);

            bl_LoginManager.UpdateDescription("Welcome "+RichColor(mSplit[1],"orange")+"\n Kills: <color=red>" + mSplit[2]+"</color> \n Deaths: <color=orange>"+
                mSplit[3] + "</color> \n Score: <color=orange>" + mSplit[4] + "</color>");
            if (KeepMe)
            {
                PlayerPrefs.SetString(bl_LoginManager.SavedUser, m_User);
            }
            else
            {
                PlayerPrefs.SetString(bl_LoginManager.SavedUser, string.Empty);
            }
            bl_LoginManager.OnLoginEvent(name, kills, deaths, score);
            yield return new WaitForSeconds(1f);
            Application.LoadLevel(NextLevel);
        }
        else
        {
            Debug.LogWarning(www.text);
            bl_LoginManager.UpdateDescription(www.text);
        }
        isLogin = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    private string RichColor(string text, string color)
    {

        return "<color=" + color + ">" + text + "</color>";
    }
}
