using UnityEngine;
using System.Collections;

public class CubeMeshModder : PrimitiveModder, IMeshModder {

	private Mesh mesh;
	private Vector3[]vertices;
	private int[][] corners;
	public float pointAdjustmentAmount = 0;
	
	float lbAdjust;
	float rbAdjust;
	float lfAdjust;
	float rfAdjust;

	void Awake () {
		Initialise();
	}
	
	public void Initialise () {
		mesh = gameObject.GetComponent<MeshFilter>().mesh;
		vertices = (Vector3[])mesh.vertices.Clone ();
		vals = new MeshModValues[8]; // 0:l, 1:lb, 2:b, 3:rb, 4:r, 5:rf, 6:f, 7:lf
		
		lbAdjust = Random.Range(-pointAdjustmentAmount, pointAdjustmentAmount);
		rbAdjust = Random.Range(-pointAdjustmentAmount, pointAdjustmentAmount);
		lfAdjust = Random.Range(-pointAdjustmentAmount, pointAdjustmentAmount);
		rfAdjust = Random.Range(-pointAdjustmentAmount, pointAdjustmentAmount);
		
		corners = new int[8][];
		corners[0] = FindCorners(vertices, -0.5f, 0.5f, -0.5f); // left top front -+-
		corners[1] = FindCorners(vertices, -0.5f, 0.5f, 0.5f); // left top back -++
		corners[2] = FindCorners(vertices, 0.5f, 0.5f, -0.5f); // right top front ++-
		corners[3] = FindCorners(vertices, 0.5f, 0.5f, 0.5f); // right top back +++

		corners[4] = FindCorners(vertices, -0.5f, -0.5f, -0.5f); // left bottom front ---
		corners[5] = FindCorners(vertices, -0.5f, -0.5f, 0.5f); // left bottom back --+
		corners[6] = FindCorners(vertices, 0.5f, -0.5f, -0.5f); // right bottom front +--
		corners[7] = FindCorners(vertices, 0.5f, -0.5f, 0.5f);// right bottom back +-+
	}

	public Vector3[] GetVertices () {
		return vertices;
	}

	public Mesh GetMesh () {
		return mesh;
	}

	public int[] GetCorner (int x, int y, int z) {

		x = x == -1 ? 0 : 2;
		y = (y - 1) * 2;
		z = z == -1 ? 0 : 1;

		return corners[x + y + z];
	}
	
	int[] FindCorners (Vector3[] vertices, float x, float y, float z) {
		int[] targetArray = new int[4] {-1, -1, -1, -1}; 
		int c = 0;
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 vertex = vertices[i];
			if (vertex.y == y)
			{
				if (vertex.x == x && vertex.z == z) 
				{
					targetArray[c++] = i;
				}
			}
		}
		return targetArray;
	}
	
	public void UpdateVertices ()
	{
		SetVerticesUp (vals[0], vals[1], vals[2], GetCorner (-1, 1, 1), lbAdjust);
		SetVerticesUp (vals[2], vals[3], vals[4], GetCorner (1, 1, 1), rbAdjust);
		SetVerticesUp (vals[6], vals[7], vals[0], GetCorner (-1, 1, -1), lfAdjust);
		SetVerticesUp (vals[4], vals[5], vals[6], GetCorner (1, 1, -1), rfAdjust);
		
		Mesh mesh = GetMesh();
		mesh.vertices = vertices;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
	}
	
	public void SetVerticesUp (MeshModValues obj1, MeshModValues obj2, MeshModValues obj3, int[] targets, float adjust)
	{	
		if (IsWater(gameObject)) {
			ResetVertices();
		} else {
			Modify(adjust, targets, obj1, obj2, obj3);
		}
	}

	public Vector3 GetTopCentrePoint (float weighting) {
		Vector3 front = (vertices[corners[0][0]] + vertices[corners[2][0]]) / 2;
		Vector3 back = (vertices[corners[1][0]] + vertices[corners[3][0]]) / 2;
		return back + ((back - front) * weighting);
	}
	
	public void Modify (float adjust, int[] targets, MeshModValues obj1, MeshModValues obj2, MeshModValues obj3) {
		float y = gameObject.transform.position.y;
		float yScale = gameObject.transform.localScale.y;
		
		float y1 = obj1.y;
		float y2 = obj2.y;
		float y3 = obj3.y;
		
		float cy = 0.0f;
		float c1y = 0.0f;
		float c2y = 0.0f;
		float c3y = 0.0f;
		
		c1y = y1 - y;
		c2y = y2 - y;
		c3y = y3 - y;
		
		bool nextToWater = obj1.isWater || obj2.isWater || obj3.isWater;
		
		cy = GetCY(nextToWater, yScale, c1y, c2y, c3y);
		cy /= yScale;
		if (ShouldModify(cy, nextToWater)) {
			foreach (int target in targets) {
				if (target > -1) {
					vertices [target].y = cy + adjust;
				}
			}
		} else {
			foreach (int target in targets) {
				if (target > -1) {
					vertices [target].y = 0.5f;
				}
			}
		}
	}
	
	public float GetCY(bool nextToWater, float yScale, float c1y, float c2y, float c3y) {
		if (nextToWater) return AverageMin(c1y, c2y, c3y) + (yScale / 2f);
		else return AverageMax(c1y, c2y, c3y) + (yScale / 2f);
	}
	
	public float GetCY(float c1y, float c2y, float c3y) {
		return AverageMax(c1y, c2y, c3y);
	}
	
	public float GetNextToWaterCY(float c1y, float c2y, float c3y) {
		return AverageMax(c1y, c2y, c3y);
	}
	
	public bool ShouldModify (float cy, bool nextToWater) {
		return cy > 0.5 || nextToWater;
	}
	
	public void ResetVertices () {
		ResetVertices(GetCorner (-1, 1, 1));
		ResetVertices(GetCorner (1, 1, 1));
		ResetVertices(GetCorner (-1, 1, -1));
		ResetVertices(GetCorner (1, 1, -1));
		Mesh mesh = GetMesh();
		mesh.vertices = vertices;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
	}
	
	public void ResetVertices(int[] targets) {
		foreach (int i in targets)
		{
			if (i > -1 && i < vertices.Length - 1) vertices[i].y = 0.5f;
		}
	}
	
	public Vector3 GetAverageOfTopNormal () {
		Vector3[] normals = GetComponent<MeshFilter>().mesh.normals;
		
		int c = 0;
		Vector3 avg = Vector3.zero;
		foreach (Vector3 normal in normals) {
			if (normal.y == 0 || normal.y < 0) continue;
			avg += normal;
			c++;
		}
		
		avg /= (float)c;
		
		return avg;
	}
}
