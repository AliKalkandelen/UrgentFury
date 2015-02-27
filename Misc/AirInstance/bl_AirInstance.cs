/////////////////////////////////////////////////////////////////////////////////
/////////////////////////bl_AirInstance.cs///////////////////////////////////////
/////////////place this in the prefabs that is instantiated in the air///////////
/////////////////////////////////////////////////////////////////////////////////
////////////////////////////////Briner Games/////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

public class bl_AirInstance : MonoBehaviour {
    /// <summary>
    /// position where the kit arrives
    /// </summary>
    public Vector3 m_Position;
    /// <summary>
    /// speed at which it travels
    /// </summary>
    public float m_speed = 100;
    /// <summary>
    /// Is Recive Info?
    /// </summary>
    private bool InfoRecive = false;
    public GameObject MedKit;
    public GameObject AmmoKit;
    public GameObject ImpactEffect;
    public AudioClip ImpactSound;
    [Space(5)]
    public float m_Radius = 7;//Radius for take shake
    private bool AlredyShake = false;
    private int m_type = 0;

    /// <summary>
    /// receive ordering information
    /// </summary>
    /// <param name="t_pos">Position to Arrive</param>
    /// <param name="t_speed">Kit speed</param>
    /// <param name="type">Kit Type</param>
    public void SepUp(Vector3 t_pos,float t_speed,int type)
    {
        m_Position = t_pos;
        m_speed = t_speed;
        InfoRecive = true;
        m_type = type;
    }

   
	// Update is called once per frame
	void Update () {
        if (!InfoRecive)
            return;

      float t_multipler = 0.0f;
        t_multipler = Mathf.Lerp(t_multipler, m_speed, Time.deltaTime *2.2f);
        transform.localPosition = Vector3.Lerp(transform.localPosition, m_Position, t_multipler);

        if (transform.localPosition == m_Position)
        {
            if (m_type == 0)
            {
                Instantiate(MedKit, transform.position, Quaternion.identity);
            }
            else if (m_type == 1)
            {
                Instantiate(AmmoKit, transform.position, Quaternion.identity);
            }

            if (ImpactEffect)
            {
                Instantiate(ImpactEffect, transform.position, Quaternion.identity);              
            }
            if (ImpactSound)
            {
                AudioSource.PlayClipAtPoint(ImpactSound, transform.position, 1.0f);
            }
            if (!AlredyShake)
            {
                AlredyShake = true;
                Vector3 m_pos = transform.position;

                Collider[] colliders = Physics.OverlapSphere(m_pos, m_Radius);
                foreach (Collider hit in colliders)
                {
                    //Send shake effect for players
                    hit.SendMessageUpwards("SendShake", 3, SendMessageOptions.DontRequireReceiver);
                }
            }
            Destroy(this.gameObject);
        }
	}
}