using UnityEngine;
using System.Collections;

[System.Serializable]
public class MeshModValues {

	public bool exists;
	public float y;
	public bool isWater;

	public MeshModValues (bool exists, float y, bool isWater) {
		this.exists = exists;
		this.y = y;
		this.isWater = isWater; 
	}
}
