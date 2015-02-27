////////////////////////////////////////////////////////////////////////////////
// bl_Gun.cs
//
// This script Charge of the logic of arms
// place it on a GameObject in the root of gun model
//
//                        Lovatto Studio
////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(UnityEngine.AudioSource))]
public class bl_Gun : MonoBehaviour
{

    //General Vars
    [HideInInspector]
    public bool CanFire = true;
    private bool m_enabled = true;
    private bl_GunBob GunBob;
    private bl_SyncWeapon Sync;
    public enum weaponType { Shotgun, Machinegun,Sniper,Pistol, Burst, Launcher }; // use burst for single shot weapons like pistols / sniper rifles
    public weaponType typeOfGun;

    public enum BulletType { Physical, Raycast }; // physical bullets of raycasts
    public BulletType typeOfBullet;

    public int GunID;
    public string GunName = "";
    // basic weapon variables all guns have in common
    public bool SoundReloadByAnim = false;
    public AudioClip TakeSound;
    public AudioClip FireSound;
    public AudioClip ReloadSound;
    public AudioClip ReloadSound2 = null;
    public AudioClip ReloadSound3 = null;
    public AudioSource DelaySource = null;
    // Objects, effects and tracers
    public GameObject bullet = null;        // the weapons bullet object
    public GameObject grenade = null;       // the grenade style round... this can also be used for arrows or similar rounds
    public GameObject rocket = null;        // the rocket round
    public Renderer muzzleFlash = null;     // the muzzle flash for this weapon
    public Light lightFlash = null;         // the light flash for this weapon
    public Transform muzzlePoint = null;    // the muzzle point of this weapon
    public Transform ejectPoint = null;     // the ejection point
    public Transform mountPoint = null;     // the mount point.... more for weapon swapping then anything
    public Rigidbody shell = null;          // the weapons empty shell object
    private bl_MouseLook m_mouse = null;  
    public GameObject impactEffect = null;  // impact effect, used for raycast bullet types
    public GameObject bulletHole = null;    // bullet hole for raycast bullet types   
   public Vector3 AimPosition; //position of gun when is aimed
   private Vector3 DefaultPos;
   private Vector3 CurrentPos;
   [HideInInspector]
   public bool isAmed;
    private bool CanAim;
    public bool useSmooth = true; 
   public float AimSmooth;
   public float ShakeSmooth = 5;
   public float ShakeIntense = 0.03f;
    [Range(0,179)]
   public float AimFog = 50;
   private float DefaultFog;
   private float CurrentFog;

    private Quaternion CamPosition;
    private Quaternion DefaultCamRot;
    public float AimSway = 0.0f;
    private float DefaultSway;
    //Machinegun Vars
    [HideInInspector]
    public bool isFiring = false;          // is the machine gun firing?  used for decreasing accuracy while sustaining fire

    //Shotgun Specific Vars
    public int pelletsPerShot = 10;         // number of pellets per round fired for the shotgun
     public float delayForSecondFireSound = 0.45f;

    //Burst Specific Vars
    public int roundsPerBurst = 3;          // number of rounds per burst fire
    public float lagBetweenShots = 0.5f;    // time between each shot in a burst
    private bool isBursting = false;
    //Launcher Specific Vars
    public List<GameObject> OnAmmoLauncher = new List<GameObject>();
    
    // basic stats
    public int range = 300;                 // range for raycast bullets... bulletType = Ray
    public float damage = 20.0f;            // bullet damage
    public float maxPenetration = 3.0f;     // how many impacts the bullet can survive
    public float fireRate = 0.5f;           // how fast the gun shoots... time between shots can be fired
    public int impactForce = 50;            // how much force applied to a rigid body
    public float bulletSpeed = 200.0f;      // how fast are your bullets

