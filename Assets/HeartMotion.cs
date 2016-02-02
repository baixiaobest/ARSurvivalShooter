using UnityEngine;
using System.Collections;

public class HeartMotion : MonoBehaviour {

	Vector3 rot = new Vector3(0, 100, 0);

	// Update is called once per frame
	void Update () {
		transform.Rotate (rot * Time.deltaTime);
	}
}
