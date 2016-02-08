using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Safezone: MonoBehaviour {
	public float HaloTimeInterval=0.05f;
	public float lightIntensityMax = 6.5f;
	public float lightIntensityMin = 4.5f;
	public float lifeSpan = 20f;
	public float fadingTime = 2f;
	//for enemy entering safezone
	public float killingRadius;

	//for halo
	int index=0;
	int prevIndex=0;
	float timer=0;  //for calculating timer interval
	List<GameObject> Halos = new List<GameObject>();

	//for light
	Light myLight;
	int middle;
	float intensitySegment;
	float time=0f;

	void Start () {
		//initialize halo
		foreach (Transform child in transform) {
			Halos.Add (child.gameObject);
		}
		middle = Halos.Count;
		intensitySegment = (lightIntensityMax - lightIntensityMin) / (Halos.Count - 1);
		for (int i = Halos.Count-2; i >=1; i--) {
			Halos.Add (Halos[i]);
		}
		myLight = GetComponent<Light> ();
		//setup killing zone radius, it is smaller than collider's radius
		killingRadius = GetComponent<SphereCollider> ().radius * 0.9f;
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if (time > lifeSpan) {
			Die ();
		} else {
			UpdateHalo ();
		}
	}

	void UpdateHalo(){
		//update light
		myLight.intensity = Mathf.Abs(middle - index) * intensitySegment + lightIntensityMin;

		//halo changes when timer reaches interval
		if (timer > HaloTimeInterval) {
			Halos [prevIndex].SetActive (false);
			Halos [index].SetActive (true);
			prevIndex = index;
			index = (index + 1) % Halos.Count;
			timer = 0;
		} else { //increment timer
			timer += Time.deltaTime;
		}

	}

	//halo disappears, light fades away
	void Die(){
		Halos [prevIndex].SetActive (false);
		float newIntensity = myLight.intensity - lightIntensityMax * Time.deltaTime / fadingTime;
		if (newIntensity <= 0)
			Destroy (this.gameObject);
		else
			myLight.intensity = newIntensity;
	}
		
}
