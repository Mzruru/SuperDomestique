using UnityEngine;
using System.Collections;

[System.Serializable]
public class TimedColourRangeSet {

	public TimeRange timeRange;
	public DualColour start;
	public DualColour mid;
	public DualColour end;
	
	public TimedColourRangeSet (TimeRange timeRange, DualColour start, DualColour mid, DualColour end)
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
