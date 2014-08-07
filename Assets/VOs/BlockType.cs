using UnityEngine;
using System.Collections;

[System.Serializable]
public class BlockType {
	public string name;
	public GameObject prefab;
	public Color32 colour = Color.grey;
	public bool canMerge = true;
}
