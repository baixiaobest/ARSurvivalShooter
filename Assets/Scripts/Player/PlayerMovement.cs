#define EPSON
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
	public float smooth = 0.001f;
	public float boostTime = 20f;
	public Slider speedSlider;
	public GameObject speedUI;

	float originalSmooth;
    Vector3 movement;
	Vector3 dest;
    Animator anim;
    //Rigidbody playerRigidbody;
    int floorMask;
	int shootableMask;
    float camRayLength = 100f; // how far from camera
	float boostTimer;

    void Awake()
    {
		originalSmooth = smooth;
        floorMask = LayerMask.GetMask("Floor");
		shootableMask = LayerMask.GetMask ("Shootable");
		anim = GetComponent<Animator> ();
		boostTimer = 0f;
		speedSlider.maxValue = boostTime;
		//playerRigidbody = GetComponent<Rigidbody> ();
    }

	void FixedUpdate() // Build-in function called on every framerate update
	{
		if (boostTimer > 0) {
			boostTimer -= Time.deltaTime;
			speedSlider.value = boostTimer;
		} else {
			smooth = originalSmooth;
			speedUI.SetActive(false);
		}
		bool epson=false;
		#if EPSON
			epson = true;
		#endif
		if (Input.GetMouseButton (0) || Input.touchCount > 0 || epson) {
			#if (UNITY_EDITOR && !EPSON)
			Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);		
			#elif ((UNITY_ANDROID || UNITY_IPHONE) && !EPSON)
			Ray camRay = Camera.main.ScreenPointToRay (Input.GetTouch(0).position);
			#elif EPSON
			Ray camRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			#endif
			RaycastHit floorHit, otherHit;

			if (Physics.Raycast (camRay, out floorHit, camRayLength, floorMask)) {
				if (!Physics.Raycast(camRay, out otherHit, camRayLength, shootableMask) || otherHit.collider.gameObject.tag != "Enemy") {
					dest = floorHit.point;
					dest.y = 0f;
				}
			}
		}
		Vector3 lastPos = transform.position;
		transform.position = Vector3.MoveTowards (transform.position, dest, smooth);
		anim.SetBool ("IsWalking", lastPos != transform.position);
	}

	public void StartBoost() {
		boostTimer = boostTime;
		smooth *= 1.5f;
		speedUI.SetActive (true);
	}
//
//	void Move ()
//	{
//		movement.Set (h, 0f, v);
//		movement = movement.normalized * speed * Time.deltaTime;
//		playerRigidbody.MovePosition (transform.position + movement);
//	}
//
//	void Turning (RaycastHit floorHit)
//	{
//		Vector3 playerToMouse = floorHit.point - transform.position;
//		playerToMouse.y = 0f; // No rotation on y, actually not needed here
//		playerRigidbody.MoveRotation (Quaternion.LookRotation (playerToMouse));
//	}
}
