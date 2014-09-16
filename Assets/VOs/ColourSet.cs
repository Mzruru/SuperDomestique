using UnityEngine;
using System.Collections;

[System.Serializable]
public class ColourSet {

	public TimeRange timeRange;
	public ColourRange start;
	public ColourRange mid;
	public ColourRange end;
	
	public ColourSet (TimeRange timeRange, ColourRange start, ColourRange mid, ColourRange end)
	{
		this.timeRange = timeRange;
		this.start = start;
		this.mid = mid;
		this.end = end;
	}
	
	public bool IsInRange (float time)
	{
		return timeRange.IsInRange(time);
	}
}
