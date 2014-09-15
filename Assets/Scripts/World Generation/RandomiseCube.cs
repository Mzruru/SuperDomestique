using UnityEngine;
using System.Collections;

public class RandomiseCube : MonoBehaviour {

	public GameObject cube1;
	public GameObject cube2;

	// Use this for initialization
	void Start () {
		MakeRandom(cube1);
		MakeRandom(cube2);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void MakeRandom (GameObject target) {
		Mesh mesh = target.GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = (Vector3[])mesh.vertices.Clone();
		setVerticesForBackLeft(vertices, 0.7f);
		mesh.vertices = vertices;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}

	void setVerticesForBackLeft (Vector3[] vertices, float y) {
		vertices[3].y = y;
		vertices[9].y = y;
		vertices[19].y = y;
	}
}
