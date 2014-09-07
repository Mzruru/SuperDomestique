using UnityEngine;
using System.Collections;

public class HillGenerator : MonoBehaviour
{
	[Tooltip("The seed to generate this road from")] public int seed = 1202458724;
	[Tooltip("How many blocks to render at a time")]public int visibleLength = 100;
	[Tooltip("The type of blocks to use")]public BlockType[] blockTypes;
	[Tooltip("The road cross section")]public RoadProfile[] profiles;
	[Tooltip("The width of the road (all blocks)")]public float roadWidth = 10;
	[Tooltip("The max rise/fall for each hill section")]public float risePerHill = 40;
	[Tooltip("The maximum height of the road")]public float maxHillHeight = 200;
	[Tooltip("A divisor for the risePerHill for flat sections")]public float flatness = 10;
	[Tooltip("How likely hill sections are")]public float hilliness = 0.75f;
	[Tooltip("How likely the road is to rise")]public float mountaneousness = 0.25f;
	[Tooltip("The minimum number of blocks in a hill section")]public int minHillBlocks = 100;
	[Tooltip("The maximum number of blocks in a hill section")]public int maxHillBlocks = 200;
	[Tooltip("The minimum number of blocks in a flat section")]public int minFlatBlocks = 50;
	[Tooltip("The maximum number of blocks in a flat section")]public int maxFlatBlocks = 100;
	[Tooltip("The total number of blocks in the road")]public int totalBlocks = 5000;
	[Tooltip("The standard texture")]public Texture textureA;
	[Tooltip("The texture applied every textureStep blocks")]public Texture textureB;
	[Tooltip("How often to apply texture b")]public int textureStep = 5;
	
	SeededRandomiser rndmsr;
	[HideInInspector] public int xOffset = 0;
	[HideInInspector] public int currentWidth;
	[HideInInspector] public float offset = 0;
	RoadProfile currentProfile;
	int currentGeneratorCount = 0;
	GameObject[] blockMap;
	Mesh[] meshMap;
	ArrayList blockPool;
	ArrayList hillSections;
	protected Vector3 move = Vector3.zero;

	// Lifecycle

	// Use this for initialization
	void Start ()
	{
		rndmsr = new SeededRandomiser (seed);
		SetCurrentProfile (0);
		int l = (int)(visibleLength * currentWidth);
		blockMap = new GameObject[l];
		blockPool = new ArrayList ();

		GenerateSections ();
		GenerateBlocks (0, 0, visibleLength);
	}

	void GenerateSections ()
	{
		hillSections = new ArrayList ();
		int blocksRemaining = totalBlocks;
		int i = 0;
		int start = 0;
		while (blocksRemaining > 0) {
			int length = GenerateHillSection (i++, start);
			blocksRemaining -= length;
			start += length;
		}
	}

	HillSection GetSectionForBlockIndex (int index)
	{
		foreach (HillSection section in hillSections) {
			if (index >= section.startBlock && index < section.startBlock + section.numberOfBlocks) return section;
		}

		return null;
	}

	// Update is called once per frame
	void Update ()
	{
	}

	public void Move (float amount)
	{
		Vector3 m = Vector3.zero;
		m.x = amount;
		gameObject.transform.Translate (m);
		UpdateXOffset ();
	}

	int GenerateHillSection (int index, int startBlock)
	{
		HillSection previousHill = index > 0 ? (HillSection)hillSections[index - 1] : new HillSection (rndmsr, index - 1, 0, false, hilliness, mountaneousness, risePerHill, maxHillHeight, flatness);
		HillSection currentHill = new HillSection (rndmsr, index, previousHill.hillHeight, previousHill.isRising, hilliness, mountaneousness, risePerHill, maxHillHeight, flatness);

		currentHill.bezier = new Bezier (new Vector2 (0, previousHill.hillHeight),
		                           new Vector2 (currentHill.inOffset, previousHill.hillHeight),
		                           new Vector2 (1 - currentHill.outOffset, currentHill.hillHeight),
		                           new Vector2 (1, currentHill.hillHeight));
		                           
		currentHill.startBlock = startBlock;
		float min = currentHill.isFlat ? minFlatBlocks : minHillBlocks;
		float max = currentHill.isFlat ? maxFlatBlocks : maxHillBlocks;
		int length = Mathf.FloorToInt (rndmsr.GetRandomFromRangeForN (min, max, seed + index));
		currentHill.numberOfBlocks = length;
		hillSections.Add (currentHill);
		
		return length;
	}

