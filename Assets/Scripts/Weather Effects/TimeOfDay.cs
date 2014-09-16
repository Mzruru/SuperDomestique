using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TimeOfDay : MonoBehaviour {

	[Tooltip("The time of day in format HH:MM:SS (or HH:MM), 24 hour clock")]public string timeOfDay = "00:00:00";
	[Tooltip("How fast time passes")] public float timeScale = 2.0f;

	private float _currentTime = 0.0f;
	
	// Use this for initialization
	void Start () {
		_currentTime = StringUtils.ConvertTimeStringToFloat(timeOfDay);
	}
	
	// Update is called once per frame
	void Update () {
		_currentTime = StringUtils.ConvertTimeStringToFloat(timeOfDay);
		_currentTime += Time.smoothDeltaTime * timeScale;
		if (_currentTime > 1440.0f) {
			_currentTime -= 1440.0f;
		}
		
		timeOfDay = StringUtils.ConvertTimeFloatToString(_currentTime);
	}
	
	public float currentTime
	{
		get { return _currentTime; }
		set
		{
			currentTime = value;
			timeOfDay = StringUtils.ConvertTimeFloatToString(_currentTime);
		}
	}	
}