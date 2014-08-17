using UnityEngine;
using System.Collections;

public class MeshModTest : MonoBehaviour {
	
	public GameObject[] road;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		MeshModValues[] vals = new MeshModValues[8];
		
		for (int i = 0; i < road.Length; i++)
		{
			GameObject curr = road[i];
			IMeshModder modder = (IMeshModder)curr.GetComponent(typeof(IMeshModder));
			
			GameObject prev = null;
			GameObject next = null;
			if (i > 0) prev = road[i - 1];
			if (i < road.Length - 1) next = road[i + 1];
			
			for (int j = 0; j < 8; j++)
			{
				
				MeshModValues val;
				if (j == 6 && prev != null) {
					val = new MeshModValues(true, prev.transform.position.y, false);
				} else if (j == 2 && next != null) {
					val = new MeshModValues(true, next.transform.position.y, false);
				} else {
					val = new MeshModValues(false, 0, false);
				}
				vals[j] = val;
			}
			
			modder.SetVals(vals);
			modder.UpdateVertices();
		}
	}
}
