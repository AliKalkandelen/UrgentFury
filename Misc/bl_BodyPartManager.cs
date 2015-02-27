//////////////////////////////////////////////////////////////////
// bl_BodyPartManager.cs
//
// This script helps us manage our remote player hitboxes
// mind just place it in the root of the remote player
// and executes the last two options of the "ContextMenu" component. 
//                       Lovatto Studio
//////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class bl_BodyPartManager : MonoBehaviour {

    [System.Serializable]
    public class Part
    {
        public string name;
        public Collider m_Collider;
        public float m_Multipler = 1.0f;
        public bool m_HeatShot = false;
    }
    /// <summary>
    /// Change the tag if you use other
    /// </summary>
    public string HitBoxTag = "BodyPart";
    /// <summary>
    /// List of information for hitboxes
    /// </summary>
    public List<Part> HitBoxs = new List<Part>();
    public List<Rigidbody> mRigidBody = new List<Rigidbody>();
    [Space(5)]
    public bl_UpperAnimations UpperAnimation;
    public bl_PlayerAnimations PlayerAnimation;
    public bl_OnVisible OnVisible;
    public Animation mAnimation;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        GetRigidBodys();
        if (mRigidBody.Count > 0)
        {
            SetKinematic();
        }
    }

    #if UNITY_EDITOR
    [ContextMenu("Get HitBoxes")]
     #endif
    void GetAllCollider()
    {
        HitBoxs.Clear();
        Collider[] mCol = transform.GetComponentsInChildren<Collider>();
        if (mCol.Length > 0)
        {
            foreach (Collider c in mCol)
            {
                if (c.gameObject.tag != HitBoxTag)
                {
                    c.gameObject.tag = HitBoxTag;
                }
                Part p = new Part();
                p.m_Collider = c;
                p.name = c.name;
                HitBoxs.Add(p);
            }
        }
        else
        {
            Debug.LogError("This transform no have colliders in childrens");
        }
    }

    #if UNITY_EDITOR
    [ContextMenu("Add Script")]
    #endif
    void AddScript()
    {
        if (HitBoxs.Count > 0)
        {
            foreach (Part p in HitBoxs)
            {
                //DestroyImmediate(p.m_Collider.gameObject.GetComponent<bl_BodyPart>()); //use for remove script
                if (p.m_Collider != null || p.m_Collider.gameObject.GetComponent<bl_BodyPart>() == null)
                {

                    p.m_Collider.gameObject.AddComponent<bl_BodyPart>();
                    bl_BodyPart bp = p.m_Collider.gameObject.GetComponent<bl_BodyPart>();
                    bp.TakeHeatShot = p.m_HeatShot;
                    bp.multiplier = p.m_Multipler;
                    bp.HealtScript = this.transform.root.GetComponent<bl_PlayerDamageManager>();
                }
            }
        }
        else
        {
            Debug.LogError("Hitbox List is emty, get hitbox before");
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Get RigidBodys")]
#endif
    void GetRigidBodys()
    {
        Rigidbody[] R = this.transform.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rb in R)
        {
            if (!mRigidBody.Contains(rb))
            {
                mRigidBody.Add(rb);
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="b">is Kinematic?</param>
    public void SetKinematic(bool b = true)
    {
        if (mRigidBody == null || mRigidBody.Count <= 0)
            return;

        foreach (Rigidbody r in mRigidBody)
        {
            r.isKinematic = b;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public void Ragdolled()
    {
        this.transform.parent = null;
        if (OnVisible != null)
        {
            Destroy(OnVisible);
        }
        mAnimation.enabled = false;
        foreach (Rigidbody r in mRigidBody)
        {
            r.isKinematic = false;
            r.useGravity = true;
            r.velocity = PlayerAnimation.velocity;
        }
        UpperAnimation.enabled = false;
        PlayerAnimation.enabled = false;
        Destroy(this.gameObject, 10);
    }
}