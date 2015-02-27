using UnityEngine;
using System.Collections;

public class bl_PlayerAnimations : MonoBehaviour
{

    [HideInInspector]
    public bool m_Update = true;

	public Animation anim;
    public string idleAnim = "";
    public string idleAim = "";
	//Walk
    public string walkForward = "";
    public string walkBackwards = "";
    public string strafeLeft = "";
    public string strafeRight = "";
	//Run
    public string runForward = "";

	//Crouch
    public string crouchIdle = "";
    public string crouchWalkForward = "";
    public string crouchWalkBackwards = "";
    public string crouchWalkLeft = "";
    public string crouchWalkRight = "";
	//Jump
    public string runJump = "";
    public string standingJump = "";

    public Transform rootBone;
    public Transform upperBodyBone;
	private float lowerBodyDeltaAngle = 0;
	private float lowerBodyDeltaAngleTarget;
    [HideInInspector]
    public bool grounded = true;
    [HideInInspector]
    public int state = 0;
    public Transform player;
	private Transform tr;
	private Vector3 lastPosition = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
    public Vector3 localVelocity = Vector3.zero;

	private float angle = 0;
    public float lastYRotation;
    public float turnSpeed;
    public string turnAnim;

    public Transform aimTarget;
    public Transform aimPivot;
    public float aimAngleY = 0.0f;
    public Transform spineRot;
    public Transform spineRot2;
    public Transform spineRot3;
    public float adjust = 35.0f;
	private float adjustX;
    public float maxAngle = 60.0f;
    public float movementSpeed;
    public Transform anims;
    public float rot;
    public float targerRotcrouch = -26;
    public float targerRotStand = 0.0f;
    public float adjustXFinal = 0.0f;

