using UnityEngine;
using System.Collections;

public class QuadCornerFinder : MonoBehaviour, ICornerFinder {

	private Mesh mesh;
	private Vector3[]vertices;
	private int[][] corners;
	
	public void Initialise () {
		mesh = gameObject.GetComponent<MeshFilter>().mesh;
		vertices = (Vector3[])mesh.vertices.Clone ();
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
	
	public Vector3 GetTopCentrePoint (float weighting) {
		Vector3 front = (vertices[corners[0][0]] + vertices[corners[2][0]]) / 2;
		Vector3 back = (vertices[corners[1][0]] + vertices[corners[3][0]]) / 2;
		return back + ((back - front) * weighting);
	}
}
