using UnityEngine;
using System.Collections;

public class bl_TilHead : MonoBehaviour
{
    [Header("TillEffect")]
    private Transform m_transform;
    public float smooth = 4f;
    public float tiltAngle = 6f;
    [Header("FallEffect")]
    [Range(0.01f, 1.0f)]
    public float m_time = 0.2f;
    public float DownAmount = 8;

     void Awake()
    {
        this.m_transform = this.transform;
    }
     void OnEnable()
     {
         bl_EventHandler.OnSmallImpact += this.OnSmallImpact;
     }
     void OnDisable()
     {
         bl_EventHandler.OnSmallImpact -= this.OnSmallImpact;
     }

    void Update()
    {
        if (Screen.lockCursor )
        {
            float t_amount = -Input.GetAxis("Mouse X") * this.tiltAngle;
            t_amount = Mathf.Clamp(t_amount, -this.tiltAngle, this.tiltAngle);
            if (!Input.GetMouseButton(1))
            {
                this.m_transform.localRotation = Quaternion.Lerp(this.m_transform.localRotation, Quaternion.Euler(0, 0, t_amount), Time.deltaTime * this.smooth);
            }
            else
            {
                this.m_transform.localRotation = Quaternion.Lerp(this.m_transform.localRotation, Quaternion.Euler(0, 0, t_amount / 2), Time.deltaTime * this.smooth);
            }
        }
    }

    void OnSmallImpact()
    {
        StartCoroutine(FallEffect());
    }

    IEnumerator FallEffect()
    {
        Quaternion m_default = this.transform.localRotation;
        Quaternion m_finaly = this.transform.localRotation * Quaternion.Euler(new Vector3(DownAmount, 0, 0));
        float t_rate = 1.0f / m_time;
        float t_time = 0.0f;
        while (t_time < 1.0f)
        {
            t_time += Time.deltaTime * t_rate;
            this.transform.localRotation = Quaternion.Slerp(m_default, m_finaly, t_time);
            yield return t_rate;
        }
    }
}

