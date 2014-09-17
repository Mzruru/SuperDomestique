using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class FogController : TimedColourDensityController {
	
	override protected void UpdateValues (Color colour, float value) {
		if (value <= 0) RenderSettings.fog = false;
		else {
			RenderSettings.fog = true;
			RenderSettings.fogDensity = value;
			RenderSettings.fogColor = colour;
		}
	}
}
