///////////////////////////////////////////////////////////////////////////////////////
// bl_DelaySmooth.cs
//
// Generates a soft effect and delayed rotation for added realism
// place it in the top of the hierarchy of Weapon Manager
//                           
//                                 Lovatto Studio
///////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;

public class bl_DelaySmooth : MonoBehaviour
{
    [Header("TilMovement")]  
    public float maxAmount = 0.03F;
    public float smooth = 3.0F;
    private float amount = 0.02F;
    [Header("FallEffect")]
    [Range(0.01f,1.0f)]
    public float m_time = 0.2f;
    public float m_ReturnSpeed = 5;
    public float SliderAmount = 12;
    public float DownAmount = 13;
    //private
    private Vector3 def;
    private Quaternion DefaultRot;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        def = transform.localPosition;
        DefaultRot = this.transform.localRotation;
    }
    /// <summary>
    /// 
    /// </summary>
    void Update()
    {     
            float factorX = -Input.GetAxis("Mouse X") * amount;
            float factorY = -Input.GetAxis("Mouse Y") * amount;
            factorX = Mathf.Clamp(factorX, -maxAmount, maxAmount);
            factorY = Mathf.Clamp(factorY, -maxAmount, maxAmount);
            Vector3 Final = new Vector3(def.x + factorX, def.y + factorY, def.z);
            transform.localPosition = Vector3.Lerp(transform.localPosition, Final, Time.deltaTime * smooth);
            this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, DefaultRot, Time.deltaTime * m_ReturnSpeed);
        
    }
    /// <summary>
    /// 
    /// </summary>
     void OnEnable()
     {
         bl_EventHandler.OnFall += this.OnFall;
         bl_EventHandler.OnSmallImpact += this.OnSmallImpact;
     }
    /// <summary>
    /// 
    /// </summary>
     void OnDisable()
     {
         bl_EventHandler.OnFall -= this.OnFall;
         bl_EventHandler.OnSmallImpact -= this.OnSmallImpact;
     }
    /// <summary>
    /// On event fall 
    /// </summary>
    /// <param name="t_amount"></param>
     void OnFall(float t_amount)
     {
         StartCoroutine(FallEffect());
     }
    /// <summary>
    /// On Impact event
    /// </summary>
     void OnSmallImpact()
     {
         StartCoroutine(FallEffect());
     }
    /// <summary>
     /// create a soft impact effect
    /// </summary>
    /// <returns></returns>
    public IEnumerator FallEffect()
     {
         Quaternion m_default = this.transform.localRotation;
         Quaternion m_finaly = this.transform.localRotation * Quaternion.Euler(new Vector3(DownAmount,Random.Range(-SliderAmount,SliderAmount),0));
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