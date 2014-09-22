using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkyColoursFogController : SkyColoursController {
	
	[Tooltip("An optional set of values to set the fog density over time")]
	public List<TimedValue> sets;
	 
	// Update is called once per frame
	override protected void UpdateValues (DualColour colours) {
	
		float value = 0;
		foreach (TimedValue set in sets)
		{
			float setValue = set.GetValue(skyColours.GetCurrentTime());
			value += setValue;
		}
		
		if (value == 0)
		{
			RenderSettings.fog = false;
			return;
		}
		
		Color colour = Color.Lerp (colours.lower, colours.upper, skyColourPosition);
		RenderSettings.fog = true;
		RenderSettings.fogColor = colour;
		RenderSettings.fogDensity = value;
	}
}
