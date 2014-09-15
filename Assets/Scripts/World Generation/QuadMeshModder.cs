using UnityEngine;
using System.Collections;

public class QuadMeshModder : PrimitiveModder, IMeshModder {

	private Mesh mesh;
	private Vector3[]vertices;
	private int[][] corners;
	
	void Awake () {
		Initialise();
	}
	
	public void Initialise () {
		mesh = gameObject.GetComponent<MeshFilter>().mesh;
		vertices = (Vector3[])mesh.vertices.Clone ();
		vals = new MeshModValues[8]; // 0:l, 1:lb, 2:b, 3:rb, 4:r, 5:rf, 6:f, 7:lf
		
		corners = new int[4][];
		corners[0] = FindCorners(vertices, -0.5f, -0.5f, 0); // left top front -+-
		corners[1] = FindCorners(vertices, -0.5f, 0.5f, 0); // left top back -++
		corners[2] = FindCorners(vertices, 0.5f, -0.5f, 0); // right top front ++-
		corners[3] = FindCorners(vertices, 0.5f, 0.5f, 0); // right top back +++
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
	
	public void UpdateVertices ()
	{	
		SetVerticesUp (vals[6], vals[7], vals[0], GetCorner (-1, 1, -1));
		SetVerticesUp (vals[4], vals[5], vals[6], GetCorner (1, 1, -1));
		
		Mesh mesh = GetMesh();
		mesh.vertices = vertices;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
	}
	
	public void SetVerticesUp (MeshModValues obj1, MeshModValues obj2, MeshModValues obj3, int[] targets)
	{	
		if (IsWater(gameObject)) {
			ResetVertices();
		} else {
			Modify(targets, obj1, obj2, obj3);
		}
	}
	
	int[] FindCorners (Vector3[] vertices, float x, float y, float z) {
		int[] targetArray = new int[4] {-1, -1, -1, -1}; 
		int c = 0;
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 vertex = vertices[i];
			if (Compare(vertex.x, x) && Compare(vertex.y, y) && Compare(vertex.z, z)) {
				targetArray[c++] = i;
			}
		}
		return targetArray;
	}

	public bool Compare (float actual, float target) {
		return Mathf.Abs(actual - target) < 0.001; 
	}
	
	public Vector3 GetPointOnTopFace (float weightingX, float weightingZ) {
		Vector3 leftFront = vertices[corners[0][0]];
		Vector3 leftBack = vertices[corners[1][0]];
		Vector3 rightFront = vertices[corners[2][0]];
		Vector3 rightBack = vertices[corners[3][0]];
		
		Vector3 front = leftFront + ((rightFront - leftFront) * weightingX);
		Vector3 back = leftBack + ((rightBack - leftFront) * weightingX);
		Vector3 left = leftFront + ((leftBack - leftFront) * weightingZ);
		Vector3 right = rightFront + ((rightBack - rightFront) * weightingZ);
		
		return (front + back + left + right) / 4;
	}
	
	public void Modify (int[] targets, MeshModValues obj1, MeshModValues obj2, MeshModValues obj3) {
		float y = gameObject.transform.position.y;
		
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
			
		cy = GetCY(nextToWater, c1y, c2y, c3y);
		foreach (int target in targets) {
			if (target > -1) {
				vertices [target].z = -cy;
			}
		}
	}
	
	public float GetCY(bool nextToWater, float c1y, float c2y, float c3y) {
		if (nextToWater) return AverageMin(c1y, c2y, c3y);
		else return AverageMax(c1y, c2y, c3y);
	}
	
	public float GetNextToWaterCY(float c1y, float c2y, float c3y) {
		return AverageMax(c1y, c2y, c3y);
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
			if (i > -1 && i < vertices.Length - 1) vertices[i].z = 0f;
		}
	}
	
	public Vector3 GetAverageOfTopNormal () {
		Vector3[] normals = GetComponent<MeshFilter>().mesh.normals;
		
		int c = 0;
		Vector3 avg = Vector3.zero;
		foreach (Vector3 normal in normals) {
			if (normal.z == 0 || normal.z > 0) continue;
			avg += normal;
			c++;
		}
		
		avg /= (float)c;
		avg = Quaternion.Euler(90, 0, 0) * avg;
		print (avg);
		return avg;
	}
}
