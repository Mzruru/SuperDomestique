using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SkyColours : MonoBehaviour
{
		[Tooltip("The time of day in format HH:MM:SS (or HH:MM), 24 hour clock")]
		public string
				timeOfDay = "00:00:00";
		[Tooltip("How fast time passes")]
		public float
				timeScale = 2.0f;
		[Tooltip("What time dawn should start")]
		public string
				dawnTime = "06:00:00";
		[Tooltip("What time dusk should start")]
		public string
				duskTime = "19:00:00";
		[Tooltip("How long dawn should last")]
		public float
				dawnLength = 60.0f;
		[Tooltip("How long dusk should last")]
		public float
				duskLength = 60.0f;
		[Tooltip("Whether time of day should adjust ambient light value")]
		public bool
				changeAmbientLight;
		[Tooltip("Colour of ambient light during the day")]
		public Color
				dayAmbientLightColour;
		[Tooltip("Colour of ambient light at night")]
		public Color
				nightAmbientLightColour;
		[HideInInspector]
		public float
				_currentTime = 0.0f;
		//
		public Color upperMidnight = new Vector4 (0, 0, 0.208f, 1f);
		public Color lowerMidnight = new Vector4 (0, 0.137f, 0.404f, 1f);
		public Color upperDawnStart = new Vector4 (0.043f, 0, 0.345f, 1f);
		public Color lowerDawnStart = new Vector4 (0, 0.165f, 0.529f, 1f);
		public Color upperDawnMid = new Vector4 (0.870f, 0.917f, 0, 1);
		public Color lowerDawnMid = new Vector4 (1, 0.302f, 0.302f, 1);
		public Color upperDawnEnd = new Vector4 (0.470f, 0.914f, 1, 1);
		public Color lowerDawnEnd = new Vector4 (0.856f, 0.914f, 1, 1);
		public Color upperMidday = new Vector4 (0.447f, 0.976f, 1, 1);
		public Color lowerMidday = new Vector4 (0.706f, 0.706f, 1, 1);
		public Color upperDuskStart = new Vector4 (0.357f, 0.639f, 0.808f, 1);
		public Color lowerDuskStart = new Vector4 (0.216f, 0.216f, 0.965f, 1);
		public Color upperDuskMid = new Vector4 (0.196f, 0.255f, 0.839f, 1);
		public Color lowerDuskMid = new Vector4 (0.745f, 0.216f, 1, 1);
		public Color upperDuskEnd = new Vector4 (0, 0.051f, 0.573f, 1);
		public Color lowerDuskEnd = new Vector4 (0, 0, 0.278f, 1);
		float _dawnTimeFloat;
		float _duskTimeFloat;
			
		// Use this for initialization
		void Start ()
		{
				_currentTime = StringUtils.ConvertTimeStringToFloat (timeOfDay);
				_dawnTimeFloat = StringUtils.ConvertTimeStringToFloat (dawnTime);
				_duskTimeFloat = StringUtils.ConvertTimeStringToFloat (duskTime);
				UpdateShaderColourValues ();
		}
		
		public void UpdateShaderColourValues ()
		{
				gameObject.renderer.sharedMaterial.SetColor ("_C0U", upperMidnight);
				gameObject.renderer.sharedMaterial.SetColor ("_C0L", lowerMidnight);
				gameObject.renderer.sharedMaterial.SetColor ("_C1U", upperDawnStart);
				gameObject.renderer.sharedMaterial.SetColor ("_C1L", lowerDawnStart);
				gameObject.renderer.sharedMaterial.SetColor ("_C2U", upperDawnMid);
				gameObject.renderer.sharedMaterial.SetColor ("_C2L", lowerDawnMid);
				gameObject.renderer.sharedMaterial.SetColor ("_C3U", upperDawnEnd);
				gameObject.renderer.sharedMaterial.SetColor ("_C3L", lowerDawnEnd);
				gameObject.renderer.sharedMaterial.SetColor ("_C4U", upperMidday);
				gameObject.renderer.sharedMaterial.SetColor ("_C4L", lowerMidday);
				gameObject.renderer.sharedMaterial.SetColor ("_C5U", upperDuskStart);
				gameObject.renderer.sharedMaterial.SetColor ("_C5L", lowerDuskStart);
				gameObject.renderer.sharedMaterial.SetColor ("_C6U", upperDuskMid);
				gameObject.renderer.sharedMaterial.SetColor ("_C6L", lowerDuskMid);
				gameObject.renderer.sharedMaterial.SetColor ("_C7U", upperDuskEnd);
				gameObject.renderer.sharedMaterial.SetColor ("_C7L", lowerDuskEnd);
				
				gameObject.renderer.sharedMaterial.SetFloat ("_DawnTime", _dawnTimeFloat);
				gameObject.renderer.sharedMaterial.SetFloat ("_DuskTime", _duskTimeFloat);
				gameObject.renderer.sharedMaterial.SetFloat ("_DawnLength", dawnLength);
				gameObject.renderer.sharedMaterial.SetFloat ("_DuskLength", duskLength);
		}
		
		void Update ()
		{
		
				_dawnTimeFloat = StringUtils.ConvertTimeStringToFloat (dawnTime);
				_duskTimeFloat = StringUtils.ConvertTimeStringToFloat (duskTime);
				_currentTime = StringUtils.ConvertTimeStringToFloat (timeOfDay);
				_currentTime += Time.smoothDeltaTime * timeScale;
				if (_currentTime > 1440.0f) {
						_currentTime -= 1440.0f;
				}
			
				renderer.sharedMaterial.SetFloat ("_CurrentTime", _currentTime);
			
				timeOfDay = StringUtils.ConvertTimeFloatToString (_currentTime);
				
				if (changeAmbientLight) {
						UpdateAmbientLight ();
				}
				#if UNITY_EDITOR
				UpdateShaderColourValues();
				#endif
		}
		
		void UpdateAmbientLight ()
		{
				if (_currentTime < _dawnTimeFloat || _currentTime > _duskTimeFloat + duskLength) {
						RenderSettings.ambientLight = nightAmbientLightColour;
				} else if (_currentTime > _dawnTimeFloat + dawnLength && _currentTime < _duskTimeFloat) {
						RenderSettings.ambientLight = dayAmbientLightColour;
				} else if (_currentTime > _dawnTimeFloat) {
						RenderSettings.ambientLight = Color.Lerp (nightAmbientLightColour, dayAmbientLightColour, (_currentTime - _dawnTimeFloat) / dawnLength);
				} else if (_currentTime > _duskTimeFloat) {
						RenderSettings.ambientLight = Color.Lerp (dayAmbientLightColour, nightAmbientLightColour, (_currentTime - _duskTimeFloat) / duskLength);
				}
		}
}
