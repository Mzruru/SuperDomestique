using UnityEngine;
using System.Collections;

[System.Serializable]
public class HillSection
{
	private string[] randomValues = {
		"hillType",
		"changeRising",
		"inOffset",
		"outOffset",
		"hillHeight"
	};
	public int modIndex;
	public bool isFlat;
	public bool isRising;
	public float hillType;
	public float inOffset;
	public float outOffset;
	public float hillHeight;
	public float hillHeightScalar;
	public Bezier bezier;
	public int startBlock;
	public int numberOfBlocks;

	public HillSection (SeededRandomiser randomiser, int index, float startHeight, bool wasRising, float hilliness, float mountaneousness, float normalHillHeight, float maxHillHeight, float flatSectionDivisor)
	{
		modIndex = index * randomValues.Length;
		hillType = randomiser.GetRandomForN (GetRandomIndexForKey("hillType"));

		float changeRising = randomiser.GetRandomForN (GetRandomIndexForKey("changeRising"));
		float changeMultiplier = 1 - (startHeight / maxHillHeight);
		if (changeMultiplier > 1) changeMultiplier = 1;
		if (changeMultiplier < 0) changeMultiplier = 0;
		if (changeRising > (mountaneousness * changeMultiplier)) isRising = false;
		else isRising = true;
		
		bool sharp = false;
		isFlat = false;
		if (hillType > hilliness) {
			hillHeightScalar = normalHillHeight / flatSectionDivisor;
			isFlat = true;
		} else {
			float v = hilliness / 4f;
			hillHeightScalar = normalHillHeight;
			sharp = (hillType >= v && hillType < (v * 2)) || (hillType >= (v * 3));
		}
		
		if (sharp) {
			inOffset = randomiser.GetRandomFromRangeForN (0.3f, 0.6f, GetRandomIndexForKey("inOffset"));
			outOffset = randomiser.GetRandomFromRangeForN (0.4f, 0.7f, GetRandomIndexForKey("outOffset"));
		} else {
			inOffset = randomiser.GetRandomFromRangeForN (0.1f, 0.5f, GetRandomIndexForKey("inOffset"));
			outOffset = randomiser.GetRandomFromRangeForN (0.5f, 0.9f, GetRandomIndexForKey("outOffset"));
		}
		
		float half = (hillHeightScalar / 2);
		float change = index > -1 ? (randomiser.GetRandomForN (GetRandomIndexForKey("hillHeight")) * half) + half : 0;
		
		if (!isRising && startHeight - change  < 0) isRising = true;
		if (isRising) hillHeight = startHeight + change;
		else hillHeight = startHeight - change;
	}
	
	int GetRandomIndexForKey (string key) {
		int i = 0;
		foreach (string check in randomValues) {
			if (check == key) return modIndex + i;
			i++;
		}
		return modIndex;
	}
	
	public float GetYForBlockIndex (int blockIndex)
	{
		float t = ((float)(blockIndex - startBlock)) / (float)numberOfBlocks;
		return bezier.GetPointAtTime (t).y;
	}
}

