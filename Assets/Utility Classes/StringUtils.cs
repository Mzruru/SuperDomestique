using UnityEngine;
using System.Collections;

public class StringUtils {

	public static float ConvertTimeStringToFloat (string timeString) {
		char[] splitchar = { ':' };
		string[] comps = timeString.Split(splitchar);
		int hours = 0;
		if (comps.Length > 0) int.TryParse(comps[0], out hours);
		int mins = 0;
		if (comps.Length > 1) int.TryParse(comps[1], out mins);
		int secs = 0;
		if (comps.Length > 2) int.TryParse(comps[2], out secs);
		
		float minutes = (hours * 60.0f) + mins + (secs / 60.0f);
		
		return minutes;
	}
	
	public static string ConvertTimeFloatToString (float timeFloat) {
		int hours = (int)(timeFloat / 60);
		int mins = (int)(timeFloat - (hours * 60));
		int secs = (int)((timeFloat - (hours * 60) - mins) * 60);
		
		string hoursString = hours < 10 ? "0" + hours : "" + hours;
		string minsString = mins < 10 ? "0" + mins : "" + mins;
		string secsString = secs < 10 ? "0" + secs : "" + secs;
		
		return hoursString + ":" + minsString + ":" + secsString;
	}
}
