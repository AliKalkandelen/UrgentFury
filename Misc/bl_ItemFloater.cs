using UnityEngine;
using System.Collections;

public class bl_ItemFloater : MonoBehaviour {

	public float amount = 1.0f;
	public float speed  = 4.0f;

    void OnEnable()
    {
        StartCoroutine(StartIE());
    }

    IEnumerator StartIE () {
		Vector3 pointA = transform.position;
		Vector3 pointB = transform.position + new Vector3(0, amount, 0);
		while (true) {
			yield return StartCoroutine( MoveObject(transform, pointA, pointB, speed));
			yield return StartCoroutine(MoveObject(transform, pointB, pointA, speed));
		}
    }
     
    IEnumerator MoveObject (Transform thisTransform ,Vector3 startPos , Vector3 endPos , float time ) {
		float i = 0.0f;
		float  rate = 1.0f/time;
		while (i < 1.0f) {
			i += Time.deltaTime * rate;
			thisTransform.position = Vector3.Lerp(startPos, endPos, i);
			yield return null;
		}
    }
}
