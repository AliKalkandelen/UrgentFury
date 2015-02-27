using UnityEngine;
using System.Collections;

public class bl_SimpleChange : MonoBehaviour {

    public GameObject[] AllCustom;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            foreach (GameObject go in AllCustom)
            {
                go.SetActive(false);
            }
            AllCustom[0].SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            foreach (GameObject go in AllCustom)
            {
                go.SetActive(false);
            }
            AllCustom[1].SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            foreach (GameObject go in AllCustom)
            {
                go.SetActive(false);
            }
            AllCustom[2].SetActive(true);
        }
	}
}
