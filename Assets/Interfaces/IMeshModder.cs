﻿using UnityEngine;
using System.Collections;

public interface IMeshModder {

	void Initialise();
	int[] GetCorner(int x, int y, int z);
	Vector3[] GetVertices();
	Mesh GetMesh();
	Vector3 GetTopCentrePoint (float weighting);
	void ResetVertices();
	void ResetVertices(int[] targets);
	void UpdateVertices ();
	MeshModValues[] GetVals ();
	void SetVals (MeshModValues[] newVals);
	Vector3 GetAverageOfTopNormal ();
}