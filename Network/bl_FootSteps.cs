using UnityEngine;
using System.Collections;

public class bl_FootSteps : MonoBehaviour
{

    public bool CanUpdate = true;
    public AudioSource InstanceReference;
    public AudioClip[] m_dirtSounds;
    public AudioClip[] m_concreteSounds;
    public AudioClip[] m_WoodSounds;
    public float m_minSpeed = 2f;
    public float m_maxSpeed = 8f;
    public float audioStepLengthCrouch = 0.65f;
    public float audioStepLengthWalk = 0.45f;
    public float audioStepLengthRun = 0.25f;
    public float audioVolumeCrouch = 0.3f;
    public float audioVolumeWalk = 0.4f;
    public float audioVolumeRun = 1.0f;
    public PhotonView m_view;
    //private
    private bool isStep = true;
    private string m_MaterialHit;

    // Update is called once per frame
    void Update()
    {
        if (!CanUpdate)//if remote is not updated, only receives the RPC call (optimization helps)
            return;
        if (InstanceReference == null)
            return;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 10))
        {
            m_MaterialHit = hit.collider.transform.tag;
        }
        float m_magnitude = m_charactercontroller.velocity.magnitude;
        if (m_charactercontroller.isGrounded && isStep)
        {
            switch (m_MaterialHit)
            {
                case "Concrete":
                    if (m_magnitude < m_minSpeed && m_magnitude > 0.75f)
                    {
                        m_view.RPC("SyncSteps", PhotonTargets.All, "Crouch", "Concrete");
                    }
                    else if (m_magnitude > m_minSpeed && m_magnitude < m_maxSpeed)
                    {
                        m_view.RPC("SyncSteps", PhotonTargets.All, "Walk", "Concrete");
                    }
                    else if (m_magnitude > m_maxSpeed)
                    {
                        m_view.RPC("SyncSteps", PhotonTargets.All, "Run", "Concrete");
                    }
                    break;
                case "Dirt":
                    if (m_magnitude < m_minSpeed && m_magnitude > 0.75f)
                    {
                        m_view.RPC("SyncSteps", PhotonTargets.All, "Crouch", "Dirt");
                    }
                    else if (m_magnitude > m_minSpeed && m_magnitude < m_maxSpeed)
                    {
                        m_view.RPC("SyncSteps", PhotonTargets.All, "Walk", "Dirt");
                    }
                    else if (m_magnitude > m_maxSpeed)
                    {
                        m_view.RPC("SyncSteps", PhotonTargets.All, "Run", "Dirt");
                    }
                    break;

                case "Wood":
                    if (m_magnitude < m_minSpeed && m_magnitude > 0.75f)
                    {
                        m_view.RPC("SyncSteps", PhotonTargets.All, "Crouch", "Wood");
                    }
                    else if (m_magnitude > m_minSpeed && m_magnitude < m_maxSpeed)
                    {
                        m_view.RPC("SyncSteps", PhotonTargets.All, "Walk", "Wood");
                    }
                    else if (m_magnitude > m_maxSpeed)
                    {
                        m_view.RPC("SyncSteps", PhotonTargets.All, "Run", "Wood");
                    }
                    break;
                default:
                    if (m_magnitude < m_minSpeed && m_magnitude > 0.75f)
                    {
                        m_view.RPC("SyncSteps", PhotonTargets.All, "Crouch", "Concrete");
                    }
                    else if (m_magnitude > m_minSpeed && m_magnitude < m_maxSpeed)
                    {
                        m_view.RPC("SyncSteps", PhotonTargets.All, "Walk", "Concrete");
                    }
                    else if (m_magnitude > m_maxSpeed)
                    {
                        m_view.RPC("SyncSteps", PhotonTargets.All, "Run", "Concrete");
                    }
                    break;
            }            
        }
    }

    IEnumerator Crouch(string m_material)
    {
        if (InstanceReference.audio == null)
            yield return null;

        isStep = false;
        switch (m_material)
        {
            case "Dirt":
                InstanceReference.clip = m_dirtSounds[Random.Range(0, m_dirtSounds.Length)];
                InstanceReference.volume = audioVolumeCrouch;
                InstanceReference.audio.Play();
                break;
            case "Concrete":
                InstanceReference.clip = m_concreteSounds[Random.Range(0, m_concreteSounds.Length)];
                InstanceReference.volume = audioVolumeCrouch;
                InstanceReference.audio.Play();
                break;
            case "Wood":
                InstanceReference.clip = m_WoodSounds[Random.Range(0, m_WoodSounds.Length)];
                InstanceReference.volume = audioVolumeCrouch;
                InstanceReference.audio.Play();
                break;

        }
        yield return new WaitForSeconds(audioStepLengthCrouch);
        isStep = true;
    }



    IEnumerator Walk(string m_material)
    {
        if (InstanceReference.audio == null)
            yield return null;

        isStep = false;
        switch (m_material)
        {
            case "Dirt":
                InstanceReference.clip = m_dirtSounds[Random.Range(0, m_dirtSounds.Length)];
                InstanceReference.volume = audioVolumeWalk;
                InstanceReference.audio.Play();
                break;
            case "Concrete":
                InstanceReference.clip = m_concreteSounds[Random.Range(0, m_concreteSounds.Length)];
                InstanceReference.volume = audioVolumeWalk;
                InstanceReference.audio.Play();
                break;
            case "Wood":
                InstanceReference.clip = m_WoodSounds[Random.Range(0, m_WoodSounds.Length)];
                InstanceReference.volume = audioVolumeWalk;
                InstanceReference.audio.Play();
                break;

        }
        yield return new WaitForSeconds(audioStepLengthWalk);
        isStep = true;
    }

    IEnumerator Run(string m_material)
    {
        if (InstanceReference.audio == null)
           yield return null;

        isStep = false;
        switch (m_material)
        {
            case "Dirt":
        InstanceReference.clip = m_dirtSounds[Random.Range(0, m_dirtSounds.Length)];
        InstanceReference.volume = audioVolumeRun;
        InstanceReference.audio.Play();
                break;
            case "Concrete":
        InstanceReference.clip = m_concreteSounds[Random.Range(0, m_concreteSounds.Length)];
        InstanceReference.volume = audioVolumeRun;
        InstanceReference.audio.Play();
                break;
            case "Wood":
        InstanceReference.clip = m_WoodSounds[Random.Range(0, m_WoodSounds.Length)];
        InstanceReference.volume = audioVolumeRun;
        InstanceReference.audio.Play();
                break;

        }
        
        yield return new WaitForSeconds(audioStepLengthRun);
        isStep = true;
    }

    [RPC]
    void SyncSteps(string t_corrutine,string m_material)
    {
        StartCoroutine(t_corrutine,m_material);
    }

    public void OffUpdate()
    {
        CanUpdate = false;
    }
    public CharacterController m_charactercontroller
    {
        get
        {
            return this.transform.root.GetComponent<CharacterController>();
        }
    }
}


