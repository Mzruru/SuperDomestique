using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TimedLightController : TimedColourDensityController {

	public Color baseColour;
	
	override protected void UpdateValues (Color colour, float value)
	{
		colour += baseColour;
		gameObject.light.color = colour;
		gameObject.light.intensity = value;
	}
}
