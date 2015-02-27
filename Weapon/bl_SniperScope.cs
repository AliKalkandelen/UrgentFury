using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class bl_SniperScope : MonoBehaviour {

    public GUISkin Skin;
    public Texture2D Scope;
    /// <summary>
    /// Object to desactivate when is aimed
    /// </summary>
    public List<GameObject> OnScopeDisable = new List<GameObject>();
    public int m_depth = 200;
    public bool m_show_distance = true;
    /// <summary>
    /// maximum distance raycast
    /// </summary>
    public float Max_Distance = 1000;

    public float m_SmoothAppear = 12;
	public bool aimbool = true;


    //private
    private bl_Gun m_gun;
    private float m_alpha = 0;
    private Vector3 m_point = Vector3.zero;
    private float m_dist = 0.0f;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        m_gun = this.GetComponent<bl_Gun>();
    }

	void Update(){
		GameObject look = GameObject.FindGameObjectWithTag ("Player");
		GameObject look2 = GameObject.Find("Mouse");
		bl_MouseLook sensx = look.GetComponent<bl_MouseLook> ();
		bl_MouseLook sensy = look2.GetComponent<bl_MouseLook> ();


		if (m_gun.isAmed && aimbool) {
//			sensx.sX /= 3;
//			sensx.sY /= 3;
//			sensy.sX /= 3;
//			sensy.sY /= 3;
			PlayerPrefs.SetFloat("sensitive", sensx.sX /2);
			aimbool = false;
		}
		else if(!m_gun.isAmed && !aimbool){
//			sensx.sX *= 3;
//			sensx.sY *= 3;
//			sensy.sX *= 3;
//			sensy.sY *= 3;
			PlayerPrefs.SetFloat("sensitive", sensx.sX *2);
			aimbool = true;
		}



	}
    /// <summary>
    /// 
    /// </summary>
	void OnGUI() {

        if (Scope == null)
            return;

        GUI.depth = m_depth;
        if (m_gun.isAmed)
		{


            if (m_show_distance)
            {
                GetDistance();
            }
            //add a little fade in to avoid the impact of appearing once
            m_alpha = Mathf.Lerp(m_alpha, 1.0f, Time.deltaTime * m_SmoothAppear);
            foreach (GameObject go in OnScopeDisable)
            {
                go.SetActive(false);
            }
            GUI.color = new Color(1, 1, 1, m_alpha);
            GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),Scope);
            if (m_show_distance)
            {
                GUI.Label(new Rect(Screen.width / 2 + 25, Screen.height / 2 + 25, 100, 30), m_dist.ToString("0.0")+"<size=12>m</size>",style);
            }

        }
        else
        {

            m_alpha = Mathf.Lerp(m_alpha, 0.0f, Time.deltaTime * m_SmoothAppear);
            foreach (GameObject go in OnScopeDisable)
            {
                go.SetActive(true);
            }
        }

	}
    /// <summary>
    /// calculate the distance to the first object that raycast hits
    /// </summary>
    void GetDistance()
    {
        RaycastHit m_ray;
        Vector3 fwd = Camera.main.transform.forward;
        if (Physics.Raycast(Camera.main.transform.position, fwd, out m_ray, Max_Distance))
        {
            m_point = m_ray.point;
            m_dist = bl_UtilityHelper.GetDistance(m_point, Camera.main.transform.position);
        }
        else
        {
            m_dist = 0.0f;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public GUIStyle style
    {
        get
        {
            GUIStyle style = new GUIStyle();
            if (Skin)
            {
                style.font = Skin.font;
            }
            style.normal.textColor = Color.white;
            style.richText = true;
            return style;
        }
    }
}
