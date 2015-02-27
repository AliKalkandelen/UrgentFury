using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class bl_Bullet : MonoBehaviour
{
    #region Variables
    [HideInInspector]
    public Vector3 DirectionFrom = Vector3.zero;
    [HideInInspector]
    public bool isNetwork = false;
    private int hitCount = 0;         // hit counter for counting bullet impacts for bullet penetration
    private int OwnGunID = 777;  //Information for contain Gun id
    private float damage;             // damage bullet applies to a target
    private float maxHits;            // number of collisions before bullet gets destroyed
    private float impactForce;        // force applied to a rigid body object
    private float maxInaccuracy;      // maximum amount of inaccuracy
    private float variableInaccuracy; // used in machineguns to decrease accuracy if maintaining fire
    private float speed;              // bullet speed
    private float lifetime = 1.5f;    // time till bullet is destroyed
    [HideInInspector]
    public string GunName = ""; //Weapon name
    private Vector3 velocity = Vector3.zero; // bullet velocity
    private Vector3 newPos = Vector3.zero;   // bullet's new position
    private Vector3 oldPos = Vector3.zero;   // bullet's previous location
    private bool hasHit = false;             // has the bullet hit something?
    private Vector3 direction;               // direction bullet is travelling
    public bool isTracer = false;          // used in raycast bullet system... sets the bullet to just act like a tracer
    [Space(5)]
    public List<AudioClip> mHitsSounds = new List<AudioClip>();
    public Vector2 mPicht = new Vector2(1.0f, 1.5f);

    [HideInInspector]
    public enum HitType
    {
        CONCRETE,
        WOOD,
        METAL,
        OLD_METAL,
        GLASS,
        GENERIC
    };

    HitType type;

    // bullet hole game object
    public GameObject bulletHoleObject;

    //textures for different materials
    public Texture2D[] concrete; 
    public Texture2D[] wood;
    public Texture2D[] metal;
    public Texture2D[] oldMetal;
    public Texture2D[] glass;
    public Texture2D[] generic;

    //impact effects for materials
    public GameObject woodParticle;
	public GameObject metalParticle;
	public GameObject concreteParticle;
	public GameObject sandParticle;
	public GameObject waterParticle;
    public GameObject genericParticle;
    public GameObject bloodParticle;

    #endregion // end of variables

    #region Bullet Set Up

    public void SetUp(bl_BulletInitSettings info) // information sent from gun to bullet to change bullet properties
    {
        damage = info.m_damage;              // bullet damage
        impactForce = info.m_ImpactForce;         // force applied to rigid bodies
        maxHits = info.m_MaxPenetration;             // max number of bullet impacts before bullet is destroyed
        maxInaccuracy = info.m_maxspread;       // max inaccuracy of the bullet
        variableInaccuracy = info.m_spread;  // current inaccuracy... mostly for machine guns that lose accuracy over time
        speed = info.m_speed;               // bullet speed
        DirectionFrom = info.m_position;
        GunName = info.m_weaponname;
        OwnGunID = info.m_weapinID;
        isNetwork = info.isNetwork;
        // drection bullet is traveling
        direction = transform.TransformDirection(Random.Range(-maxInaccuracy, maxInaccuracy) * variableInaccuracy, Random.Range(-maxInaccuracy, maxInaccuracy) * variableInaccuracy, 1);

        newPos = transform.position;   // bullet's new position
        oldPos = newPos;               // bullet's old position
        velocity = speed * transform.forward; // bullet's velocity determined by direction and bullet speed

        // schedule for destruction if bullet never hits anything
        Destroy(gameObject, lifetime);
    }

    #endregion

    #region Create Bullet Holes
        
    public void MakeBulletHole(RaycastHit hit, GameObject go)
    {
        Texture2D useTexture;       
        int random;
        Vector3 contact = hit.point; // point where bullet hit
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal); // rotation of bullet impact

        switch (type)
        {
            case HitType.CONCRETE:
                if (concrete == null) return;
                if (concrete.Length == 0) return;

                random = Random.Range(0, concrete.Length);

                useTexture = concrete[random];
                break;
            case HitType.WOOD:
                if (wood == null) return;
                if (wood.Length == 0) return;

                random = Random.Range(0, wood.Length);

                useTexture = wood[random];
                break;
            case HitType.METAL:
                if (metal == null) return;
                if (metal.Length == 0) return;

                random = Random.Range(0, metal.Length);

                useTexture = metal[random];
                break;
            case HitType.OLD_METAL:
                if (oldMetal == null) return;
                if (oldMetal.Length == 0) return;

                random = Random.Range(0, oldMetal.Length);

                useTexture = oldMetal[random];
                break;
            case HitType.GLASS:
                if (glass == null) return;
                if (glass.Length == 0) return;

                random = Random.Range(0, glass.Length);

                useTexture = glass[random];
                break;
            case HitType.GENERIC:
                if (generic == null) return;
                if (generic.Length == 0) return;

                random = Random.Range(0, generic.Length);

                useTexture = generic[random];
                break;
            default:
                if (wood == null) return;
                if (wood.Length == 0) return;

                random = Random.Range(0, wood.Length);

                useTexture = wood[random];
                return;
        }

        

        GameObject newBulletHole = Instantiate(bulletHoleObject, contact, rotation) as GameObject;
        newBulletHole.renderer.material.SetTexture("_MainTex", useTexture);

        //newBulletHole.GetComponent<Decal>().decalMode = DecalMode.MESH_COLLIDER;
        //newBulletHole.GetComponent<Decal>().decalMaterial.mainTexture = useTexture;
        //newBulletHole.GetComponent<Decal>().CalculateDecal();
        newBulletHole.transform.parent = go.transform;

        /*
        Decal d = (Decal)gameObject.GetComponent("Decal");
        d.affectedObjects = new GameObject[1];
        d.affectedObjects[0] = go;
        d.decalMode = DecalMode.MESH_COLLIDER;

        Material m = new Material(d.decalMaterial);
		m.mainTexture = useTexture;
		d.decalMaterial = m;
        d.CalculateDecal();
        d.transform.parent = go.transform;
        */
    }

    #endregion // end of bullet hole creation / info


    void Update()
    {
        if (hasHit)
            return; // if bullet has already hit its max hits... exit

        // assume we move all the way
        newPos += (velocity + direction) * Time.deltaTime;

        // Check if we hit anything on the way
        Vector3 dir = newPos - oldPos;
        float dist = dir.magnitude;

        if (dist > 0)
        {
            // normalize
            dir /= dist;

            RaycastHit[] hits = Physics.RaycastAll(oldPos, dir, dist);

            Debug.DrawLine(oldPos, newPos, Color.red);
            // Find the first valid hit
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                
                newPos = hit.point;
                
                OnHit(hit);
                
                if (hitCount >= maxHits)
                {
                    hasHit = true;
                    Destroy(gameObject);                    
                }
            }
        }

        RaycastHit hit2;
        if (Physics.Raycast(newPos, -dir, out hit2, dist)) // send a ray behind the bullet to check for exit impact
        {   
            if ((!hasHit))// && (hit2.transform != owner.transform))
            {
                OnBackHit(hit2); // send rear impact and check what to do with it
            }
        }

        oldPos = transform.position;  // set old position to current position
        transform.position = newPos;  // set current position to the new position
    }

    #region Bullet On Hits

    void OnHit(RaycastHit hit)
    {        
        GameObject go = null;
        Ray mRay = new Ray(transform.position, transform.forward);
        if (!isTracer)  // if this is a bullet and not a tracer, then apply damage to the hit object
        {
            if (hit.rigidbody != null && !hit.rigidbody.isKinematic) // if we hit a rigi body... apply a force
            {
                float mAdjust = 1.0f / (Time.timeScale * (0.02f / Time.fixedDeltaTime));
                hit.rigidbody.AddForceAtPosition(((mRay.direction * impactForce) / Time.timeScale) / mAdjust, hit.point);
            }
        }
        switch (hit.transform.tag) // decide what the bullet collided with and what to do with it
        {
            case "Projectile":
                // do nothing if 2 bullets collide
                break;
            case "BodyPart"://Send Damage for other players
                if (hit.transform.GetComponent<bl_BodyPart>() != null && !isNetwork)
                {
                    hit.transform.GetComponent<bl_BodyPart>().GetDamage(damage, PhotonNetwork.player.name, GunName, DirectionFrom, OwnGunID);
                }
                go = GameObject.Instantiate(bloodParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                go.transform.parent = hit.transform;
                Destroy(this.gameObject);
                break;
            case "Wood":
                hitCount++; // add another hit to counter
                type = HitType.WOOD;
                go = GameObject.Instantiate(woodParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                go.transform.parent = hit.transform;
                MakeBulletHole(hit, hit.transform.gameObject);
                break;
            case "Concrete":
                hitCount += 2; // add 2 hits to counter... concrete is hard
                type = HitType.CONCRETE;
                go = GameObject.Instantiate(concreteParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                MakeBulletHole(hit, hit.transform.gameObject);
                go.transform.parent = hit.transform;
                break;
            case "Glass":
                type = HitType.GLASS;
                MakeBulletHole(hit, hit.transform.gameObject);
                go.transform.parent = hit.transform;
                break;
            case "Metal":
                hitCount += 3; // metal slows bullets alot
                type = HitType.METAL;
                go = GameObject.Instantiate(metalParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                MakeBulletHole(hit, hit.transform.gameObject);
                go.transform.parent = hit.transform;
                break;
            case "oldMetal":
                hitCount += 3; // metal slows bullets alot
                type = HitType.OLD_METAL;
                go = GameObject.Instantiate(metalParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                MakeBulletHole(hit, hit.transform.gameObject);
                go.transform.parent = hit.transform;
                break;
            case "Dirt":
                hasHit = true; // ground kills bullet
                type = HitType.CONCRETE;
                go = GameObject.Instantiate(sandParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                MakeBulletHole(hit, hit.transform.gameObject);
                go.transform.parent = hit.transform;
                break;
            case "water":
                hasHit = true; // water kills bullet
                go = GameObject.Instantiate(waterParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                go.transform.parent = hit.transform;
                break;
            default:
                hitCount++; // add a hit
                type = HitType.GENERIC;
                go = GameObject.Instantiate(genericParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                MakeBulletHole(hit, hit.transform.gameObject);
                go.transform.parent = hit.transform;
                break;
        }
        if (mHitsSounds.Count > 0 && audio != null)
        {
            AudioSource.PlayClipAtPoint(mHitsSounds[Random.Range(0, mHitsSounds.Count)], transform.position, 1.0f);
        }
        
    }

    void OnBackHit(RaycastHit hit)
    {
        GameObject go;

        switch (hit.transform.tag) // decide what the bullet collided with and what to do with it
        {
            case "Projectile":
                // do nothing if 2 bullets collide
                break;
            case "Wood":
                type = HitType.WOOD;
                go = GameObject.Instantiate(woodParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                MakeBulletHole(hit, hit.transform.gameObject);
                go.transform.parent = hit.transform;
                break;
            case "Concrete":
                type = HitType.CONCRETE;
                go = GameObject.Instantiate(concreteParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                MakeBulletHole(hit, hit.transform.gameObject);
                go.transform.parent = hit.transform;
                break;
            case "Glass":
                type = HitType.GLASS;
                MakeBulletHole(hit, hit.transform.gameObject);
                break;
            case "Metal":
                type = HitType.METAL;
                go = GameObject.Instantiate(metalParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                MakeBulletHole(hit, hit.transform.gameObject);
                go.transform.parent = hit.transform;
                break;
            case "oldMetal":
                type = HitType.OLD_METAL;
                go = GameObject.Instantiate(metalParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                MakeBulletHole(hit, hit.transform.gameObject);
                go.transform.parent = hit.transform;
                break;
            case "Dirt":
                type = HitType.CONCRETE;
                go = GameObject.Instantiate(sandParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                MakeBulletHole(hit, hit.transform.gameObject);
                go.transform.parent = hit.transform;
                break;
            case "Water":
                go = GameObject.Instantiate(waterParticle, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                break;
            default:
                type = HitType.GENERIC;
                MakeBulletHole(hit, hit.transform.gameObject);
                break;
        }
    }

    #endregion

    public void SetTracer()
    {
        isTracer = true; // tell this bullet it is only a tracer... keeps this object from applying damage
    }
}