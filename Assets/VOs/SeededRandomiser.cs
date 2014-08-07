using UnityEngine;
using System.Collections;

[System.Serializable]
public class SeededRandomiser
{
	public int seed;

	public SeededRandomiser (int seed) {
		this.seed = seed;
	}

	public float GetRandomForN (int n) {
		Random.seed = n * seed;
		return Random.value;
	}
	
	public float GetRandomFromRangeForN (float min, float max, int n) {
		Random.seed = n * seed;
		return Random.Range(min, max); 
	}
}