	void UpdateXOffset ()
	{
		offset = gameObject.transform.position.x;
		int oldXOffset = xOffset;
		int newXOffset = Mathf.FloorToInt(offset);
		int dif = newXOffset - oldXOffset;
		if (dif < 0) {
			for (int i = 0; i < -dif; i++) {
				ShiftForwardsOnRoad ();
			}
		}
	}

	// Road Creation

	void SetCurrentProfile (int index)
	{
		currentProfile = profiles [index];
		currentWidth = currentProfile.profile.Length;
	}

	void GenerateBlocks (int generatorCount, int start, int end)
	{
		for (int x = start; x < end; x++) {
			for (int z = 0; z < currentWidth; z++) {
				string blockName = currentProfile.profile [z];
				float y = GetYForMapPosition (x, z, xOffset);
				GameObject block = GetBlockFromPoolOrCreate (blockName);
				block.renderer.material.mainTexture = x % textureStep == 0 ? textureB : textureA;
				PositionBlock (block, x, y, z, xOffset);
				int index = GetBlockIndexForMapCoords (x, z, xOffset);
				blockMap [index] = block;
				//Colourise (block, blockName);
			}
		}

		InitMeshModifiers (0, start, currentWidth, end);
		//PositionBlocks();
	}

	void PositionBlocks ()
	{
		float zPerBlock = roadWidth / currentProfile.profile.Length;
		for (int x = 0; x < visibleLength; x++) {
			for (int z = 0; z < currentWidth; z++) {
				GameObject block = blockMap [(x * currentWidth) + z];
				Vector3 position = Vector3.zero;
				float y = GetYForMapPosition (x, z, xOffset);
				position.x = (x - xOffset) + offset;
				position.y = y;
				position.z = z * zPerBlock;
				
				block.transform.localScale = new Vector3 (1, 1, zPerBlock);
				block.transform.position = position;
			}
		}
	}

	void PositionBlock (GameObject block, int x, float y, int z, int zOffset)
	{
		float zPerBlock = roadWidth / currentProfile.profile.Length;
		Vector3 position = Vector3.zero;
		position.x = x;
		position.y = y;
		position.z = z * zPerBlock;

		block.transform.localScale = new Vector3 (1, 1, zPerBlock);
		block.transform.localPosition = position;
	}

	// Getters

	int GetBlockIndexForMapCoords (int mapX, int mapZ, int xOffset)
	{
		return (int)(((mapX + xOffset) * currentWidth) + mapZ);
	}

	public GameObject GetObjectForPosition (int mapX, int mapZ, int xOffset)
	{
		if (mapZ < 0 || mapZ >= currentWidth || mapX < 0 || mapX + xOffset >= visibleLength)
			return null;
		int index = GetBlockIndexForMapCoords (mapX, mapZ, xOffset);
		if (index < 0)
			return null;
		if (index >= blockMap.Length)
			return null;
		return blockMap [index];
	}
	
	GameObject GetPrefabByName (string name)
	{
		foreach (BlockType blockType in blockTypes) {
			if (blockType.name.Equals (name))
				return blockType.prefab;
		}
		
		return null; 
	}

	public BlockType GetBlockTypeByName (string name)
	{
		foreach (BlockType type in blockTypes) {
			if (type.name.Equals (name))
				return type;
		}

		return null;
	}

	// Mesh Modding

