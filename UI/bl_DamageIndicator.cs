/////////////////////////////////////////////////////////////////////////////////
////////////////////////////bl_DamageIndicator.cs////////////////////////////////
////////////////////Use this to to signal the last attack received///////////////
/////////////////////////////////////////////////////////////////////////////////
////////////////////////////////Briner Games/////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

public class bl_DamageIndicator : MonoBehaviour {
    /// <summary>
    /// Attack from direction
    /// </summary>
    [HideInInspector]
    public Vector3 attackDirection;
    /// <summary>
    /// time reach for fade arrow
    /// </summary>
    public float FadeTime = 3;    
    public Texture damageIndicatorTexture;
    public Color ArrowColor = new Color(0.85f, 0, 0); 
    /// <summary>
    /// the transform root of player 
    /// </summary>
    public Transform target;
    //Private
    private Vector2 pivotPoint;
    private float alpha = 0.0f;
    private float offset = 350f;
    private float rotationOffset;

    /// <summary>
    /// 
    /// </summary>
    void OnGUI()
    {
        if (this.alpha > 0)
        {
            this.offset = Screen.height / 4;
            GUI.color = new Color(ArrowColor.r,ArrowColor.g,ArrowColor.b, this.alpha);
            Vector2 vector = new Vector2(((Screen.width / 2) - (this.damageIndicatorTexture.width / 2)), (Screen.height / 2) - this.offset);
            this.pivotPoint = new Vector2((Screen.width / 2), (Screen.height / 2));
            GUIUtility.RotateAroundPivot(this.rotationOffset, this.pivotPoint);
            GUI.DrawTexture(new Rect(vector.x, vector.y, this.damageIndicatorTexture.width,this.damageIndicatorTexture.height), this.damageIndicatorTexture);
        }
    }
    /// <summary>
    /// Use this to send a new direction of attack
    /// </summary>
    /// <param name="dir">postion of attacker</param>
    public void AttackFrom(Vector3 dir)
    {
        this.attackDirection = dir;
        this.alpha = 3f;
    }
    /// <summary>
    /// if this is visible Update position
    /// </summary>
    void Update()
    {
        if (this.alpha > 0)
        {
            this.alpha -= Time.deltaTime;
            this.UpdateDirection();
        }
    }
    /// <summary>
    /// update direction as the arrow shows
    /// </summary>
    void UpdateDirection()
    {
        Vector3 rhs = this.attackDirection - this.target.position;
        rhs.y = 0;
        rhs.Normalize();
        Vector3 forward;
        if (Camera.main != null)
        {
            forward = Camera.main.transform.forward;
        }
        else
        {
            if (Camera.current != null)
            {
                forward = Camera.current.transform.forward;
            }
            else
            {
                forward = this.transform.forward;
            }
        }    
        float GetPos = Vector3.Dot(forward, rhs);
        if (Vector3.Cross(forward, rhs).y > 0)
        {
            this.rotationOffset = (1f - GetPos) * 90;
        }
        else
        {
            this.rotationOffset = (1f - GetPos) * -90;
        }
    }
}
