using UnityEngine;
using System.Collections;

public class FlareController : MonoBehaviour {

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.CompareTag ("Player")) {
			col.gameObject.GetComponentInChildren<PlayerShooting> ().GetFlare ();
			Destroy(gameObject);
		}
	}
}