    public int bulletsPerClip = 50;         // number of bullets in each clip
    public int numberOfClips = 5;           // number of clips you start with
    public int maxNumberOfClips = 10;       // maximum number of clips you can hold
    [HideInInspector]
    public int bulletsLeft;                // bullets in the gun-- current clip
    public float DelayFire = 0.85f;
    public float baseSpread = 1.0f;         // how accurate the weapon starts out... smaller the number the more accurate
    public float maxSpread = 4.0f;          // maximum inaccuracy for the weapon
    public float spreadPerSecond = 0.2f;    // if trigger held down, increase the spread of bullets
    public float spread = 0.0f;             // current spread of the gun
    public float decreaseSpreadPerSec = 0.5f;// amount of accuracy regained per frame when the gun isn't being fired 
    private float DefaultSpreat;
    private float DefaultMaxSpread;
    public float reloadTime = 1.0f;         // time it takes to reload the weapon
    [HideInInspector]
    public bool isReloading = false;       // am I in the process of reloading
    // used for tracer rendering
    public int shotsFired = 0;              // shots fired since last tracer round
    public int roundsPerTracer = 1;         // number of rounds per tracer
    private float nextFireTime = 0.0f;      // able to fire again on this frame
    // Kick vars
    public float kickBackAmount = 5.0f;
    //Network Parts 
    public bool localPlayer = true; //set to false // Am I a local player... or networked
    public string localPlayerName = "";            // what's my name
    private Slider m_AmmoSlider = null;
    private float SliderFactor = 3.7f;
    private Text TypeFireText = null;
    private string s_TypeFire = "Full";
    private bool isRegistered = false;

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        m_mouse = transform.root.GetComponentInChildren<bl_MouseLook>();
        GunBob = transform.root.GetComponentInChildren<bl_GunBob>();
        Sync = GameObject.FindWithTag("WeaponManager").GetComponent<bl_SyncWeapon>();
        DefaultSpreat = baseSpread;
        DefaultMaxSpread = maxSpread;
    }
    // Setting up variables as soon as a level starts
    void Start()
    {
        bulletsLeft = bulletsPerClip; // load gun on startup
        localPlayerName = PhotonNetwork.player.name;
        DefaultPos = transform.localPosition;
        DefaultCamRot = Camera.main.transform.localRotation;
        DefaultSway = GunBob.bobbingAmount;
        DefaultFog = Camera.main.fieldOfView;
        CanAim = true;
        TypeFireText = GameObject.Find("TypeFireText").GetComponent<Text>(); //Get the Text for show bullet of scene
        if (muzzleFlash)
        {
            muzzleFlash.gameObject.SetActive(false);
        }
        if (lightFlash)
        {
            lightFlash.enabled = false;
        }
    }
    
   
    /// <summary>
    /// check whats the player is doing every frame
    /// </summary>
    /// <returns></returns>
    bool Update()
    {
        if (!Screen.lockCursor)
            return false;

        if (!localPlayer)
        {
            return false;  // if not the local player.... exit function
        }
        if (!m_enabled)
            return false;

        InputUpdate();
        Aim();
        CameraShakeLerp();
        SyncState();
        
        
        if (isFiring) // if the gun is firing
        {
            spread += spreadPerSecond; // gun is less accurate with the trigger held down
        }
        else
        {
            spread -= decreaseSpreadPerSec; // gun regains accuracy when trigger is released
        }
        if (m_AmmoSlider != null)
        {
            if (m_AmmoSlider.value != bulletsLeft)
            {
                m_AmmoSlider.value = Mathf.Lerp(m_AmmoSlider.value, bulletsLeft,Time.deltaTime * SliderFactor);
            }
            //m_AmmoSlider.value = bulletsLeft;                
        }
        //===========================================================================================
        return true;
    }
    /// <summary>
    /// use FixedUpdate not call it is not necessary to call in each frame
    /// but if need be called in a loop
    /// </summary>
    void FixedUpdate()
    {
        if (typeOfGun == weaponType.Launcher)
        {
            OnLauncherNotAmmo();
        }
    }
    /// <summary>
    /// All Input events 
    /// </summary>
    void InputUpdate()
    {
        // Did the user press fire.... and what kind of weapon are they using ?  ===============
        switch (typeOfGun)
        {
            case weaponType.Shotgun:
                if (Input.GetButtonDown("Fire1") && m_CanFire)
                {
                    ShotGun_Fire();  // fire shotgun
                }
                break;
            case weaponType.Machinegun:
                if (Input.GetButton("Fire1") && m_CanFire)
                {
                    MachineGun_Fire();   // fire machine gun                 
                }
                break;
            case weaponType.Burst:
                if (Input.GetButtonDown("Fire1") && m_CanFire && !isBursting)
                {
                    StartCoroutine(Burst_Fire()); // fire off a burst of rounds                   
                }
                break;

            case weaponType.Launcher:
                if (Input.GetButtonDown("Fire1") && m_CanFire)
                {
                    StartCoroutine(Launcher_Fire());
                }
                break;
            case weaponType.Pistol:
                if (Input.GetButtonDown("Fire1") && m_CanFire)
                {
                    MachineGun_Fire();   // fire Pistol gun     
                }
                break;
            case weaponType.Sniper:
                if (Input.GetButtonDown("Fire1") && m_CanFire)
                {
                    Sniper_Fire();
                }
                break;
        }//=========================================================================================
        if (Input.GetButton("Fire2") && m_CamAim)
        {
            isAmed = true;
        }
        else
        {
            isAmed = false;
        }
        if (Input.GetKeyDown(KeyCode.R) && m_CanReload)
        {
            StartCoroutine(reload());
        }
        if (typeOfGun == weaponType.Machinegun || typeOfGun == weaponType.Burst || typeOfGun == weaponType.Pistol)
        {
            ChangeTypeFire();
            TypeFireText.text = s_TypeFire;
        }
        else
        {
            TypeFireText.text = "Single";
        }
        //used to decrease weapon accuracy as long as the trigger remains down =====================
        if (typeOfGun != weaponType.Launcher)
        {
            if (Input.GetButton("Fire1") && m_CanFire)
            {
                isFiring = true; // fire is down, gun is firing
            }
            else
            {
                isFiring = false;
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && m_CanFire)
            {
                isFiring = true; // fire is down, gun is firing
            }
        }
    }
    /// <summary>
    /// change the type of gun gust
    /// </summary>
    void ChangeTypeFire()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            switch (typeOfGun)
            {
                case weaponType.Machinegun:
                    typeOfGun = weaponType.Burst;
                    if (TypeFireText != null)
                    {
                        s_TypeFire = "Semi";
                    }
                    break;
                case weaponType.Burst:
                    typeOfGun = weaponType.Pistol;
                    if (TypeFireText != null)
                    {
                        s_TypeFire = "Single";
                    }
                    break;
                case weaponType.Pistol:
                    typeOfGun = weaponType.Machinegun;
                    if (TypeFireText != null)
                    {
                        s_TypeFire = "Full";
                    }
                    break;
            }
            audio.clip = ReloadSound;//create a custom swicht sound
            audio.Play();
        }
    }
    /// <summary>
    /// Sync Weapon state for Upper animations
    /// </summary>
    void SyncState()
    {
        if (isFiring && !isReloading )
        {
            if (m_playersync)
            {
                m_playersync.WeaponState = "Firing";
            }
        }
        else if (isAmed && !isFiring && !isReloading )
        {
            if (m_playersync)
            {
                m_playersync.WeaponState = "Aimed";
            }
        }
        else if (isReloading)
        {
            if (m_playersync)
            {
                m_playersync.WeaponState = "Reloading";
            }
        }
        else if (controller.running && !isReloading && !isFiring && !isAmed)
        {
            if (m_playersync)
            {
                m_playersync.WeaponState = "Running";
            }
        }
        else
        {
            if (m_playersync)
            {
                m_playersync.WeaponState = "Idle";
            }
        }
    }

    /// <summary>
    /// update weapon flashes after checking user inout in update function
    /// </summary>
    void LateUpdate()
    {
       
        if (spread >= maxSpread)
        {
            spread = maxSpread;  //if current spread is greater then max... set to max
        }
        else
        {
            if (spread <= baseSpread)
            {
                spread = baseSpread; //if current spread is less then base, set to base
            }
        }
    }
     /// <summary>
     /// Return the camera position why a smooth movement
     /// </summary>
     void CameraShakeLerp(){
         Camera.main.transform.localRotation = Quaternion.Lerp(Camera.main.transform.localRotation, CamPosition, Time.deltaTime * ShakeSmooth);
     }
    /// <summary>
     /// determine the status of the launcher ammo
     /// to decide whether to show or hide the mesh granade
    /// </summary>
     void OnLauncherNotAmmo()
     {
         foreach (GameObject go in OnAmmoLauncher)
         {
             // if not have more ammo for launcher
             //them desactive the grenade in hands
             if (bulletsLeft <= 0 && !isReloading)// if not have ammo
             {
                 go.SetActive(false);
             }
             else
             {
                 go.SetActive(true);
             }
         }
     }
    /// <summary>
    /// fire the machine gun
    /// </summary>
    void MachineGun_Fire()
    {
        if (bulletsLeft <= 0 && numberOfClips > 0)
        {
            StartCoroutine(reload());
            return;
        }
        // If there is more than one bullet between the last and this frame
        // Reset the nextFireTime
        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;

        // Keep firing until we used up the fire time
        while (nextFireTime < Time.time)
        {
            switch (typeOfBullet)
            {
                case BulletType.Physical:
                    StartCoroutine(FireOneShot());  // fire a physical bullet
                    break;
                case BulletType.Raycast:
                    StartCoroutine(FireOneRay());  // fire a raycast.... change to FireOneRay
                    break;
                default:
                    Debug.Log("error in bullet type");
                    break;
            }
            if (Animat != null)
            {
                if (isAmed)
                {
                    Animat.AimFire();
                }
                else
                {
                    Animat.Fire();
                }
            }
            if (Sync)
            {
                Sync.Firing(weaponType.Machinegun.ToString(),spread);
            }
            audio.clip = FireSound;
            audio.spread = Random.Range(1.0f, 1.5f);
            audio.pitch = Random.Range(1.0f, 1.05f);
            audio.Play();
            shotsFired++;
            bulletsLeft--;
            nextFireTime += fireRate;
            EjectShell();
            Kick();
            StartCoroutine(CamShake());
            StartCoroutine(MuzzleFlash());
        }
        
    }
    /// <summary>
    /// fire the sniper gun
    /// </summary>
    void Sniper_Fire()
    {
        if (bulletsLeft <= 0 && numberOfClips > 0)
        {
            StartCoroutine(reload());
            return;
        }
        // If there is more than one bullet between the last and this frame
        // Reset the nextFireTime
        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;

        // Keep firing until we used up the fire time
        while (nextFireTime < Time.time)
        {
            switch (typeOfBullet)
            {
                case BulletType.Physical:
                    StartCoroutine(FireOneShot());  // fire a physical bullet
                    break;
                case BulletType.Raycast:
                    StartCoroutine(FireOneRay());  // fire a raycast.... change to FireOneRay
                    break;
                default:
                    Debug.Log("error in bullet type");
                    break;
            }
            if (Animat != null)
            {
                Animat.Fire();
            }
            if (Sync)
            {
                Sync.Firing(weaponType.Sniper.ToString(),spread);
            }
            StartCoroutine(DelayFireSound());
            shotsFired++;
            bulletsLeft--;
            nextFireTime += fireRate;
            EjectShell();
            Kick();
            StartCoroutine(CamShake());
            if (!isAmed)
            {
                StartCoroutine(MuzzleFlash());
            }
        }

    }

    /// <summary>
    /// activate the effect of flash for a few seconds
    /// randomize rotation of this every time it is displayed
    /// </summary>
    /// <returns></returns>
    IEnumerator MuzzleFlash()
    {
        if (muzzleFlash && lightFlash)  // need to have a muzzle or light flash in order to enable or disable them
        {
            // We shot this frame, enable the muzzle flash         
                muzzleFlash.transform.localRotation = Quaternion.AngleAxis(Random.value * 57.3f, Vector3.forward);
                muzzleFlash.gameObject.SetActive(true); // enable the muzzle and light flashes
                lightFlash.enabled = true;
                yield return new WaitForSeconds(0.04f);
                muzzleFlash.gameObject.SetActive(false); // disable the light and muzzle flashes
                lightFlash.enabled = false;
            
        }
    }
    /// <summary>
    /// burst shooting
    /// </summary>
    /// <returns></returns>
    IEnumerator Burst_Fire()
    {
        int shotCounter = 0;

        if (bulletsLeft <= 0 && numberOfClips > 0)
        {
            StartCoroutine(reload());
            yield break;//return;
        }

        // If there is more than one bullet between the last and this frame
        // Reset the nextFireTime
        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;

        // Keep firing until we used up the fire time
        while (nextFireTime < Time.time)
        {
            while (shotCounter < roundsPerBurst)
            {
                isBursting = true;
                //Debug.Log(" shotCounter = " + shotCounter + ", roundsPerBurst = "+roundsPerBurst);
                switch (typeOfBullet)
                {
                    case BulletType.Physical:
                        StartCoroutine(FireOneShot());  // fire a physical bullet
                        break;
                    case BulletType.Raycast:
                        StartCoroutine(FireOneRay());  // fire a raycast.... change to FireOneRay
                        break;
                    default:
                        Debug.Log("error in bullet type");
                        break;
                }                
                //Debug.Log("FireOneShot Called in Fire function.");
                shotCounter++;
                shotsFired++;
                bulletsLeft--; // subtract a bullet 
                Kick();
                EjectShell();
                StartCoroutine(CamShake());
                StartCoroutine(MuzzleFlash());
                if (Sync)
                {
                    Sync.Firing(weaponType.Burst.ToString(),spread);
                }
                if (Animat != null)
                {
                    Animat.Fire();
                }
                if (FireSound)
                {
                    audio.clip = FireSound;
                    audio.spread = Random.Range(1.0f, 1.5f);
                    audio.Play();
                }
                yield return new WaitForSeconds(lagBetweenShots); 
                
            }           

            nextFireTime += fireRate;
        }
        isBursting = false;
    }

    /// <summary>
    /// fire the shotgun
    /// </summary>
    void ShotGun_Fire()
    {
        int pelletCounter = 0;  // counter used for pellets per round

        if (bulletsLeft <= 0 && numberOfClips > 0)
        {
            StartCoroutine(reload()); // if out of ammo, reload
            return;
        }

        // If there is more than one bullet between the last and this frame
        // Reset the nextFireTime
        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;

        // Keep firing until we used up the fire time
        while (nextFireTime < Time.time)
        {
            do
            {
                switch (typeOfBullet)
                {
                    case BulletType.Physical:
                        StartCoroutine(FireOneShot());  // fire a physical bullet
                        break;
                    case BulletType.Raycast:
                        StartCoroutine(FireOneRay());  // fire a raycast.... change to FireOneRay
                        break;
                    default:
                        Debug.Log("error in bullet type");
                        break;
                }
                if (Sync)
                {
                    Sync.Firing(bl_Gun.weaponType.Shotgun.ToString(), spread);
                }
                pelletCounter++; // add another pellet
                shotsFired++; // another shot was fired                
            } while (pelletCounter < pelletsPerShot); // if number of pellets fired is less then pellets per round... fire more pellets

            StartCoroutine(DelayFireSound());
            if (Animat != null)
            {
                Animat.Fire();
            }
            StartCoroutine(CamShake());
            EjectShell(); // eject 1 shell 
            nextFireTime += fireRate;  // can fire another shot in "firerate" number of frames
            bulletsLeft--; // subtract a bullet
            Kick();
        }
    }
    /// <summary>
    /// most shotguns have the sound of shooting and then reloading
    /// </summary>
    /// <returns></returns>
    IEnumerator DelayFireSound()
    {
        audio.clip = FireSound;
        audio.spread = Random.Range(1.0f, 1.5f);
        audio.Play();
        yield return new WaitForSeconds(delayForSecondFireSound);
        if (DelaySource != null)
        {
            DelaySource.clip = ReloadSound3;
            DelaySource.Play();
        }
        else
        {
            audio.clip = ReloadSound3;
            audio.Play();
        }
    }

    /// <summary>
    /// fire your launcher
    /// </summary>
    IEnumerator Launcher_Fire()
    {
        if (bulletsLeft == 0)
        {
            StartCoroutine(reload()); // if out of ammo, reload
           yield return null;
        }
        
        // If there is more than one bullet between the last and this frame
        // Reset the nextFireTime
        if (Time.time - fireRate > nextFireTime)
            nextFireTime = Time.time - Time.deltaTime;
        
        // Keep firing until we used up the fire time
        while (nextFireTime < Time.time)
        {          
                        
            nextFireTime += fireRate;  // can fire another shot in "firerate" number of frames
            if (Animat != null)
            {
                Animat.Fire();
            }   
            yield return new WaitForSeconds(DelayFire);
            StartCoroutine(FireOneProjectile()); // fire 1 round            
            bulletsLeft--; // subtract a bullet
            Kick();
            if (Sync)
            {
                Sync.Firing(weaponType.Launcher.ToString(),spread);
            }           
            if (FireSound)
            {
                audio.clip = FireSound;
                audio.spread = Random.Range(1.0f, 1.5f);
                audio.Play();
            }
            StartCoroutine(CamShake());
            isFiring = false;
        }
    }

    /// <summary>
    /// Create and fire a bullet
    /// </summary>
    /// <returns></returns>
    IEnumerator FireOneShot()
    {
        Vector3 position = muzzlePoint.position; // position to spawn bullet is at the muzzle point of the gun       

        // set the gun's info into an array to send to the bullet
        bl_BulletInitSettings t_info = new bl_BulletInitSettings();
        t_info.m_damage = damage;
        t_info.m_ImpactForce = impactForce;
        t_info.m_MaxPenetration = maxPenetration;
        t_info.m_maxspread = maxSpread;
        t_info.m_spread = spread;
        t_info.m_speed = bulletSpeed;
        t_info.m_weaponname = GunName;
        t_info.m_position = this.transform.root.position;
        t_info.m_weapinID = this.GunID;
        t_info.isNetwork = false;

        //bullet info is set up in start function
        GameObject newBullet = Instantiate(bullet, position, transform.parent.rotation) as GameObject; // create a bullet
        newBullet.GetComponent<bl_Bullet>().SetUp(t_info);// send the gun's info to the bullet

        if (!(typeOfGun == weaponType.Launcher))
        {
            if (shotsFired >= roundsPerTracer) // tracer round every so many rounds fired... is there a tracer this round fired?
            {
                newBullet.renderer.enabled = true; // turn on tracer effect
                shotsFired = 0;                    // reset tracer counter
            }
            else
            {
                newBullet.renderer.enabled = false; // turn off tracer effect
            }
            audio.clip = FireSound;
            audio.spread = Random.Range(1.0f, 1.5f);
            audio.Play();
        }

        if ((bulletsLeft == 0))
        {
            StartCoroutine(reload());  // if out of bullets.... reload
            yield break;
        }
        
    }

    /// <summary>
    /// Create and Fire a raycast
    /// and show an effect tracing of the bullet
    /// </summary>
    /// <returns></returns>
    IEnumerator FireOneRay()
    {

        int hitCount = 0;
        bool tracerWasFired = false;
        Vector3 position = muzzlePoint.position; // position to spawn bullet is at the muzzle point of the gun
        Vector3 direction = muzzlePoint.TransformDirection(Random.Range(-maxSpread, maxSpread) * spread, Random.Range(-maxSpread, maxSpread) * spread, 1);
        //Vector3 dir = (weaponTarget.transform.position - position) + direction;

        // set the gun's info into an array to send to the bullet
        bl_BulletInitSettings t_info = new bl_BulletInitSettings();
        t_info.m_damage = damage;
        t_info.m_ImpactForce = impactForce;
        t_info.m_MaxPenetration = maxPenetration;
        t_info.m_maxspread = maxSpread;
        t_info.m_spread = spread;
        t_info.m_speed = bulletSpeed;
        t_info.m_weaponname = GunName;
        t_info.m_position = this.transform.root.position;
        t_info.m_weapinID = GunID;
        t_info.isNetwork = false;

        if (shotsFired >= roundsPerTracer)
        {
            FireOneTracer(t_info);
            shotsFired = 0;
            tracerWasFired = true;
        }         
        
        RaycastHit[] hits = Physics.RaycastAll(position , direction, range);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hitCount >= maxPenetration)
            {
                yield break;
            }           

            RaycastHit hit = hits[i];
            //Debug.Log( "Bullet hit " + hit.collider.gameObject.name + " at " + hit.point.ToString() );

            // notify hit
            if (!tracerWasFired)
            { // tracers are set to show impact effects... we dont want to show more then 1 per bullet fired
                ShowHits(hit); // show impacts effects if no tracer was fired this round
            }
            
            // Debug.Log("if " + hitCount + " > " + maxHits + " then destroy bullet...");    
            hitCount++;
        }        
    }
    
    /// <summary>
    /// Create and Fire 1 launcher projectile
    /// *** this is WIP ***
    /// </summary>
    /// <returns></returns>
    IEnumerator FireOneProjectile()
    {
        Vector3 position = muzzlePoint.position; // position to spawn rocket / grenade is at the muzzle point of the gun

        bl_BulletInitSettings t_info = new bl_BulletInitSettings();
        t_info.m_damage = damage;
        t_info.m_ImpactForce = impactForce;
        t_info.m_MaxPenetration = maxPenetration;
        t_info.m_maxspread = maxSpread;
        t_info.m_spread = spread;
        t_info.m_speed = bulletSpeed;
        t_info.m_weaponname = GunName;
        t_info.m_position = this.transform.root.position;
        t_info.m_weapinID = GunID;
        t_info.isNetwork = false;

        //Instantiate grenade
                GameObject newNoobTube = Instantiate(grenade, position, transform.parent.rotation) as GameObject;
                if (newNoobTube.rigidbody != null)//if grenade have a rigitbody,then apply velocity
                {
                    newNoobTube.rigidbody.angularVelocity = (UnityEngine.Random.onUnitSphere * 10f);
                }
                newNoobTube.GetComponent<bl_Grenade>().SetUp(t_info);// send the gun's info to the grenade    
           
        if ((bulletsLeft == 0 && numberOfClips > 0))
        {
            StartCoroutine(reload());  // if out of bullets.... reload
            yield break;
        }

    }
    /// <summary>
    /// create and "fire" an empty shell
    /// apply the speed and direction randomized
    /// </summary>
    void EjectShell()
    {
        Vector3 position = ejectPoint.position; // ejectile spawn point at gun's ejection point
        
        if (shell)
        {
            Rigidbody newShell = Instantiate(shell, position, transform.parent.rotation) as Rigidbody; // create empty shell
            //give ejectile a slightly random ejection velocity and direction
            newShell.velocity = transform.TransformDirection(-Random.Range(-2, 2) - 3.0f, Random.Range(-1, 2) + 3.0f, Random.Range(-2, 2) + 1.0f);
        }
    }
    // tracer rounds for raycast bullets
    void FireOneTracer(bl_BulletInitSettings info)
    {
        Vector3 position = muzzlePoint.position; 
        GameObject newTracer = Instantiate(bullet, position, transform.parent.rotation) as GameObject; // create a bullet
        newTracer.SendMessageUpwards("SetUp", info); // send the gun's info to the bullet
        newTracer.GetComponent<bl_Bullet>().SetTracer();  // tell the bullet it is only a tracer
    }

    /// <summary>
    /// effects for raycast bullets
    /// </summary>
    /// <param name="hit"></param>
    void ShowHits(RaycastHit hit)
    {
        switch (hit.transform.tag)
        {
            case "bullet":
                // do nothing if 2 bullets collide
                break;
            case "BodyPart":
                if (hit.transform.GetComponent<bl_BodyPart>() != null)
                {
                    hit.transform.GetComponent<bl_BodyPart>().GetDamage(damage, PhotonNetwork.player.name, GunName, this.transform.root.position, GunID);
                }
                break;
            case "Wood":
                // add wood impact effects
                break;
            case "Concrete":
                // add concrete impact effect
                break;
            case "Dirt":
                // add dirt or ground  impact effect
                break;
            default: // default impact effect and bullet hole
                Instantiate(impactEffect, hit.point + 0.1f * hit.normal, Quaternion.FromToRotation(Vector3.up, hit.normal));
                GameObject newBulletHole = Instantiate(bulletHole, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject;
                newBulletHole.transform.parent = hit.transform;
                break;
        }
    }
    /// <summary>
    /// ADS
    /// </summary>
    void Aim()
    {
        if (isAmed)
        {
            CurrentPos = AimPosition; //Place in the center ADS
            CurrentFog = AimFog; //create a zoom camera
            GunBob.transform.localPosition = Vector3.zero; //Fix position of gun
            GunBob.bobbingAmount = AimSway; //setting the sway of weapons
            baseSpread = typeOfGun == weaponType.Sniper ? 0.01f : DefaultSpreat / 2f;//if sniper more accuracy
            maxSpread = typeOfGun == weaponType.Sniper ? 0.01f : DefaultMaxSpread / 2; //add more accuracy when is aimed
        }
        else // if not aimed
        {
            CurrentPos = DefaultPos; //return to default gun position       
            CurrentFog = DefaultFog; //return to default fog
            GunBob.bobbingAmount = DefaultSway; //enable the gun bob
            baseSpread = DefaultSpreat; //return to default spreat
            maxSpread = DefaultMaxSpread; //return to default max spread
        }
        //apply position
        transform.localPosition = useSmooth ? Vector3.Lerp(transform.localPosition, CurrentPos, Time.deltaTime * AimSmooth) : //with smoot effect
        Vector3.MoveTowards(transform.localPosition, CurrentPos, Time.deltaTime * AimSmooth); // with snap effect
        

        Camera.main.fieldOfView = useSmooth ? Mathf.Lerp(Camera.main.fieldOfView, CurrentFog, Time.deltaTime * (AimSmooth * 3)) : //apply fog distance
         Mathf.Lerp(Camera.main.fieldOfView, CurrentFog, Time.deltaTime * AimSmooth );
    }

    /// <summary>
    /// send kick back to mouse look
    /// when is fire
    /// </summary>
    void Kick()
    {
       m_mouse.offsetY += kickBackAmount;
    }

    /// <summary>
    /// start reload weapon
    /// deduct the remaining bullets in the cartridge of a new clip
    /// as this happens, we disable the options: fire, aim and run
    /// </summary>
    /// <returns></returns>
    IEnumerator reload()
    {
        isAmed = false;
        CanFire = false;
       
        if (isReloading)      
            yield break; // if already reloading... exit and wait till reload is finished

        if (numberOfClips > 0)//if have at least one cartridge
        {
            if (Animat != null)
            {
                if (typeOfGun == weaponType.Shotgun)
                {
                    int t_repeat = bulletsPerClip - bulletsLeft; //get the number of spent bullets
                    Animat.ReloadRepeat(reloadTime, t_repeat);
                }
                else
                {
                    Animat.Reload(reloadTime);
                }
            }
            if (!SoundReloadByAnim)
            {
                StartCoroutine(ReloadSoundIE());
            }
            isReloading = true; // we are now reloading
            numberOfClips--; // take away a clip
            yield return new WaitForSeconds(reloadTime); // wait for set reload time
            bulletsLeft = bulletsPerClip; // fill up the gun
        }      
        isReloading = false; // done reloading
        CanAim = true;
        CanFire = true;
    }

   /// <summary>
    /// use this method to various sounds reload.
    /// if you have only 1 sound, them put only one in inspector
    /// and leave emty other box
   /// </summary>
   /// <returns></returns>
    IEnumerator ReloadSoundIE()
    {
        float t_time = reloadTime / 3;
        if (ReloadSound != null)
        {
            audio.clip = ReloadSound;
            audio.Play();
            if (GManager != null)
            {
                GManager.heatReloadAnim(1);
            }
                  
        }
        if (ReloadSound2 != null)
        {
            if (typeOfGun == weaponType.Shotgun)
            {
                int t_repeat = bulletsPerClip - bulletsLeft;
                for (int i = 0; i < t_repeat; i++)
                {
                    yield return new WaitForSeconds(t_time / t_repeat + 0.025f);
                    audio.clip = ReloadSound2;
                    audio.Play();
                   
                }
            }
            else
            {
                yield return new WaitForSeconds(t_time);
                audio.clip = ReloadSound2;
                audio.Play();
            }
        }
        if (ReloadSound3 != null)
        {
            yield return new WaitForSeconds(t_time);
            audio.clip = ReloadSound3;
            audio.Play();
            if (GManager != null)
            {
                GManager.heatReloadAnim(2);
            }
        }
        yield return new WaitForSeconds(0.65f);
        if (GManager != null)
        {
            GManager.heatReloadAnim(0);
        }
    }

    /// <summary>
    /// move the camera in a small range
    /// with the presets Gun
    /// </summary>
    /// <returns></returns>
    IEnumerator CamShake()
    {

        float shakeIntensity = 0.1f;
        while (shakeIntensity > 0)
        {
            CamPosition = new Quaternion(
                         Random.Range(-ShakeIntense * 2.5f, ShakeIntense * 2.5f),
                         Random.Range(-ShakeIntense * 2.5f, ShakeIntense * 2.5f),
                         Random.Range(-ShakeIntense * 2.5f, ShakeIntense * 2.5f),
                         Random.Range(-ShakeIntense * 4.1f, ShakeIntense * 4.1f)

                );
            shakeIntensity -= 0.0075f;
            yield return false;
        }
        //yield return new WaitForSeconds(0.03f);
        CamPosition = DefaultCamRot;

    }
    /// <summary>
    /// 
    /// </summary>
    void OnEnable()
    {
        audio.clip = TakeSound;
        audio.Play();
        if (Animat)
        {
            Animat.DrawWeapon();
        }
        CanFire = true;
        CanAim = true;
        if (!isRegistered)
        {
            isRegistered = true; 
            bl_EventHandler.OnKitAmmo += this.OnPickUpAmmo;
            bl_EventHandler.OnRoundEnd += this.OnRoundEnd;
        }
        m_AmmoSlider = GameObject.Find("SliderAmmo").GetComponent<Slider>();
        if (m_AmmoSlider != null)
        {
            m_AmmoSlider.maxValue = bulletsPerClip;
        }
    }
    /// <summary>
    /// When we disable the gun ship called the animation
    /// and disable the basic functions
    /// </summary>
    public void DisableWeapon()
    {
        CanAim = false;
        isReloading = false;
        CanFire = false;
        if (Animat)
        {
            Animat.HideWeapon();
        }
        if (GManager != null)
        {
            GManager.heatReloadAnim(0);
        }
        StopAllCoroutines();
    }

    /// <summary>
    /// When round is end we can't fire
    /// </summary>
    void OnRoundEnd()
    {
        m_enabled = false;
    }

    void OnPickUpAmmo(int t_clips)
    {
        if (numberOfClips < maxNumberOfClips)
        {
            numberOfClips += t_clips;
            if (numberOfClips > maxNumberOfClips)
            {
                numberOfClips = maxNumberOfClips;
            }
        }
    }

    public bl_WeaponAnin Animat
    {
        get
        {
            return this.GetComponentInChildren<bl_WeaponAnin>();
        }
    }
    public bl_PlayerMovement controller
    {
        get
        {
            return transform.root.GetComponent<bl_PlayerMovement>();
        }
    }
    public bl_PlayerSync m_playersync
    {
        get
        {
            return this.transform.root.GetComponent<bl_PlayerSync>();
        }
    }
    /// <summary>
    /// determine if we are ready to shoot
    /// TIP: if you want to have to shoot when running
    /// just remove "!controller.run" of the condition
    /// </summary>
   public bool m_CanFire
    {
        get
        {
            bool can = false;
            if (bulletsLeft > 0 && CanFire && !isReloading && !controller.run)
            {
                can = true;
            }
            return can;
        }
    }
    /// <summary>
    /// determine if we can Aim
    /// </summary>
   public bool m_CamAim
   {
       get
       {
           bool can = false;
           if (CanAim && !controller.run && !controller.m_OnLadder)
           {
               can = true;
           }
           return can;
       }
   }
    /// <summary>
    /// determine is we can reload
    /// TIP: if you want to have to shoot when running
    /// just remove "!controller.run" of the condition
    /// </summary>
   bool m_CanReload
   {
       get
       {
           bool can = false;
           if (bulletsLeft < bulletsPerClip && numberOfClips > 0 && !controller.run)
           {
               can = true;
           }
           return can;
       }
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