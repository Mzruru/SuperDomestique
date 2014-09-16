using UnityEngine;
using System.Collections;

[System.Serializable]
public class ColourRange {

	public Color upper;
	public Color lower;
	
	public ColourRange (Color upper, Color lower)
	{
		this.upper = upper;
		this.lower = lower;
	}
}
