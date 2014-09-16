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
		get { return StringUtils.ConvertTimeStringToFloat(end) - StringUtils.ConvertTimeStringToFloat(start); }
		set {
			endFloat = StringUtils.ConvertTimeStringToFloat(start) + value;
		}
	}
	
	public bool IsInRange (float time) {
		return time > StringUtils.ConvertTimeStringToFloat(start) && time < StringUtils.ConvertTimeStringToFloat(end);
	}
	
	public float Lerp (float time) {
		return Mathf.Lerp(StringUtils.ConvertTimeStringToFloat(start), StringUtils.ConvertTimeStringToFloat(end), time);
	}
}
