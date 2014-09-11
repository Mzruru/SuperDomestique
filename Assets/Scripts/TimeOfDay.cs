using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TimeOfDay : MonoBehaviour {

	
	[Tooltip("The number of minutes since midnight")]public float _currentTime = 0.0f;
	[Tooltip("How fast time passes")] public float timeScale = 2.0f;
	[Tooltip("An object which has a material that uses GradientSky shader")] public GameObject timeTarget;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		_currentTime += Time.deltaTime * timeScale;
		if (_currentTime > 1440.0f) {
			_currentTime -= 1440.0f;
		}

		timeTarget.renderer.sharedMaterial.SetFloat ("_CurrentTime", _currentTime);
	}
}