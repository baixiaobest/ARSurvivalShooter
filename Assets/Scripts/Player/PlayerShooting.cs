﻿using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public int damagePerShot = 20;
    public float timeBetweenBullets = 0.12f;
    public float range = 100f;

    float timer;
    Ray shootRay;
    RaycastHit shootHit;
    int shootableMask;
    ParticleSystem gunParticles;
    LineRenderer gunLine;
    AudioSource gunAudio;
	Light gunLight;
	Rigidbody playerRigidbody;
	float effectsDisplayTime;
	float camRayLength = 100f;
	GameObject target; // enemy to shoot

    void Awake ()
    {
        shootableMask = LayerMask.GetMask ("Shootable");
        gunParticles = GetComponent<ParticleSystem> ();
        gunLine = GetComponent <LineRenderer> ();
        gunAudio = GetComponent<AudioSource> ();
		gunLight = GetComponent<Light> ();
		playerRigidbody = transform.parent.GetComponent<Rigidbody> ();
		effectsDisplayTime = 0.2f * timeBetweenBullets;
    }


    void Update ()
    {
        timer += Time.deltaTime;

		#if UNITY_EDITOR
		if (Input.GetMouseButton(0)) {
			Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit enemyHit;
			
			if (Physics.Raycast (camRay, out enemyHit, camRayLength, shootableMask) && enemyHit.collider.gameObject.tag == "Enemy")
				target = enemyHit.collider.gameObject;
		}
		#elif (UNITY_ANDROID || UNITY_IOS)
		foreach (Touch touch in Input.touches) {
			if (touch.phase != TouchPhase.Ended) {
				Ray camRay = Camera.main.ScreenPointToRay (touch.position);
				RaycastHit enemyHit;
				
				if (Physics.Raycast (camRay, out enemyHit, camRayLength, shootableMask) && enemyHit.collider.gameObject.tag == "Enemy")
					target = enemyHit.collider.gameObject;
			}
		}
		#endif
//
//		if (Input.GetMouseButton (0) || Input.touchCount > 0) {
//			#if UNITY_EDITOR
//			Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);		
//			#elif (UNITY_ANDROID || UNITY_IPHONE)
//			Ray camRay = Camera.main.ScreenPointToRay (Input.GetTouch(0).position);
//			#endif
//			RaycastHit enemyHit;
//
//			if (Physics.Raycast (camRay, out enemyHit, camRayLength, shootableMask) && enemyHit.collider.gameObject.tag == "Enemy")
//				target = enemyHit.collider.gameObject;
//		}
		
		if (target != null) { // Turning
			Vector3 playerToMouse = target.transform.position - transform.position;
			playerToMouse.y = 0f; // No rotation on y, actually not needed here
			playerRigidbody.MoveRotation (Quaternion.LookRotation (playerToMouse));
		}

		if(target != null && target.GetComponent<EnemyHealth>().currentHealth > 0 && timer >= timeBetweenBullets && Time.timeScale != 0)
            Shoot ();

        if(timer >= effectsDisplayTime)
            DisableEffects ();
    }


    public void DisableEffects ()
    {
        gunLine.enabled = false;
        gunLight.enabled = false;
    }


    void Shoot ()
    {
        timer = 0f;

        gunAudio.Play ();

        gunLight.enabled = true;

		gunParticles.Simulate (0.01f);
        gunParticles.Play ();

        gunLine.enabled = true;
        gunLine.SetPosition (0, transform.position);

        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        if(Physics.Raycast (shootRay, out shootHit, range, shootableMask))
        {
            EnemyHealth enemyHealth = shootHit.collider.GetComponent <EnemyHealth> ();
            if(enemyHealth != null)
                enemyHealth.TakeDamage (damagePerShot, shootHit.point);
            
            gunLine.SetPosition (1, shootHit.point);
        }
        else
            gunLine.SetPosition (1, shootRay.origin + shootRay.direction * range);
    }
}
