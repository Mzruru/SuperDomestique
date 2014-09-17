using UnityEngine;
using System.Collections;

[System.Serializable]
public class TimedColourValue
{

		public TimeRange fadeIn = new TimeRange ("00:00:00", "00:00:00");
		public TimeRange fadeOut = new TimeRange ("00:00:00", "00:00:00");
		public float peakValue = 0;
		public Color colour = Color.white;
	
		public TimedColourValue (TimeRange fadeIn, TimeRange fadeOut, float peakValue, Color colour)
		{
				this.fadeIn = fadeIn;
				this.fadeOut = fadeOut;
				this.peakValue = peakValue;
				this.colour = colour;
		}
	
		public float GetValue (float currentTime)
		{
				float startF = fadeIn.startFloat;
				float endF = fadeOut.endFloat;
				if (currentTime < startF || currentTime > endF)
						return 0;
				if (fadeIn.IsInRange (currentTime))
						return fadeIn.InverseLerp (currentTime) * peakValue;
				if (fadeOut.IsInRange (currentTime))
						return (1 - fadeOut.InverseLerp (currentTime)) * peakValue;
				return peakValue;
		}
}
