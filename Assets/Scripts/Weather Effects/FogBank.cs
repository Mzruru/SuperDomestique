using UnityEngine;
using System.Collections;

[System.Serializable]
public class FogBank
{

		public TimeRange fadeIn = new TimeRange ("00:00:00", "00:00:00");
		public TimeRange fadeOut = new TimeRange ("00:00:00", "00:00:00");
		public float peakDensity = 0;
		public Color colour = Color.white;
	
		public FogBank (TimeRange fadeIn, TimeRange fadeOut, float peakDensity, Color colour)
		{
				this.fadeIn = fadeIn;
				this.fadeOut = fadeOut;
				this.peakDensity = peakDensity;
				this.colour = colour;
		}
	
		public float GetDensity (float currentTime)
		{
				float startF = fadeIn.startFloat;
				float endF = fadeOut.endFloat;
				if (currentTime < startF || currentTime > endF)
						return 0;
				if (fadeIn.IsInRange (currentTime))
						return (currentTime - startF) / fadeIn.length * peakDensity;
				if (fadeOut.IsInRange (currentTime))
						return (1 - ((currentTime - fadeOut.startFloat) / fadeOut.length)) * peakDensity;
				return peakDensity;
		}
}
