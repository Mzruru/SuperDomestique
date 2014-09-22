using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class TimedValueController : MonoBehaviour {

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
		foreach (TimedColourValue set in sets)
		{
			float setValue = set.GetValue(timeOfDay.currentTime);
			value += setValue;
		}
		
		if (value > 0)
		{
			UpdateValue(value);
		}
	}
		
	abstract protected void UpdateValue (float value);
}
