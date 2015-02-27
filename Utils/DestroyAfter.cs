using UnityEngine;
using System.Collections;

public class DestroyAfter : MonoBehaviour {
    
    public float destroyAfter = 15.0f;

    void Start() {
        Destroy(gameObject, destroyAfter);
    }
}
