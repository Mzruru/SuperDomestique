using UnityEngine;
using System.Collections;

public class PrimitiveModder : MonoBehaviour {

	[HideInInspector] public MeshModValues[] vals;
	
	public float Average (float c1y, float c2y, float c3y) {
		return ((c1y + c2y + c3y) / 3);
	}
	
	public float Max (float c1y, float c2y, float c3y) {
		return Mathf.Max(c1y, Mathf.Max(c2y, c3y));
	}
	
	public float Min (float c1y, float c2y, float c3y) {
		return Mathf.Min(c1y, Mathf.Min(c2y, c3y));
	}
	
	public float AverageMax (float c1y, float c2y, float c3y) {
		float max = Max(c1y, c2y, c3y);
		float avg = (c1y + c2y + c3y) / 3;
		float val = max > avg ? max : avg;
		return val;
	}
	
	public float AverageMin  (float c1y, float c2y, float c3y) {
		float min = Min(c1y, c2y, c3y);
		float avg = (c1y + c2y + c3y) / 3;
		float val = min > avg ? avg : min;
		return val;
	}
	
	
	public bool IsWater (GameObject block)
	{
		if (block == null)
			return false;
		return block.name.Contains ("Water");
	}
	
	public MeshModValues[] GetVals () {
		return vals;
	}
	
	public void SetVals (MeshModValues[] newVals) {
		vals = newVals;
	}
}
