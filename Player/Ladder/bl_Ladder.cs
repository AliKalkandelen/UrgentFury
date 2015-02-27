using UnityEngine;
using System.Collections;

public class bl_Ladder : MonoBehaviour {

    private Vector3 climbDirection = Vector3.zero;

    void Start()
    {
        this.climbDirection = (this.gameObject.transform.position + new Vector3(0, (this.collider as BoxCollider).size.y / (2), 0)) - (this.gameObject.transform.position - new Vector3(0, (this.collider as BoxCollider).size.y / (2),0));
    }

    public Vector3 ClimbDirection()
    {
        return this.climbDirection;
    }

    
}