using UnityEngine;
using System.Collections;

public class bl_CallEventKit : MonoBehaviour {

    public GUISkin Skin;
    public float WaitForCall = 3;
    public bool ShowUI = true;
    public string m_text = "Kit";
    private bool CallNow = false;
    public KitType m_type = KitType.Medit;
	// Use this for initialization
	void Start () {
        if (WaitForCall <= 0)
        {
            CallNow = true;
        }
        else
        {
            StartCoroutine(WaitCall(WaitForCall));
        }
	}
	
	// Update is called once per frame
	void FixedUpdate(){
        if (!CallNow)
            return;
        CallNow = false;
        if (m_type == KitType.Medit)
        {
            bl_EventHandler.KitAirEvent(this.transform.position,0);//ID Medkit is 0
        }
        if (m_type == KitType.Ammo)
        {
            bl_EventHandler.KitAirEvent(this.transform.position, 1);//ID AmmoKit is 1
        }
        Destroy(this.gameObject, 0.5f);
	}
    void OnGUI()
    {
        if (!ShowUI)
            return;
        if (Camera.main == null)
            return;
        GUI.skin = Skin;

        Vector3 m_vector = Camera.main.WorldToScreenPoint(this.transform.position);
        if (m_vector.z > 0)
        {
            GUI.Box(new Rect(m_vector.x + 7, (Screen.height - m_vector.y) - 7, 100, 30),m_text);
        }
    }

    IEnumerator WaitCall(float t_time)
    {
        yield return new WaitForSeconds(t_time);
        CallNow = true;
    }
    public enum KitType
    {
        Medit,
        Ammo
    }
}
