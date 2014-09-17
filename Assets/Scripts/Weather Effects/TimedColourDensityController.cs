using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public abstract class TimedColourDensityController : MonoBehaviour {

	public GameObject timeController;
	public List<TimedColourValue> sets;
	
	TimeOfDay timeOfDay;
	
	// Use this for initialization
	void Start () {
		if (!timeController) return;
		timeOfDay = timeController.GetComponent<TimeOfDay> ();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (timeOfDay == null) return;
		
		float value = 0;
		Color colour = new Color(0, 0, 0, 0);
		foreach (TimedColourValue set in sets)
		{
			float setValue = set.GetValue(timeOfDay.currentTime);
			value += setValue;
			colour += set.colour * setValue;
		}
		
		colour /= value;
		UpdateValues(colour, value);
	}
	
	abstract protected void UpdateValues (Color colour, float value);
}
