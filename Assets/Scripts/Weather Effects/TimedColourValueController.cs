using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public abstract class TimedColourValueController : MonoBehaviour {

	public TimeOfDay timeOfDay;
	public List<TimedColourValue> sets;
	
	// Use this for initialization
	void Start () {
		if (!timeOfDay) {
			Debug.Log (this.GetType() + ": you must assign a GameObject with the timeOfDay behaviour");
			enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		float value = 0;
		Color colour = new Color(0, 0, 0, 0);
		foreach (TimedColourValue set in sets)
		{
			float setValue = set.GetValue(timeOfDay.currentTime);
			value += setValue;
			colour += set.colour * setValue;
		}
		
		if (value > 0)
		{
			colour /= value;
			UpdateValues(colour, value);
		}
	}
	
	abstract protected void UpdateValues(Color colour, float value);
}
