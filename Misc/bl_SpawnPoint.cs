using UnityEngine;
using System.Collections;

public class bl_SpawnPoint : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (this.transform.renderer != null)
        {
            this.renderer.enabled = false;
        }
	}
	
}
