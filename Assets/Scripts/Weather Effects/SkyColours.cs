using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SkyColours : MonoBehaviour
{
		[Tooltip("Whether time of day should adjust ambient light value")]
		public bool
				changeAmbientLight;
		[Tooltip("Colour of ambient light during the day")]
		public Color
				dayAmbientLightColour;
		[Tooltip("Colour of ambient light at night")]
		public Color
				nightAmbientLightColour;
		[Tooltip("How overcast the sky is (blends to grey)"), Range(0,1)]
		public float
				overcastAmount = 0;
		[Tooltip("How much the overcast darkens the sky"), Range(0,1)]
		public float
				overcastDarken = 0;
				
		//
		public DualColour midnight = new DualColour (new Vector4 (0, 0, 0.208f, 1f), new Vector4 (0, 0.137f, 0.404f, 1f));
		public TimedColourRangeSet dawn = new TimedColourRangeSet (new TimeRange ("06:00:00", "07:00:00"),
			new DualColour (new Vector4 (0.043f, 0, 0.345f, 1f), new Vector4 (0, 0.165f, 0.529f, 1f)),
			new DualColour (new Vector4 (0.870f, 0.917f, 0, 1), new Vector4 (1, 0.302f, 0.302f, 1)),
			new DualColour (new Vector4 (0.470f, 0.914f, 1, 1), new Vector4 (0.856f, 0.914f, 1, 1)));
		public DualColour midday = new DualColour (new Vector4 (0.447f, 0.976f, 1, 1), new Vector4 (0.706f, 0.706f, 1, 1));
		public TimedColourRangeSet dusk = new TimedColourRangeSet (new TimeRange ("19:00:00", "20:00:00"),
			new DualColour (new Vector4 (0.357f, 0.639f, 0.808f, 1), new Vector4 (0.216f, 0.216f, 0.965f, 1)),
			new DualColour (new Vector4 (0.196f, 0.255f, 0.839f, 1), new Vector4 (0.745f, 0.216f, 1, 1)),
			new DualColour (new Vector4 (0, 0.051f, 0.573f, 1), new Vector4 (0, 0, 0.278f, 1)));
		TimeOfDay timeOfDay;
		float[] times;
		Color[] uppers;
		Color[] lowers;
		
		// Use this for initialization
		void Start ()
		{
				timeOfDay = GetComponent<TimeOfDay> ();
				UpdateLookups ();
				UpdateShaderColourValues ();
		}
		
		public void UpdateShaderColourValues ()
		{
				gameObject.renderer.sharedMaterial.SetColor ("_C0U", midnight.upper);
				gameObject.renderer.sharedMaterial.SetColor ("_C0L", midnight.lower);
				gameObject.renderer.sharedMaterial.SetColor ("_C1U", dawn.start.upper);
				gameObject.renderer.sharedMaterial.SetColor ("_C1L", dawn.start.lower);
				gameObject.renderer.sharedMaterial.SetColor ("_C2U", dawn.mid.upper);
				gameObject.renderer.sharedMaterial.SetColor ("_C2L", dawn.mid.lower);
				gameObject.renderer.sharedMaterial.SetColor ("_C3U", dawn.end.upper);
				gameObject.renderer.sharedMaterial.SetColor ("_C3L", dawn.end.lower);
				gameObject.renderer.sharedMaterial.SetColor ("_C4U", midday.upper);
				gameObject.renderer.sharedMaterial.SetColor ("_C4L", midday.lower);
				gameObject.renderer.sharedMaterial.SetColor ("_C5U", dusk.start.upper);
				gameObject.renderer.sharedMaterial.SetColor ("_C5L", dusk.start.lower);
				gameObject.renderer.sharedMaterial.SetColor ("_C6U", dusk.mid.upper);
				gameObject.renderer.sharedMaterial.SetColor ("_C6L", dusk.mid.lower);
				gameObject.renderer.sharedMaterial.SetColor ("_C7U", dusk.end.upper);
				gameObject.renderer.sharedMaterial.SetColor ("_C7L", dusk.end.lower);
				
				gameObject.renderer.sharedMaterial.SetFloat ("_DawnTime", dawn.timeRange.startFloat);
				gameObject.renderer.sharedMaterial.SetFloat ("_DuskTime", dusk.timeRange.startFloat);
				gameObject.renderer.sharedMaterial.SetFloat ("_DawnLength", dawn.timeRange.length);
				gameObject.renderer.sharedMaterial.SetFloat ("_DuskLength", dusk.timeRange.length);
				
				gameObject.renderer.sharedMaterial.SetFloat ("_OvercastAmount", overcastAmount);
				gameObject.renderer.sharedMaterial.SetFloat ("_OvercastDarken", overcastDarken);
		}
		
		void Update ()
		{
				if (timeOfDay) {
						float currentTime = timeOfDay.currentTime;			
						renderer.sharedMaterial.SetFloat ("_CurrentTime", currentTime);
						if (changeAmbientLight) {
								UpdateAmbientLight (currentTime);
						}
				}
				
				#if UNITY_EDITOR
				if (!Application.isPlaying) {
					UpdateLookups();
					UpdateShaderColourValues();
				}
				#endif
		}
		
		void UpdateAmbientLight (float currentTime)
		{
				if (dawn.IsInRange (currentTime)) {
						RenderSettings.ambientLight = Color.Lerp (nightAmbientLightColour, dayAmbientLightColour, (currentTime - dawn.timeRange.startFloat) / dawn.timeRange.length);
				} else if (dusk.IsInRange (currentTime)) {
						RenderSettings.ambientLight = Color.Lerp (dayAmbientLightColour, nightAmbientLightColour, (currentTime - dusk.timeRange.startFloat) / dusk.timeRange.length);
				} else if (currentTime < dawn.timeRange.startFloat || currentTime > dusk.timeRange.endFloat) {
						RenderSettings.ambientLight = nightAmbientLightColour;
				} else if (currentTime > dawn.timeRange.endFloat) {
						RenderSettings.ambientLight = dayAmbientLightColour;
				}
		}
		
		public float GetCurrentTime ()
		{
			return timeOfDay.currentTime;
		}
		
		public DualColour GetColoursForCurrentTime ()
		{
				if (timeOfDay == null)
						return GetColoursForTime (0);
				return GetColoursForTime (timeOfDay.currentTime);
		}
	
		public DualColour GetColoursForTime (float time)
		{
				float ratio = 0;
		
				int c = times.Length;
				float minusValue = 0;
				Color previousUpper = uppers [0];
				Color previousLower = uppers [1]; 
				Color nextUpper = Color.black;
				Color nextLower = Color.black;
				for (int i = 1; i < c; i++) {
						if (time <= times [i]) {
								nextUpper = uppers [i];
								nextLower = lowers [i];
					
								ratio = (time - minusValue) / (times [i] - minusValue);
								break;
						}
				
						minusValue = times [i];
						previousUpper = uppers [i];
						previousLower = lowers [i];
				}
			
				Color upper = Color.Lerp (previousUpper, nextUpper, ratio); 
				Color lower = Color.Lerp (previousLower, nextLower, ratio);
				if (overcastAmount > 0)
				{
					upper = GetOvercastColour(upper);
					lower = GetOvercastColour(lower);
				}
				return new DualColour (upper, lower);
		}
		
		public Color GetOvercastColour (Color original)
		{
			float overcastValue = ((original.r + original.g + original.b) / 3) * (1 - overcastDarken);
			Color overcastColour = new Color(overcastValue, overcastValue, overcastValue, 1); 
			return Color.Lerp (original, overcastColour, overcastAmount);
		}
		
		void UpdateLookups ()
		{
			times = new float[]{0.0f, dawn.timeRange.startFloat, dawn.timeRange.startFloat + (dawn.timeRange.length / 2), dawn.timeRange.endFloat, 720f,
			dusk.timeRange.startFloat, dusk.timeRange.startFloat + (dusk.timeRange.length / 2), dusk.timeRange.endFloat, 1440f};
				uppers = new Color[]{midnight.upper, dawn.start.upper, dawn.mid.upper, dawn.end.upper, midday.upper, dusk.start.upper, dusk.mid.upper
			, dusk.end.upper, midnight.upper};
				lowers = new Color[]{midnight.lower, dawn.start.lower, dawn.mid.lower, dawn.end.lower, midday.lower, dusk.start.lower, dusk.mid.lower
			, dusk.end.lower, midnight.lower};
		}
}
