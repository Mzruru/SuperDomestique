using UnityEngine;
using System.Collections;

public interface ICornerFinder {

	void Initialise();
	int[] GetCorner(int x, int y, int z);
	Vector3[] GetVertices();
	Mesh GetMesh();
	Vector3 GetTopCentrePoint (float weighting);
}
