//////////////////////////////////////////////////////////////////////////////
// bl_PlayerDamageManager.cs
//
// this contains all the logic of the player health
// This is enabled locally or remotely
//                      Lovatto Studio
/////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable; //Replace default Hashtables with Photon hashtables
using UnityEngine.UI;

public class bl_PlayerDamageManager : bl_PhotonHelper
{

    public GUISkin Skin;
	//variables
	public bool localPlayer = true; //set to false // Am I a local player... or networked
    public string localPlayerName = "";            // what's my name
    [HideInInspector]
    public bool DamageEnabled = true;
   [Space(5)]	
    //Current Player Health
	public float health = 100.0f;
    /// <summary>
    /// Max Player Health
    /// </summary>
	public float maxHealth = 100.0f;   
    /// <summary>
    /// Time to for the player reappears after dead
    /// </summary>
	public float spawnTime = 5.0f;	
    [Space(5)]
    public GameObject m_Ragdoll;
    public bl_BodyPartManager mBodyManager;
    [Space(5)]
    [Header("GUI")]
    public Texture2D Blood;
    public Texture2D PaintFlash;
    /// <summary>
    /// Color of vignnete effect when take damage
    /// </summary>
    public Color PaintFlashColor;
    /// <summary>
    /// Color of UI when player health is low
    /// </summary>
    public Color LowHealtColor = new Color(0.9f, 0, 0);
    private Color CurColor = new Color(0, 0, 0);
    private float m_alpha = 0.0f;
    /// <summary>
    /// Blood Screen Fade speed
    /// </summary>
    public float m_FadeSpeed = 3;
    [Range(0.0f,2.0f)]
    public float m_UIIntensity = 0.2f;
    public Texture2D HitMark;
    public float HitFadeSpeed = 1;
    private float hit_alpha = 0;
    [Space(5)]
    [Header("Shake")]
    public float ShakeSmooth = 3;
    [Range(0.0f,1.0f)]
    public float ShakeTime = 0.07f;
    public float ShakeAmount = 2.5f;
    private bool dead = false;
    private string m_LastShot;
    [Space(5)]
    [Header("Effects")]
    public GameObject m_damageEffect = null;
    public Transform damageEffectTransform = null;
    public AudioClip[] HitsSound;
    [Space(5)]
    [Header("XP range to be given, for each kill")]
    /// <summary>
    /// xp range to be given, for each kill
    /// </summary>
    public Vector2 RandomScore = new Vector2(40, 50);
    private Quaternion CurrentCamRot = Quaternion.identity;
    private Quaternion DefaultCamRot = Quaternion.identity;
    private Text HealthTextUI;
    private Slider m_HealthSlider = null;
    private float m_SliderFactor = 3.7f;   
    const string FallMethod = "FallDown";


