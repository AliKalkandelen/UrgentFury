using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public class bl_Ragdoll : MonoBehaviour {

    private bl_GameManager m_manager;
    public GameObject KillCamera;
    public bl_KillCam KillCamScript;
    public float DestroyIn = 5;
    public float m_ForceFactor = 1f;
    private Rigidbody[] m_Rigidbodies;
    private Vector3 m_velocity = Vector3.zero;

    void Awake()
    {
        m_manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<bl_GameManager>();
        this.Init();
    }

    protected void Init()
    {
        m_Rigidbodies = this.transform.GetComponentsInChildren<Rigidbody>();
        ChangeRagdoll(true);
    }

    public void ChangeRagdoll(bool m)
    {
        foreach (Rigidbody rigidbody in this.m_Rigidbodies)
        {
            rigidbody.isKinematic = !m;
            if (m)
            {
                rigidbody.AddForce((Time.deltaTime <= 0f) ? Vector3.zero : (((m_velocity / Time.deltaTime) * this.m_ForceFactor)), ForceMode.Impulse);
            }
        }
    }
    public void RespawnAfter(float t_time,string killer)
    {
        KillCamScript.enabled = true;
        KillCamScript.Send_Target(killer);
        StartCoroutine(Wait(t_time));
    }
    public void  isRemote(){
       Destroy(KillCamera);
       Destroy(this.gameObject, DestroyIn);
    }
         
    IEnumerator Wait(float t_time)
    {
        float t = t_time / 3;
        yield return new WaitForSeconds(t * 2);
        StartCoroutine(bl_RoomMenu.FadeIn());
        yield return new WaitForSeconds(t);
        if ((string)PhotonNetwork.player.customProperties["Team"] == Team.Delta.ToString())
        {
            m_manager.SpawnPlayer(Team.Delta);
        }
        else if ((string)PhotonNetwork.player.customProperties["Team"] == Team.Recon.ToString())
        {
            m_manager.SpawnPlayer(Team.Recon);
        }
        else
        {
            m_manager.SpawnPlayer(Team.All);
        }
       
        Destroy(KillCamera);
        Destroy(this);
    }

    public void GetVelocity(Vector3 m_vel)
    {
        m_velocity = m_vel;
    }
}