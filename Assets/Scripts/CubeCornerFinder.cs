using UnityEngine;
using System.Collections;

public class CubeCornerFinder : MonoBehaviour, ICornerFinder {

	private Mesh mesh;
	private Vector3[]vertices;
	private int[][] corners;

	public void Initialise () {
		mesh = gameObject.GetComponent<MeshFilter>().mesh;
		vertices = (Vector3[])mesh.vertices.Clone ();
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

	public Vector3 GetTopCentrePoint (float weighting) {
		Vector3 front = (vertices[corners[0][0]] + vertices[corners[2][0]]) / 2;
		Vector3 back = (vertices[corners[1][0]] + vertices[corners[3][0]]) / 2;
		return back + ((back - front) * weighting);
	}
}