	void Colourise (GameObject block, string blockName)
	{
		if (block == null)
			return;
		BlockType type = GetBlockTypeByName (blockName);
		Mesh mesh = block.GetComponent<MeshFilter> ().sharedMesh;
		if (type == null || mesh == null)
			return; 
		VertexColouriser colouriser = block.GetComponent<VertexColouriser> ();
		colouriser.UpdateWithColour (mesh, type.colour);
	}

	void InitMeshModifiers (int zs, int xs, int ze, int xe)
	{
		for (int z = zs; z < ze; z++) {
			for (int x = xs; x < xe; x++) {
				InitMeshModifier (x, z, xOffset);
			}
		}
	}

	void InitMeshModifier (int mapX, int mapZ, int xOffset)
	{
		GameObject block = GetObjectForPosition (mapX, mapZ, xOffset);
		if (block == null)
			return;
		
		IMeshModder meshModder = (IMeshModder)block.GetComponent (typeof(IMeshModder));
		// 0:l, 1:lb, 2:b, 3:rb, 4:r, 5:rf, 6:f, 7:lf
		MeshModValues[] vals = {GetModValues (mapX - 1, mapZ, xOffset),
			GetModValues (mapX - 1, mapZ + 1, xOffset),
			GetModValues (mapX, mapZ + 1, xOffset),
			GetModValues (mapX + 1, mapZ + 1, xOffset),
			GetModValues (mapX + 1, mapZ, xOffset),
			GetModValues (mapX + 1, mapZ - 1, xOffset),
			GetModValues (mapX, mapZ - 1, xOffset),
			GetModValues (mapX - 1, mapZ - 1, xOffset)};

		meshModder.SetVals(vals);
		
		meshModder.UpdateVertices ();
	}

	public MeshModValues GetModValues (int mapX, int mapZ, int xOffset)
	{
		float y = mapZ > -1 && mapZ < currentProfile.profile.Length ? GetYForMapPosition (mapX, mapZ, xOffset) : 0.0f;
		return new MeshModValues (true, y, false);
	}
	
	bool IsWater (GameObject block)
	{
		if (block == null)
			return false;
		return block.name.Contains ("Water");
	}

	float GetYForMapPosition (int mapX, int mapZ, int zOffset)
	{
		if (mapX > totalBlocks) mapX = totalBlocks;
		else if (mapX < 0) mapX = 0;
		
		HillSection section = GetSectionForBlockIndex (mapX);
		if (section == null) return currentProfile.relativeHeights [mapZ];
		float h = section.GetYForBlockIndex (mapX);
		return h + currentProfile.relativeHeights [mapZ];
	}
	
	// Object Pooling

	GameObject GetBlockFromPoolOrCreate (string prefabType)
	{
		GameObject found = null;
		if (blockPool.Count > 0) {
			int i = 0;
			for (i = 0; i < blockPool.Count; i++) {
				GameObject block = (GameObject)blockPool [i];
				if (block.name.Equals (prefabType)) {
					found = block;
					break;
				}
			}
			if (found != null) {
				blockPool.RemoveAt (i);
			}
		}

		if (found == null) {
			GameObject prefab = GetPrefabByName (prefabType);
			if (prefab == null)
				return null;
			found = (GameObject)Instantiate (prefab, Vector3.zero, Quaternion.identity);
			found.name = prefabType;
			found.transform.parent = gameObject.transform;
		}

		if (found.GetComponent<QuadMeshModder>() != null)
		{
			found.transform.eulerAngles = new Vector3(0, 0, 90);	
		}
		
		found.SetActive (true);

		return found;
	}

	void ShiftForwardsOnRoad ()
	{
		xOffset--;
		for (int i = 0; i < blockMap.Length; i++) {
			if (i < currentWidth) {
				RecycleBlock (blockMap [i]);
			} else {
				blockMap [i - currentWidth] = blockMap [i];
			}
		}

		int start = visibleLength - xOffset - 1;
		GenerateBlocks (currentGeneratorCount, start, start + 1);
	}

	void RecycleBlock (GameObject block)
	{
		if (block == null) return;
		if (block.activeSelf == false)
			return;
		block.SetActive (false);
		blockPool.Add (block);
	}
}
