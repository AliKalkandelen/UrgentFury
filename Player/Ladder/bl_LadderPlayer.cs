using UnityEngine;
using System.Collections;

public class bl_LadderPlayer : MonoBehaviour {
    [HideInInspector]
    public bl_Ladder m_ladder;

    public GameObject m_maincamera;
    public float m_climpspeed = 6f;
      //private
    private Vector3 ladderMovement = Vector3.zero;
    private Vector3 lateralMove = Vector3.zero;
    private Vector3 climbDirection = Vector3.zero;
    private float climbDownThreshold = -0.4f;
    private CharacterController m_charactercontroller;
    private bl_PlayerMovement m_movement;

    void Awake()
    {
        m_charactercontroller = base.transform.root.GetComponent<CharacterController>();
        m_movement = this.transform.root.GetComponent<bl_PlayerMovement>();
    }

     void JumpUnlatchLadder()
    {
        this.m_movement.JumpOffLadder();
        this.m_movement.Update();
        this.m_ladder = null;
    }

    public void LadderUpdate()
    {
        if (this.enabled)
        {
            if (Input.GetButton("Jump"))
            {
                float num = Mathf.DeltaAngle(this.transform.eulerAngles.y, this.m_ladder.gameObject.transform.eulerAngles.y);
                if ((num <= 90f) && (num >= -90f))
                {
                    this.JumpUnlatchLadder();
                }
            }
            Vector3 vector = (Vector3)(this.climbDirection.normalized * Input.GetAxis("Vertical"));
            vector = (Vector3)(vector * ((this.m_maincamera.transform.forward.y <= this.climbDownThreshold) ? ((float)(-1)) : ((float)1)));
            this.lateralMove = new Vector3(Input.GetAxis("Horizontal"), (float)0, Input.GetAxis("Vertical"));
            this.lateralMove = this.transform.TransformDirection(this.lateralMove);
            this.ladderMovement = vector + this.lateralMove;
            this.m_charactercontroller.Move((Vector3)((this.ladderMovement * this.m_climpspeed) * Time.deltaTime));
        }
    }

     void LatchLadder(GameObject latchedLadder)
    {
        if (this.enabled)
        {
            this.m_ladder = (bl_Ladder)latchedLadder.GetComponent(typeof(bl_Ladder));
            this.climbDirection = this.m_ladder.ClimbDirection();
            this.m_movement.OnLadder();
        }
    }

     void OnTriggerEnter(Collider other)
    {
        if ((this.enabled && !Input.GetButton("Jump")) && other.CompareTag("Ladder"))
        {
            this.LatchLadder(other.gameObject);
        }
    }

     void OnTriggerExit(Collider other)
    {
        if (this.enabled && other.CompareTag("Ladder"))
        {
            this.UnlatchLadder();
        }
    }

     void UnlatchLadder()
    {
        if (this.enabled)
        {
            this.m_ladder = null;
            this.m_movement.OffLadder(this.ladderMovement);
        }
    }
}
