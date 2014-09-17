using UnityEngine;
using System.Collections;

[System.Serializable]
public class DualColour {

	public Color upper;
	public Color lower;
	
	public DualColour (Color upper, Color lower)
	{
		this.upper = upper;
		this.lower = lower;
	}
}
