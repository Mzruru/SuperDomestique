using UnityEngine;
using System.Collections;

[System.Serializable]
public class TimeRange {
	
	[SerializeField]
	string _start = "00:00:00";
	[SerializeField]
	string _end = "00:00:00";
	
	public TimeRange(string start, string end)
	{
		_start = start;
		_end = end;
	}
	
	public float startFloat
	{
		get { return StringUtils.ConvertTimeStringToFloat(start); }
		set {
			_start = StringUtils.ConvertTimeFloatToString(value);
		}
	}
	
	public float endFloat
	{
		get { return StringUtils.ConvertTimeStringToFloat(end); }
		set {
			_end = StringUtils.ConvertTimeFloatToString(value);
		}
	}
	
	public string start
	{
		get { return _start; }
		set {
			_start = value;
		}
	}
	
	public string end
	{
		get { return _end; }
		set {
			_end = value;
		}
	}
	
	public float length
	{
		get { return endFloat - startFloat; }
		set {
			endFloat = startFloat + value;
		}
	}
	
	public bool IsInRange (float time) {
		return time >= startFloat && time <= endFloat;
	}
	
	public float Lerp (float value) {
		return Mathf.Lerp(startFloat, endFloat, value);
	}
	
	public float InverseLerp (float time) {
		return Mathf.InverseLerp(startFloat, endFloat, time);
	}
}
