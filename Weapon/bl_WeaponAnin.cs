using UnityEngine;
using System.Collections;

public class bl_WeaponAnin : MonoBehaviour {

public string DrawName = "Draw";
public string TakeOut = "TakeOut";
public string FireName = "Fire";
public string ReloadName = "Reload";
public float FireSpeed = 1.0f;
public float AimFireSpeed = 1.0f;
public float DrawSpeed = 1.0f;
public float HideSpeed = 1.0f;
[Space(5)]
[Header("ShotGun/Sniper")]
public string StartReloadAnim = "StartReload";
public string InsertAnim = "Insert";
public string AfterReloadAnim = "AfterReload";
[Space(5)]
[Header("Audio")]
public AudioSource m_source;
public AudioClip Reload_1;
public AudioClip Reload_2;
public AudioClip Reload_3;
[Space(5)]
public AudioClip m_Fire;
//private
private int m_repeatReload;

void Awake()
{
    animation.wrapMode = WrapMode.Once;
}
    /// <summary>
    /// 
    /// </summary>
public void  Fire (){
    animation.Rewind(FireName);
    animation[FireName].speed = FireSpeed;
    animation.Play(FireName);
}
    /// <summary>
    /// 
    /// </summary>
public void AimFire()
{
    animation.Rewind(FireName);
    animation[FireName].speed = AimFireSpeed;
    animation.Play(FireName);
}
    /// <summary>
    /// 
    /// </summary>
public void  DrawWeapon (){
    animation.Rewind(DrawName);
    animation[DrawName].speed = DrawSpeed;
    animation[DrawName].time = 0;
    animation.Play(DrawName);
}
/// <summary>
///  
/// </summary>
public void HideWeapon()
{
    animation.Rewind(TakeOut);
    animation[TakeOut].speed = HideSpeed;
    animation[TakeOut].time = 0;
    animation.Play(TakeOut);
}
    /// <summary>
/// event called by animation when is a reload state
    /// </summary>
    /// <param name="ReloadTime"></param>
public void Reload(float ReloadTime)
{
    animation.Stop(ReloadName);
    animation[ReloadName].speed = (animation[ReloadName].clip.length / ReloadTime);
    animation.Rewind(ReloadName);
    animation.Play(ReloadName);
}
    /// <summary>
    /// event called by animation when is fire
    /// </summary>
public void FireAudio()
{
    if (m_source != null && m_Fire != null)
    {
        m_source.clip = m_Fire;
        m_source.pitch = Random.Range(1, 1.5f);
        m_source.Play();
    }
}
public void ReloadRepeat(float m_reloadTime, int m_repeat)
{
    float TotalTime = animation[StartReloadAnim].clip.length + (animation[InsertAnim].clip.length * m_repeat) + animation[AfterReloadAnim].clip.length;

    AnimationState firtsState = animation.CrossFadeQueued(StartReloadAnim);
    firtsState.speed = (TotalTime / m_reloadTime) / 1.4f;

    for (int i = 0; i < m_repeat; i++)
    {
        AnimationState newReload2 = animation.CrossFadeQueued(InsertAnim);
        newReload2.speed = (TotalTime / m_reloadTime)/1.4f;
    }
    AnimationState newReload3 = animation.CrossFadeQueued(AfterReloadAnim);
    if (m_repeat > 1)
    {
        newReload3.speed = (TotalTime / m_reloadTime) / 1.4f;
    }
    else
    {
        newReload3.speed = (TotalTime / m_reloadTime) / 2;
    }
}
    /// <summary>
/// Use this for greater coordination
/// reload sounds with animation
    /// </summary>
    /// <param name="index">st</param>
public void ReloadSound(int index)
{
    if (m_source == null)
        return;

    switch (index)
    {
        case 0:
            m_source.clip = Reload_1;
            m_source.Play();
            if (GManager != null)
            {
                GManager.heatReloadAnim(1);
            }
        break;
        case 1:
            m_source.clip = Reload_2;
            m_source.Play();
        break;
        case 2:
        if (Reload_3 != null)
        {
            m_source.clip = Reload_3;
            m_source.Play();
        }
            if (GManager != null)
            {
                GManager.heatReloadAnim(2);
                StartCoroutine(ReturnToIdle());
            }
        break;
    }
}
    /// <summary>
    /// Heat animation
    /// </summary>
    /// <returns></returns>
IEnumerator ReturnToIdle()
{
    yield return new WaitForSeconds(0.6f);
    GManager.heatReloadAnim(0);
}
    /// <summary>
    /// 
    /// </summary>
private bl_GunManager GManager
{
    get
    {
        return this.transform.root.GetComponentInChildren<bl_GunManager>();
    }
}
}
