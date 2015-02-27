using UnityEngine;
using System.Collections;

public class bl_UpperAnimations : MonoBehaviour {
    [HideInInspector]
    public bool m_Update = true;

    public Transform ShoulderR;
    public Transform ShouldeL;
    public Transform HeadBone;
    public bool TakeHeadInAnim = true;
    public string m_state;
    public string FireAnimation = "StandingFire";
    public string FirePistolAnim = "StandingPistolFire";
    public string FireKnifeAnim = "KnifeFire";
    [Space(5)]
    public string ReloadAnimation = "StandingReloadM4";
    [Space(5)]
    public string AimedAnimation = "StandingAim";
    public string IdleAnimation = "Idle";
    public string IdlePistolAnim = "StandingPistol";
    public string IdleKnifeAnim = "KnifeIdle";
    public string RunningAnim = "Standing";

    public bl_Gun.weaponType m_WeaponType = bl_Gun.weaponType.Machinegun;
    /// <summary>
    /// 
    /// </summary>
    void OnEnable()
    {
        SetupAnimations();
    }
    /// <summary>
    /// When add a new animation configure same how this
    /// </summary>
    void SetupAnimations()
    {
        animation[FireAnimation].AddMixingTransform(ShoulderR);
        animation[FireAnimation].AddMixingTransform(ShouldeL);
        if (TakeHeadInAnim)
        {
            animation[FireAnimation].AddMixingTransform(HeadBone);
        }
        animation[FireAnimation].wrapMode = WrapMode.Loop;
        animation[FireAnimation].layer = 3;
        animation[FireAnimation].time = 0;
        animation[FireAnimation].speed = 1.1f;
        animation[FireAnimation].weight = 0.2f;
        //
        animation[ReloadAnimation].AddMixingTransform(ShoulderR);
        animation[ReloadAnimation].AddMixingTransform(ShouldeL);
        if (TakeHeadInAnim)
        {
            animation[ReloadAnimation].AddMixingTransform(HeadBone);
        }
        animation[ReloadAnimation].wrapMode = WrapMode.Once;
        animation[ReloadAnimation].layer = 3;
        animation[ReloadAnimation].time = 0;
        animation[ReloadAnimation].speed = 0.9f;
        animation[ReloadAnimation].weight = 0.2f;
        //
        animation[AimedAnimation].AddMixingTransform(ShoulderR);
        animation[AimedAnimation].AddMixingTransform(ShouldeL);
        if (TakeHeadInAnim)
        {
            animation[AimedAnimation].AddMixingTransform(HeadBone);
        }
        animation[AimedAnimation].wrapMode = WrapMode.Loop;
        animation[AimedAnimation].layer = 3;
        animation[AimedAnimation].time = 0;
        animation[AimedAnimation].speed = 1.0f;
        animation[AimedAnimation].weight = 0.2f;
        //
        animation[IdleAnimation].AddMixingTransform(ShoulderR);
        animation[IdleAnimation].AddMixingTransform(ShouldeL);
        if (TakeHeadInAnim)
        {
            animation[IdleAnimation].AddMixingTransform(HeadBone);
        }
        animation[IdleAnimation].wrapMode = WrapMode.Loop;
        animation[IdleAnimation].layer = 3;
        animation[IdleAnimation].time = 0;
        animation[IdleAnimation].speed = 1.0f;
        animation[IdleAnimation].weight = 0.2f;
        //
        animation[FirePistolAnim].AddMixingTransform(ShoulderR);
        animation[FirePistolAnim].AddMixingTransform(ShouldeL);
        if (TakeHeadInAnim)
        {
            animation[FirePistolAnim].AddMixingTransform(HeadBone);
        }
        animation[FirePistolAnim].wrapMode = WrapMode.Loop;
        animation[FirePistolAnim].layer = 3;
        animation[FirePistolAnim].time = 0;
        animation[FirePistolAnim].speed = 1.0f;
        animation[FirePistolAnim].weight = 0.2f;
        //
        animation[FireKnifeAnim].AddMixingTransform(ShoulderR);
        animation[FireKnifeAnim].AddMixingTransform(ShouldeL);
        if (TakeHeadInAnim)
        {
            animation[FireKnifeAnim].AddMixingTransform(HeadBone);
        }
        animation[FireKnifeAnim].wrapMode = WrapMode.Once;
        animation[FireKnifeAnim].layer = 3;
        animation[FireKnifeAnim].time = 0;
        animation[FireKnifeAnim].speed = 1.0f;
        animation[FireKnifeAnim].weight = 0.2f;
        //
        animation[IdlePistolAnim].AddMixingTransform(ShoulderR);
        animation[IdlePistolAnim].AddMixingTransform(ShouldeL);
        if (TakeHeadInAnim)
        {
            animation[IdlePistolAnim].AddMixingTransform(HeadBone);
        }
        animation[IdlePistolAnim].wrapMode = WrapMode.Loop;
        animation[IdlePistolAnim].layer = 3;
        animation[IdlePistolAnim].time = 0;
        animation[IdlePistolAnim].speed = 1.0f;
        animation[IdlePistolAnim].weight = 0.2f;
        //
        animation[IdleKnifeAnim].AddMixingTransform(ShoulderR);
        animation[IdleKnifeAnim].AddMixingTransform(ShouldeL);
        if (TakeHeadInAnim)
        {
            animation[IdleKnifeAnim].AddMixingTransform(HeadBone);
        }
        animation[IdleKnifeAnim].wrapMode = WrapMode.Loop;
        animation[IdleKnifeAnim].layer = 3;
        animation[IdleKnifeAnim].time = 0;
        animation[IdleKnifeAnim].speed = 1.0f;
        animation[IdleKnifeAnim].weight = 0.2f;
        //
        animation[RunningAnim].AddMixingTransform(ShoulderR);
        animation[RunningAnim].AddMixingTransform(ShouldeL);
        if (TakeHeadInAnim)
        {
            animation[RunningAnim].AddMixingTransform(HeadBone);
        }
        animation[RunningAnim].wrapMode = WrapMode.Loop;
        animation[RunningAnim].layer = 3;
        animation[RunningAnim].time = 0;
        animation[RunningAnim].speed = 1.0f;
        animation[RunningAnim].weight = 0.2f;
    }
    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (!m_Update)
            return;