    protected override void Awake()
    {
        base.Awake();
    }
	// Use this for initialization
	void Start () {
        if (!isConnected)
            return;

        if (isMine)
        {
            bl_GameManager.isAlive = true;
            localPlayerName = PhotonNetwork.player.name;
            transform.name = localPlayerName;

            if (Camera.main)
            {
                DefaultCamRot = Camera.main.transform.localRotation;
            }
            HealthTextUI = GameObject.Find("Health_UI").GetComponentInChildren<Text>();// get UI Text in Start
            m_HealthSlider = GameObject.Find("SliderHealth").GetComponent<Slider>();
        }
		this.health = this.maxHealth;
        localPlayer = base.isMine;
        if (this.m_damageEffect != null)
        {
            if (this.damageEffectTransform == null)
            {
                this.damageEffectTransform = this.transform;
            }
        }
        if (m_HealthSlider != null)
        {
            m_HealthSlider.maxValue = maxHealth;
        }
        Debug.Log("localPlayerName is : " + localPlayerName + " my ID is : " + photonView.viewID);
	}
    /// <summary>
    /// 
    /// </summary>
    void OnEnable()
    {
        if (this.isMine)
        {           
            bl_GameManager.m_view = this.photonView.viewID;
            bl_EventHandler.OnPickUp += this.OnPickUp;
            bl_EventHandler.OnRoundEnd += this.OnRoundEnd;
            bl_EventHandler.OnDamage += this.GetDamage;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void OnDisable()
    {
        if (this.isMine)
        {
            bl_EventHandler.OnPickUp -= this.OnPickUp;
            bl_EventHandler.OnRoundEnd -= this.OnRoundEnd;
            bl_EventHandler.OnDamage -= this.GetDamage;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (base.isMine)
        {
            if (m_alpha > 0.0f)
            {
                m_alpha = Mathf.Lerp(m_alpha, 0.0f, Time.deltaTime * m_FadeSpeed);
            }
            if (health <= 15)
            {
                CurColor = Color.Lerp(CurColor, LowHealtColor, (Seno(6.0f, 0.1f, 0.0f) * 5) + 0.5f);
            }
            else
            {
                CurColor = Color.Lerp(CurColor, Color.white, (Seno(6.0f, 0.1f, 0.0f) * 5) + 0.5f);
            }
            if (Camera.main != null)//Return smooth camera rotation when shake
            {
                Camera.main.transform.localRotation = Quaternion.Lerp(Camera.main.transform.localRotation, CurrentCamRot, Time.deltaTime * ShakeSmooth);
            }
            if (HealthTextUI != null)
            {
                HealthTextUI.text = bl_UtilityHelper.GetThreefoldChar(health) +"/<size=12>" + maxHealth + "</size>";
            }
            if (m_HealthSlider != null)
            {
                if (m_HealthSlider.value != health)
                {
                    m_HealthSlider.value = Mathf.Lerp(m_HealthSlider.value, health, Time.deltaTime * m_SliderFactor);
                }
            }
        }

        

        if (hit_alpha > 0.0f)
        {
            hit_alpha = Mathf.Lerp(hit_alpha, 0.0f, Time.deltaTime);
        }
    }
    /// <summary>
    /// Call this to make a new damage to the player
    /// </summary>
    /// <param name="t_damage">damage</param>
    /// <param name="t_from">name from player attaker</param>
    /// <param name="t_weapon">weapon when send damage</param>
    /// <param name="t_direction">direction of attacker</param>
	//public void GetDamage (float t_damage,string t_from,string t_weapon,Vector3 t_direction,bool isHeatShot,int weapon_ID)
    public void GetDamage(bl_OnDamageInfo e)
	{
        if (!DamageEnabled)
            return;

        Debug.Log(" I been hit by " +e.mFrom +" for " + e.mDamage + " damage");								
            if (health > 0){
                photonView.RPC("SyncDamage", PhotonTargets.AllBuffered, e.mDamage, e.mFrom,e.mWeapon, e.mDirection, e.mHeatShot, e.mWeaponID, PhotonNetwork.player);
            }
    }
    /// <summary>
    /// Call this when Player Take Damage From fall impact
    /// </summary>
    /// <param name="t_damage"></param>
    public void GetFallDamage(float t_damage)
    {
        if (health > 0)
        {
            Vector3 downpos = this.transform.TransformDirection(Vector3.down);
            photonView.RPC("SyncDamage", PhotonTargets.AllBuffered, t_damage, PhotonNetwork.player.name, FallMethod,-downpos,false,5, PhotonNetwork.player);
        }
    }
    /// <summary>
    /// Sync the Health of player
    /// </summary>
    /// <param name="t_damage"></param>
    /// <param name="t_from"></param>
    /// <param name="t_weapon"></param>
    /// <param name="m_direction"></param>
    /// <param name="isHeatShot"></param>
    /// <param name="weaponID"></param>
    /// <param name="m_sender"></param>
    [RPC]
    void SyncDamage(float t_damage, string t_from, string t_weapon,Vector3 m_direction,bool isHeatShot,int weaponID, PhotonPlayer m_sender)
    {
        if (dead)
            return;
        if (!DamageEnabled)
            return;

        if (health > 0)
        {
            if (this.isMine)
            {
                m_alpha += (t_damage * m_UIIntensity);
                StartCoroutine(Shake());
                if (m_damageEffect != null)
                {
                    DamageEffect();
                }
                if (m_indicator != null)
                {
                    m_indicator.AttackFrom(m_direction);
                }
            }
            else
            {
                if (m_sender.name == base.LocalName)
                {
                    hit_alpha = 2;
                }
               
            }
            if (HitsSound.Length > 0 && t_weapon != FallMethod)//Audio effect of hit
            {
                AudioSource.PlayClipAtPoint(HitsSound[Random.Range(0,HitsSound.Length)],this.transform.position, 1.0f);
            }
        }
        this.m_LastShot = t_from;
        this.health -= t_damage;
        if(health <= 0)
        {
            health = 0.0f;          
         Die(m_LastShot,isHeatShot,t_weapon,weaponID);

         if (localPlayer)
         {
             bl_GameManager.isAlive = false;
         }

        }
    }
    /// <summary>
    /// Sync Health when pick up a medkit.
    /// </summary>
    /// <param name="t_amount"></param>
    [RPC]
    void PickUpHealth(float t_amount)
    {
        this.health  = t_amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
    /// <summary>
    /// Called This when player Die Logic
    /// </summary>
    /// <param name="t_from"></param>
	void Die(string t_from,bool isHeat,string t_weapon,int w_id)
	{
        dead = true;
        if (!localPlayer)
        {
            mBodyManager.Ragdolled();// convert into ragdoll the remote player
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        //Spawn ragdoll
        if (!localPlayer)// when player is not ours
        {
            if ( m_LastShot == base.LocalName)
            {
                AddKill(isHeat,t_weapon,w_id);
            }           
        }
        else//when is our
        {
            AddDeath();
            GameObject ragdoll;
            ragdoll = Instantiate(m_Ragdoll, transform.position, transform.rotation) as GameObject;
            ragdoll.GetComponent<bl_Ragdoll>().GetVelocity(this.GetComponent<CharacterController>().velocity);
            ragdoll.GetComponent<bl_Ragdoll>().RespawnAfter(5.0f,m_LastShot);
            if (t_from == base.LocalName)
            {
                bl_EventHandler.KillEvent(base.LocalName, "", "Has committed suicide",myTeam, 5, 20);
            }
            StartCoroutine(DestroyThis());
        }
	}
    /// <summary>
    /// Create a blood particle effect.
    /// </summary>
    void DamageEffect()
    {
        GameObject blood = Object.Instantiate(this.m_damageEffect, damageEffectTransform.position, Quaternion.identity) as GameObject;
        blood.transform.parent = damageEffectTransform;
        Destroy(blood, 0.85f);
    }
    /// <summary>
    /// 
    /// </summary>
    void OnGUI()
    {
        if (localPlayer)
        {
           GUI.skin = Skin;
            /* 
            GUI.color = CurColor;
            bl_UtilityHelper.ShadowLabel(new Rect(0, Screen.height - 50, 200, 50), "<size=50>" + bl_UtilityHelper.GetThreefoldChar(health) + "</size> / " + maxHealth, BoxStyle);
            GUI.color = Color.white;*/
            DamageHUD();
        }
        else
        {
            HitMarket();
        }
    }
    /// <summary>
    /// Suicide player
    /// </summary>
    public void Suicide()
    {
        if (localPlayer && bl_GameManager.isAlive)
        {
            bl_OnDamageInfo e = new bl_OnDamageInfo();
            e.mDamage = 500;
            e.mFrom = base.LocalName;
            e.mDirection = transform.position;
            e.mHeatShot = false;
            e.mWeaponID = 5;
            GetDamage(e);
        }
    }

    /// <summary>
    /// show interface damage indicating that our player received damage
    /// </summary>
    void DamageHUD()
    {
        if (PaintFlash == null)
            return;
        if (Blood == null)
            return;

        GUI.color = new Color(PaintFlashColor.r, PaintFlashColor.g, PaintFlashColor.b, m_alpha);
        if (m_alpha > 0.0f)
        {
            GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),PaintFlash);
            GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height),Blood);
        }
        GUI.color = Color.white;
    }
    /// <summary>
    /// show a sign of success, when we have done harm to the enemy
    /// </summary>
    void HitMarket()
    {
        if (HitMark == null)
            return;

        GUI.color = new Color(1, 1, 1, hit_alpha);
        if (hit_alpha > 0.0f)
        {
            GUI.DrawTexture(new Rect((Screen.width/2-13), (Screen.height/2-13), 26, 26),HitMark);
        }
    }
    /// <summary>
    /// when we get a new kill, synchronize and add points to the player
    /// </summary>
    public void AddKill(bool m_heat,string m_weapon,int W_id)
    {
        //Send a new event keelfeed
        //bl_EventHandler.KillEvent(PhotonNetwork.player.name, this.gameObject.name, "Killed ["+m_weaponEnemy+"]", (string)PhotonNetwork.player.customProperties["Team"],777, 30);     
        bl_KillFeed feed = GameObject.FindWithTag("GameManager").GetComponent <bl_KillFeed>();
        if (!m_heat)
        {
            feed.OnKillFeed(base.LocalName, this.gameObject.name, "Killed [" + m_weapon + "]",myTeam, W_id, 30);
        }
        else
        {
            feed.OnKillFeed(base.LocalName, this.gameObject.name, "HeatShot! [" + m_weapon + "]", myTeam, 6, 30);
        }
        
        //Add a new kill and update information
        PhotonNetwork.player.PostKill(1);//Send a new kill
        //Add xp for score and update
        int rando;
        //If heatshot will give you double experience
        if (m_heat)
        {
             rando = Random.Range((int)RandomScore.x * 2, (int)RandomScore.y * 2);
             bl_EventHandler.OnKillEvent("Killed Enemy",(int)rando/2);
             bl_EventHandler.OnKillEvent("HeatShot Bonus", (int)rando / 2);
        }
        else
        {
             rando = Random.Range((int)RandomScore.x, (int)RandomScore.y);
             bl_EventHandler.OnKillEvent("Killed Enemy", (int)rando);
        }
        //Send to update score to player
        PhotonNetwork.player.PostScore(rando);
    
        //TDM only if the score is updated
        if (GetGameMode == GameMode.TDM)
        {
            //Update ScoreBoard
            if (myTeam == Team.Delta.ToString())
            {
                int CurrentScore = (int)PhotonNetwork.room.customProperties[PropiertiesKeys.Team1Score];
                CurrentScore++;
                Hashtable setTeamScore = new Hashtable();
                setTeamScore.Add(PropiertiesKeys.Team1Score, CurrentScore);
                PhotonNetwork.room.SetCustomProperties(setTeamScore);
            }
            else if (myTeam == Team.Recon.ToString())
            {
                int CurrentScore = (int)PhotonNetwork.room.customProperties[PropiertiesKeys.Team2Score];
                CurrentScore++;
                Hashtable setTeamScore = new Hashtable();
                setTeamScore.Add(PropiertiesKeys.Team2Score, CurrentScore);
                PhotonNetwork.room.SetCustomProperties(setTeamScore);
            }
        }

    }
    /// <summary>
    /// When Player take a new Death synchronize Die Point
    /// </summary>
    public void AddDeath()
    {
        PhotonNetwork.player.PostDeaths(1);
    }

    IEnumerator DestroyThis()
    {
        yield return new WaitForSeconds(0.15f);
        PhotonNetwork.Destroy(this.gameObject);
    }

    public void SendShake(int repeat){
        StartCoroutine(RepeatShake(repeat));
    }
    /// <summary>
    /// Use this for repeat more than 1 shake
    /// </summary>
    /// <param name="repeat"></param>
    /// <returns></returns>
    IEnumerator RepeatShake(int repeat)
    {
        if (repeat > 0)
        {
            for (int i = 0; i < repeat; i++)
            {
                StartCoroutine(Shake());
                yield return new WaitForSeconds(ShakeTime);
            }
        }
        else
        {
            StartCoroutine(Shake());
        }
    }
    /// <summary>
    /// use this for shake camera effect
    /// </summary>
    /// <param name="shakeTime"> time for shake camera</param>
    /// <param name="size"> amount of shake</param>
    /// <returns></returns>
    public IEnumerator Shake()
    {

        float rate = 1.0f / ShakeTime;
        float ta = 1.0f;
        float shakePower;
        while (ta > 0.0f)
        {
            ta -= Time.deltaTime * rate;
            shakePower = ta / 50 * Time.deltaTime;

            if (shakePower > 0.0f)
            {
                CurrentCamRot = new Quaternion(Random.Range(-ShakeAmount, ShakeAmount),
                                           Random.Range(-ShakeAmount, ShakeAmount),
                                           Random.Range(-ShakeAmount, ShakeAmount),
                                           Random.Range(-ShakeAmount, ShakeAmount));
                yield return new WaitForSeconds(0.02f);
                CurrentCamRot = DefaultCamRot;
            }
            yield return rate;
        }
    }
    /// <summary>
    /// This event is called when player pick up a medkit
    /// use PhotonTarget.OthersBuffered to save bandwidth
    /// </summary>
    /// <param name="amount"> amount for sum at curent health</param>
    void OnPickUp(float amount)
    {
        if (photonView.isMine)
        {
            float newHealth = health + amount;
            health = newHealth;
            if (health > maxHealth)
            {
                health = maxHealth;
            }
            photonView.RPC("PickUpHealth", PhotonTargets.OthersBuffered,newHealth);
            
        }
    }
    /// <summary>
    /// When round is end 
    /// desactive some functions
    /// </summary>
    void OnRoundEnd()
    {
        DamageEnabled = false;
    }

    /// <summary>
    /// 
    /// </summary>
    public static float Seno(float rate, float amp, float offset = 0.0f)
    {
        return (Mathf.Cos((Time.time + offset) * rate) * amp);
    }

    public GUIStyle BoxStyle
    {
        get
        {
            if (Skin != null)
            {
                return Skin.customStyles[2];
            }
            else
            {
                return null;
            }
        }
    }

    public bl_DamageIndicator m_indicator
    {
        get
        {
            if (this.GetComponent<bl_DamageIndicator>() != null)
            {
                return this.transform.GetComponent<bl_DamageIndicator>();
            }
            else
            {
                return null;
            }
        }
    }

    CharacterController m_charactercontroller
    {
        get
        {
            return this.transform.root.GetComponent<CharacterController>();
        }
    }


}