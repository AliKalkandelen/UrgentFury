using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class bl_LoginManager : MonoBehaviour {

    //Call all script when Login
    public delegate void LoginEvent(string name,int kills,int deaths,int score);
    public static LoginEvent OnLogin;

    public GameObject PlayerInfo;
    public Animation LoginAnim;
    public Animation RegisterAnim;
    [Space(5)]
    public Text Description = null;
    static Text mDescrip = null;
    public Image BlackScreen = null;

    public const string SavedUser = "UserName";
    private Color alpha = new Color(0, 0, 0, 0);
    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        mDescrip = Description;
        OnLogin += onLogin;
        StartCoroutine(FadeOut());
        if (GameObject.Find("PlayerInfo") == null)
        {
            GameObject p = Instantiate(PlayerInfo, Vector3.zero, Quaternion.identity) as GameObject;
            p.name = p.name.Replace("(Clone)", "");
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void OnDisable()
    {
        OnLogin -= onLogin;
    }
    /// <summary>
    /// 
    /// </summary>
    public void ShowLogin()
    {
        LoginAnim.Play("Login_Show");
        RegisterAnim.Play("Register_Hide");
        UpdateDescription("");
    }

    /// <summary>
    /// 
    /// </summary>
    public void ShowRegister()
    {
        LoginAnim.Play("Login_Hide");
        RegisterAnim.Play("Register_Show");
        UpdateDescription("");
    }
    /// <summary>
    /// Update Text description UI
    /// </summary>
    /// <param name="t"></param>
    public static void UpdateDescription(string t)
    {
        mDescrip.text = t;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="kills"></param>
    /// <param name="deaths"></param>
    /// <param name="score"></param>
    public static void OnLoginEvent(string name, int kills, int deaths, int score)
    {
        if (OnLogin != null)
            OnLogin(name,kills,deaths,score);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="n"></param>
    /// <param name="k"></param>
    /// <param name="d"></param>
    /// <param name="s"></param>
    void onLogin(string n,int k,int d,int s)
    {
        BlackScreen.gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Effect of Fade In
    /// </summary>
    /// <returns></returns>
    IEnumerator FadeIn()
    {
        alpha = BlackScreen.color;

        while (alpha.a < 1.0f)
        {
            alpha.a += Time.deltaTime ;
            BlackScreen.color = alpha;
            yield return null;
        }
    }
    /// <summary>
    /// Effect of Effect Fade Out
    /// </summary>
    /// <returns></returns>
    IEnumerator FadeOut()
    {
        alpha.a = 1f;
        while (alpha.a > 0.0f)
        {
            alpha.a -= Time.deltaTime;
            BlackScreen.color = alpha;
            yield return null;
        }
        BlackScreen.gameObject.SetActive(false);
    }
}