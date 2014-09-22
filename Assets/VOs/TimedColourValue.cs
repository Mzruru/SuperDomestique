using UnityEngine;
using System.Collections;

[System.Serializable]
public class TimedColourValue : TimedValue
{
		public Color colour = Color.white;
	
		public TimedColourValue (TimeRange fadeIn, TimeRange fadeOut, float peakValue, Color colour) : base(fadeIn, fadeOut, peakValue)
		{
				this.colour = colour;
		}
}
