using UnityEngine;
using System.Collections;

public class VertexColouriser : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void UpdateWithColour (Mesh mesh, Color32 newColour) {
		Color[] colours = new Color[mesh.vertices.Length];
		int i = 0;
		while (i < mesh.vertices.Length) {
			colours[i] = newColour;
			i++;
		}

		mesh.colors = colours;
	}

	public void UpdateWithColour (int[] targets, Color32 newColour) {
		Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
		Color[] colours = new Color[mesh.vertices.Length];
		int i = 0;
		while (i < mesh.vertices.Length) {
			foreach (int target in targets) {
				if (target == i) colours[i] = newColour;
				else colours[i] = mesh.colors[i];
				i++;
			}
		}
		
		mesh.colors = colours;
	}
}
