using UnityEngine;
using System.Collections;

public class VertexMapper : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void MapVertices (GameObject block, BlockType type) {
		if (block == null) return;
		Mesh mesh = block.GetComponent<MeshFilter>().sharedMesh;
		VertexColouriser colouriser = block.GetComponent<VertexColouriser>();
		colouriser.UpdateWithColour(mesh, type.colour);
	}

	public BlockType GetTypeForBlocks (BlockType[] types) {
		if (types[0] != null && !types[0].canMerge) return types[0];

		Hashtable votes = new Hashtable();
		for (int i = 0; i < types.Length; i++){
			BlockType type = types[i];
			if (type == null) continue;
			if (votes[type] == null) {
				votes[type] = 1;
			} else {
				votes[type] = (int)votes[type] + 1;
			}
		}

		int max = 0;
		ArrayList maxBlocks = null;
		ICollection keys = votes.Keys;
		foreach (BlockType key in keys){
			int val = (int)votes[key];
			if (val > max) {
				maxBlocks = new ArrayList();
				maxBlocks.Add(key);
				max = val;
			} else if (val == max) {
				maxBlocks.Add(key);
			}
		}
	
		if (maxBlocks == null) return null;
		if (maxBlocks.Count == 0) return null;
		else if (maxBlocks.Count == 1) return (BlockType)maxBlocks[0];
		foreach (BlockType block in maxBlocks) {
			if (block == types[0]) return block;
		}

		return (BlockType)maxBlocks[0];
	}
}