        if (m_state == "Firing")
        {
            if (m_WeaponType == bl_Gun.weaponType.Machinegun)
            {
                animation.CrossFade(FireAnimation, 0.2f);
            }
            else if (m_WeaponType == bl_Gun.weaponType.Pistol)
            {
                animation.CrossFade(FirePistolAnim, 0.2f);
            }
            else if (m_WeaponType == bl_Gun.weaponType.Launcher)
            {
                animation.CrossFade(FireKnifeAnim, 0.2f);
            }
            else//When you have more animation per example launcher Fire add "else if(m_WeaponType == bl_Gun.weaponType.Launcher o wherever"
            {
                animation.CrossFade(FireAnimation, 0.2f);
            }
        }
        else if (m_state == "Reloading" && m_WeaponType != bl_Gun.weaponType.Launcher)
        {
            animation.CrossFade(ReloadAnimation, 0.2f);
        }
        else if (m_state == "Aimed" && m_WeaponType != bl_Gun.weaponType.Launcher)
        {
            animation.CrossFade(AimedAnimation, 0.2f);
        }
        else if (m_state == "Running")
        {
            animation.CrossFade(RunningAnim, 0.2f);
        }
        else//if idle
        {
            if (m_WeaponType == bl_Gun.weaponType.Machinegun)
            {
                animation.CrossFade(IdleAnimation, 0.2f);
            }
            else if (m_WeaponType == bl_Gun.weaponType.Pistol)
            {
                animation.CrossFade(IdlePistolAnim, 0.2f);
            }
            else if (m_WeaponType == bl_Gun.weaponType.Launcher)
            {
                animation.CrossFade(IdleKnifeAnim, 0.2f);
            }
            else//When you have more animation per example launcher Idle add "else if(m_WeaponType == bl_Gun.weaponType.Launcher o wherever"
            {
                animation.CrossFade(IdleAnimation, 0.2f);
            }

        }
    }
}