    private Vector3 spi1;
    private Vector3 spi2;
    private Vector3 spi3;
    private Vector3 spi1r;
    private Vector3 spi2r;
    private Vector3 spi3r;

void Awake (){
	tr = player.transform;
	lastPosition = tr.position;

}

void Start (){

	anim[walkForward].speed = 1.3f;
    anim[walkForward].layer = 1;
    anim[walkBackwards].layer = 1;
    anim[strafeLeft].layer = 1;
    anim[strafeRight].layer = 1;
	anim[walkBackwards].speed = 1.3f;
	anim[strafeLeft].speed = 1.3f;
	anim[strafeRight].speed = 1.3f;

	anim[runForward].layer = 1;
	anim[runForward].speed = 1.2f;

	anim[standingJump].layer = 1;
	anim[runJump].layer = 1;

	anim[idleAim].layer = 1;
	//anim[idleAim].AddMixingTransform(upperBodyBone);

	anim[crouchIdle].layer = 1;
	anim[crouchWalkForward].layer = 1;
	anim[crouchWalkBackwards].layer = 1;

    spi1 = spineRot.eulerAngles;
    spi2 = spineRot2.eulerAngles;
    spi3 = spineRot3.eulerAngles;
    spi1r = spineRot.localEulerAngles;
    spi2r = spineRot2.localEulerAngles;
    spi3r = spineRot3.localEulerAngles;
	//anim[turnAnim].layer = 5;
	//anim[turnAnim].speed = 1.5f;
}	
void Update (){
    if (!m_Update)
        return;

	velocity = (tr.position - lastPosition) / Time.deltaTime;
	localVelocity = tr.InverseTransformDirection (velocity);
	localVelocity.y = 0;
	angle = HorizontalAngle (localVelocity);
	lastPosition = tr.position;

	turnSpeed = Mathf.DeltaAngle(lastYRotation, transform.rotation.eulerAngles.y);
	movementSpeed = velocity.magnitude;
	
	if(grounded){
		anim.CrossFade(idleAim);
	
		if(movementSpeed < 1.0f ){
			if(turnSpeed > 0.1f || turnSpeed < - 0.1f){
				anim.Blend(turnAnim, 0.6f);
				return;
			}	
		}
	
		if(state == 0){ //Walk and Run
			
			if(localVelocity.z > 1.0f){ //Forward
			
			
				if(movementSpeed < 5.0f) anim.CrossFade(walkForward, 0.2f);
					else anim.CrossFade(runForward, 0.2f);
					
					if(angle < -25){
						lowerBodyDeltaAngleTarget = -45;
					}
					else if(angle > 25){
						lowerBodyDeltaAngleTarget = 45;
					} else lowerBodyDeltaAngleTarget = 0;

			}else if(localVelocity.z < -1.0f){ //Backward
			
					if(movementSpeed < 5.0f) anim.CrossFade(walkBackwards, 0.2f);
					
					if(angle > 115 && angle < 155){
						lowerBodyDeltaAngleTarget = -45;
					}
					else if(angle < -115 && angle > -155){
						lowerBodyDeltaAngleTarget = 45;
					} else lowerBodyDeltaAngleTarget = 0;

			}else if(localVelocity.x < -1.0f){ 
			
					if(movementSpeed < 5.0f) anim.CrossFade(strafeLeft, 0.5f);
					lowerBodyDeltaAngleTarget = 0;

			}else if(localVelocity.x > 1.0f){
			
					if(movementSpeed < 5.0f) anim.CrossFade(strafeRight, 0.5f);
					lowerBodyDeltaAngleTarget = 0;
			}else {	
				lowerBodyDeltaAngleTarget = 0;
				anim.CrossFade(idleAnim, 0.3f);
			}	

		} else if(state == 1){ //Crouch
			if(localVelocity.z > 0.2f){ 
			
				anim.CrossFade(crouchWalkForward, 0.5f);
					
					if(angle < -25){
						lowerBodyDeltaAngleTarget = -45;
					}
					else if(angle > 25){
						lowerBodyDeltaAngleTarget = 45;
					} else lowerBodyDeltaAngleTarget = 0;
					
			}else if(localVelocity.z < -0.2f){ 
					anim.CrossFade(crouchWalkBackwards, 0.4f);
					
					if(angle > 115 && angle < 155){
						lowerBodyDeltaAngleTarget = -45;
					}
					else if(angle < -115 && angle > -155){
						lowerBodyDeltaAngleTarget = 45;
					} else lowerBodyDeltaAngleTarget = 0;
				
			}else if(localVelocity.x < -0.2f){ 
					anim.CrossFade(crouchWalkLeft, 0.5f);
					lowerBodyDeltaAngleTarget = 0;
					
			}else if(localVelocity.x > 0.2f){
					anim.CrossFade(crouchWalkRight, 0.5f);
					lowerBodyDeltaAngleTarget = 0;
			}else {	
				lowerBodyDeltaAngleTarget = 0;
				anim.CrossFade(crouchIdle, 0.3f);
			}	
		} 
		
	}else{
		
		lowerBodyDeltaAngleTarget = 0;
		float normalizedTime = Mathf.InverseLerp(50, -50, velocity.y);
		anim[standingJump].normalizedTime = normalizedTime;
		anim.CrossFade(standingJump, 0.1f);
		anim[runJump].normalizedTime = normalizedTime;
		anim.CrossFade(runJump, 0.1f);
		
	}	
}

float HorizontalAngle ( Vector3 direction  ){
	return Mathf.Atan2 (direction.x, direction.z) * Mathf.Rad2Deg;
}
void LateUpdate (){
    if (!m_Update)
        return;
	//for rotation
	lastYRotation = transform.rotation.eulerAngles.y;
	
	//Local Rotation 
	lowerBodyDeltaAngle = Mathf.LerpAngle (lowerBodyDeltaAngle, lowerBodyDeltaAngleTarget, Time.deltaTime * 3);
	
	// Aiming up/down
	Vector3 aimDir= (aimTarget.position - aimPivot.position).normalized;
	float targetAngle= Mathf.Asin(aimDir.y) * Mathf.Rad2Deg;
	aimAngleY = Mathf.Lerp(aimAngleY, targetAngle, Time.deltaTime * 8);

	//Adjust player rotation 
	float adjustXtarget;
	if(state == 0 && movementSpeed < 1.0f) adjustXtarget = 34;
	else adjustXtarget = 19;
	adjustX = Mathf.Lerp(adjustX, adjustXtarget, Time.deltaTime * 7);
    
	//Adjust max
	if(aimAngleY > maxAngle) aimAngleY = maxAngle;
	if(aimAngleY < -maxAngle) aimAngleY = -maxAngle;

    

    spi1.z = aimAngleY / 2 - adjust;
    spi2.z = aimAngleY / 2 - adjust;
    spi3.z = aimAngleY - adjust;
    spi1r.x = adjustX;
    spineRot.eulerAngles = spi1;
    spineRot2.eulerAngles = spi2;
    spineRot3.eulerAngles = spi3;
    spineRot.localEulerAngles = spi1r;
	//offset for weapon
    spi3r.y = -aimAngleY / 3;
    spineRot3.localEulerAngles = spi3r;
	Quaternion lowerBodyDeltaRotation = Quaternion.Euler (0, lowerBodyDeltaAngle, 0);
	rootBone.rotation = lowerBodyDeltaRotation * rootBone.rotation;
	upperBodyBone.rotation = Quaternion.Inverse (lowerBodyDeltaRotation) * upperBodyBone.rotation;

	float targetRot;
	if(state == 1 && movementSpeed < 1.0f) targetRot = targerRotcrouch;
	else targetRot = targerRotStand;
	rot = Mathf.Lerp(rot, targetRot, Time.deltaTime * 8);
    Vector3 ani = anims.localEulerAngles;
	ani.y = rot;
    anims.localEulerAngles = ani;

    spi2r.x = adjustXFinal;
    spineRot2.localEulerAngles = spi2r;
	
}

}