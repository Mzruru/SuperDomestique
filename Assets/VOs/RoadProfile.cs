using UnityEngine;
using System.Collections;

[System.Serializable]
public class RoadProfile {
	public string name;
	public string[] profile;
	public float bumpiness = 1;
	public float[] relativeHeights;
	public string[] outputProfiles;
	public float[] outputWeights;

	public string ChooseOutputProfile () {
		float totalWeight = 0;
		foreach (float weight in outputWeights) {
			totalWeight += weight;
		}
		float choice = Random.Range(0, totalWeight);
		float currentWeight = 0;
		for (int i = 0; i < outputProfiles.Length; i++) {
			currentWeight += outputWeights[i];
			if (choice <= currentWeight) return outputProfiles[i];
		}
		
		return null;
	}
}
