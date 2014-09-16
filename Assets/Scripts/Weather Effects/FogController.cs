using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class FogController : MonoBehaviour {

	public List<FogBank> fogBanks;
	
	TimeOfDay timeOfDay;
	
	// Use this for initialization
	void Start () {
		timeOfDay = GetComponent<TimeOfDay> ();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (timeOfDay == null) return;
		
		float density = 0;
		Color colour = new Color(0, 0, 0, 0);
		foreach (FogBank bank in fogBanks)
		{
			float bankDensity = bank.GetDensity(timeOfDay.currentTime);
			density += bankDensity;
			colour += bank.colour * bankDensity;
		}
		
		colour /= density;
		
		if (density <= 0) RenderSettings.fog = false;
		else {
			RenderSettings.fog = true;
			RenderSettings.fogDensity = density;
			RenderSettings.fogColor = colour;
		}
	}
}
