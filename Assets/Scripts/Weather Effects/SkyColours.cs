using UnityEngine;
using System.Collections;

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
				
		//
		public ColourRange midnight = new ColourRange (new Vector4 (0, 0, 0.208f, 1f), new Vector4 (0, 0.137f, 0.404f, 1f));
		public ColourSet dawn = new ColourSet (new TimeRange ("06:00:00", "07:00:00"),
			new ColourRange (new Vector4 (0.043f, 0, 0.345f, 1f), new Vector4 (0, 0.165f, 0.529f, 1f)),
			new ColourRange (new Vector4 (0.870f, 0.917f, 0, 1), new Vector4 (1, 0.302f, 0.302f, 1)),
			new ColourRange (new Vector4 (0.470f, 0.914f, 1, 1), new Vector4 (0.856f, 0.914f, 1, 1)));
		public ColourRange midday = new ColourRange (new Vector4 (0.447f, 0.976f, 1, 1), new Vector4 (0.706f, 0.706f, 1, 1));
		public ColourSet dusk = new ColourSet (new TimeRange ("19:00:00", "20:00:00"),
			new ColourRange (new Vector4 (0.357f, 0.639f, 0.808f, 1), new Vector4 (0.216f, 0.216f, 0.965f, 1)),
			new ColourRange (new Vector4 (0.196f, 0.255f, 0.839f, 1), new Vector4 (0.745f, 0.216f, 1, 1)),
			new ColourRange (new Vector4 (0, 0.051f, 0.573f, 1), new Vector4 (0, 0, 0.278f, 1)));
		TimeOfDay timeOfDay;
		
		// Use this for initialization
		void Start ()
		{
				timeOfDay = GetComponent<TimeOfDay> ();
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
				UpdateShaderColourValues();
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
}
