using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TimedLightController : TimedColourValueController {
 
	override protected void UpdateValues (Color colour, float value)
	{
		gameObject.light.color = colour;
		gameObject.light.intensity = value;
	}
}
