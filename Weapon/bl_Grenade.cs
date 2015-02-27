using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class bl_Grenade : MonoBehaviour
{
    [HideInInspector]
    public int ID;
    [HideInInspector]
    public string mName;
    [HideInInspector]
    public bool isNetwork = false;
    public bool OnHit = false;
    public int TimeToExploit = 8;
    public GameObject explosion;   // instanced explosion
    private float damage;             // damage bullet applies to a target
    public float maxInaccuracy = 2.0f;      // maximum amount of inaccuracy
    public float variableInaccuracy = 0.2f; // used in machineguns to decrease accuracy if maintaining fire
    [Header("Audio")]
    public AudioClip BeepAudio;
    [Range(0.1f, 1)]
    public float m_Volume = 1.0f;

    //Private
    private float beepTimer = float.PositiveInfinity;
    private float startTime = 0;
    private float speed = 75.0f;              // bullet speed
    private Vector3 velocity = Vector3.zero; // bullet velocity    
    private Vector3 direction;               // direction bullet is travelling
    private float impactForce;        // force applied to a rigid body object
    private float fuseTime = 10f;
     
    /// <summary>
    /// 
    /// </summary>
    /// <param name="s"></param>
    public void SetUp(bl_BulletInitSettings s)
    {
        damage = s.m_damage;
        impactForce = s.m_ImpactForce;
        maxInaccuracy = s.m_maxspread;
        variableInaccuracy = s.m_speed;
        speed = s.m_speed;
        ID = s.m_weapinID;
        mName = s.m_weaponname;
        isNetwork = s.isNetwork;
        //direction = transform.TransformDirection(0, 0, 1);
        direction = transform.TransformDirection(Random.Range(-maxInaccuracy, maxInaccuracy) * variableInaccuracy, Random.Range(-maxInaccuracy, maxInaccuracy) * variableInaccuracy, 1);

        velocity = speed * transform.forward;

        rigidbody.velocity = velocity + direction;
        InvokeRepeating("Counter", 1, 1);
    }
    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        fuseTime = TimeToExploit;
        startTime = Time.time;
        audio.clip = BeepAudio;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="enterObject"></param>
    void OnCollisionEnter(Collision enterObject)
    {
        // things to add:
        // maybe a distance or time check to see if grenade is far enough away to arm before exploding
        // ... maybe a non armed grenade will bounce then explode
        // similar to direct noob tube shots in CoD
        if (!OnHit)
            return;
        switch (enterObject.transform.tag)
        {
            case "Projectile":
                //return;                
                break;            
            default:
                Destroy(gameObject, 0);//GetComponent<Rigidbody>().useGravity = false;
                ContactPoint contact = enterObject.contacts[0];
                Quaternion rotation = gameObject.rigidbody.rotation;  //Quaternion.FromToRotation(Vector3.up, contact.normal); 

                GameObject e = Object.Instantiate(explosion, contact.point, rotation) as GameObject;
                bl_Blast blast = e.GetComponent<bl_Blast>();
                if (blast != null)
                {
                    blast.isNetwork = isNetwork;
                    blast.WeaponID = ID;
                    blast.WeaponName = mName;
                    blast.explosionDamage = damage;
                }
                if (enterObject.rigidbody)
                {
                    enterObject.rigidbody.AddForce(transform.forward * impactForce, ForceMode.Impulse);
                }
                break;
        }
        
    }
    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (BeepAudio == null || audio == null)
            return;

        if (this.transform.childCount != 0)
        {
            float div = this.fuseTime - (Time.time - this.startTime);
            float c = div * 1f;
            if (this.beepTimer >= c)
            {
                base.audio.Play();
                this.beepTimer = 0f;
            }
            this.beepTimer += Time.deltaTime;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void Counter()
    {
        TimeToExploit--;

        if (TimeToExploit <= 0)
        {
            GameObject e = Object.Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
            bl_Blast blast = e.GetComponent<bl_Blast>();
            if (blast != null)
            {
                blast.isNetwork = isNetwork;
                blast.WeaponID = ID;
                blast.WeaponName = mName;
                blast.explosionDamage = damage;
            }
            CancelInvoke("Counter");
            Destroy(gameObject);
        }
    }
}