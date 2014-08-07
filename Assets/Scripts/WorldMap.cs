using UnityEngine;
using System.Collections;

[RequireComponent (typeof(VertexMapper))]
public class WorldMap : MonoBehaviour
{
	public GameObject player;
	public int playerX;
	public int playerZ;

	// maps
	public Texture2D typemap;
	public Texture2D typeSwatch;
	public Texture2D heightmap;
	public int swatchSize;

	// blocks
	public BlockType[] blocks;
	public float heightScale = 0.5f;

	// dimensions
	public float scale = 0.5f;
	public int width = 20;
	public int depth = 20;
	public int rotationMax = 45;

	// private
	GameObject[] blockMap;
	Mesh[] meshMap;
	Color[] heightValues;
	Color[] typeValues;
	Color[] typeSwatches;
	int mapWidth;
	int mapDepth;
	ArrayList pool;

	void Start ()
	{
		mapWidth = heightmap.width;
		mapDepth = heightmap.height;
		width = mapWidth;
		depth = mapDepth;
		heightValues = heightmap.GetPixels (0, 0, mapWidth, mapDepth);
		typeValues = typemap.GetPixels (0, 0, mapWidth, mapDepth);
		typeSwatches = GetSwatches (typeSwatch, blocks.Length);
		blockMap = new GameObject[width * depth];
		meshMap = new Mesh[width * depth];
		pool = new ArrayList ();

		Vector3 playerWorldPosition = MapCoordsToWorldCoords (playerX, playerZ, GetXOffset (), GetZOffset ());
		playerWorldPosition.y = player.transform.position.y;
		player.transform.position = playerWorldPosition;

		int xo = GetXOffset ();
		int zo = GetZOffset ();
		int xc = xo + width;
		int zc = zo + depth;
		GenerateBlocks (xo, zo, xc, zc);
	}

	void Update ()
	{
		UpdatePlayerPosition ();
	}

	// MAP GENERATION
	void GenerateBlocks (int startX, int startZ, int endX, int endZ)
	{
		for (int z = startZ; z < endZ; z++) {
			for (int x = startX; x < endX; x++) {
				int index = MapCoordsToBlockIndex (x, z, startX, startZ);
				BlockType type = GetType (x, z);
				GameObject block = blockMap [index];
				if (type != null) {
					if (block == null)
						block = GetBlockFromPoolOrCreate (type, x, z, index);
					PositionBlock (block, x, z, 0, 0);
					AssignMaterial (block, x, z);
					//UpdateSea (block, x, z);
					meshMap [index] = block.GetComponent<MeshFilter> ().mesh;
				} else if (block != null) {
					RecycleBlock (block);
					meshMap [index] = null;
				}

				blockMap [index] = block;
			}
		}
		InitMeshModifiers (startX, startZ, endX, endZ);

		//Combine ();
	}

	void Combine ()
	{
		MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter> ();
		ArrayList existingMeshes = new ArrayList ();
		foreach (MeshFilter meshFilter in meshFilters) {
			if (meshFilter.sharedMesh != null)
				existingMeshes.Add (meshFilter);
		}
		meshFilters = existingMeshes.ToArray (typeof(MeshFilter)) as MeshFilter[];

		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		int i = 0;
		int c = 0;
		while (i < meshFilters.Length) {
			if (meshFilters [i].sharedMesh != null) {
				combine [c].mesh = meshFilters [i].sharedMesh;
				combine [c].transform = meshFilters [i].transform.localToWorldMatrix;
				meshFilters [i].gameObject.SetActive (false);
				c++;
			}
			i++;
		}
		transform.GetComponent<MeshFilter> ().mesh = new Mesh ();
		transform.GetComponent<MeshFilter> ().mesh.CombineMeshes (combine);
		gameObject.SetActive (true);
	}
	
	void PositionBlock (GameObject block, int x, int z, int xOffset, int zOffset)
	{
		block.transform.position = MapCoordsToWorldCoords (x, z, xOffset, zOffset);
		if (rotationMax > 0) {
			block.transform.Rotate (new Vector3 (0, Random.Range (0, rotationMax), 0));
		}
	}
	
	void UpdateSea (GameObject block, int x, int z)
	{
		SeaMovement mov = block.GetComponent<SeaMovement> ();
		bool enable = block.name.Contains ("Water");
		mov.enabled = enable;
	}

	void AssignMaterial (GameObject block, int mapX, int mapZ)
	{
		if (block == null)
			return;
		BlockType type = GetType (mapX, mapZ);
		Mesh mesh = block.GetComponent<MeshFilter> ().sharedMesh;
		if (type == null || mesh == null)
			return; 
		VertexColouriser colouriser = block.GetComponent<VertexColouriser> ();
		colouriser.UpdateWithColour (mesh, type.colour);
	}

	// GETTERS
	
	int GetXOffset ()
	{
		int hW = Mathf.RoundToInt ((float)width / 2);
		int offset = playerX - hW;
		if (offset < 0)
			offset = 0;
		else if (offset > mapWidth)
			offset = mapWidth;
		
		return offset;
	}
	
	int GetZOffset ()
	{
		int hD = Mathf.RoundToInt ((float)depth / 2);
		int offset = playerZ - hD;
		if (offset < 0)
			offset = 0;
		else if (offset > mapDepth)
			offset = mapDepth;
		
		return offset;
	}
	
	Color[] GetSwatches (Texture2D swatch, int size)
	{
		int swatchWidth = swatch.width;
		Color[] swatches = new Color[size];
		int c = 0;
		for (int i = 0; i < swatchWidth; i+=swatchSize) {
			Color colour = swatch.GetPixel (i, 0);
			swatches [c++] = colour;
		}
		
		return swatches;
	}

	public BlockType GetType (int mapX, int mapZ)
	{
		if (mapX < 0 || mapZ < 0)
			return null;
		if (mapX >= mapWidth || mapZ >= mapDepth)
			return null;
		string colours = "";
		foreach (Color swatch in typeSwatches) {
			colours += " " + swatch;
		}

		Color mapColour = typeValues [MapCoordsToMapIndex (mapX, mapZ)];
		int c = 0;
		foreach (Color swatch in typeSwatches) {
			if (swatch == mapColour) {
				break;
			}
			c++;
		}

		if (c < typeSwatches.Length) {
			return blocks [c];
		}
		return null;
	}

	public float GetWorldHeight (int mapX, int mapZ)
	{
		int index = MapCoordsToMapIndex (mapX, mapZ);
		if (index < 0 || index >= heightValues.Length)
			return 0;
		Color mapColour = heightValues [index];
		return (float)(mapColour.b * heightScale);
	}

	public GameObject GetObjectForPosition (int mapX, int mapZ, int xOffset, int zOffset)
	{
		if (mapX < 0 || mapX >= width || mapZ < 0 || mapZ >= depth)
			return null;
		int index = MapCoordsToBlockIndex (mapX, mapZ, xOffset, zOffset);
		if (index < 0)
			return null;
		if (index >= blockMap.Length)
			return null;
		return blockMap [index];
	}
	
	public Mesh GetMeshForPosition (int mapX, int mapZ)
	{
		if (mapX < 0 || mapX >= width || mapZ < 0 || mapZ >= depth)
			return null;
		
		return meshMap [MapCoordsToBlockIndex (mapX, mapZ, GetXOffset (), GetZOffset ())];
	}

	// CONVERTORS
	int MapCoordsToMapIndex (int mapX, int mapZ)
	{
		return ((mapDepth - mapZ - 1) * mapWidth) + mapX;
	}
	
	Vector2 MapIndexToMapCoords (int index)
	{
		int mx = index % mapDepth;
		int mz = (index - mx) / mapWidth;
		return new Vector2 (mx, mz);  
	}
	
	int MapCoordsToBlockIndex (int mapX, int mapZ, int xOffset, int zOffset)
	{
		return ((mapZ - zOffset) * width) + (mapX - xOffset);
	}
	
	public Vector3 MapCoordsToWorldCoords (int mapX, int mapZ, int xOffset, int zOffset)
	{
		float hW = width / 2;
		float hD = depth / 2;
		float posX = (((float)(mapX - xOffset) - hW)) * scale;
		float posZ = (((float)(mapZ - zOffset) - hD)) * -scale;
		return new Vector3 (posX, GetWorldHeight (mapX, mapZ), posZ);
	}

	public Vector3 WorldCoordsToMapCoords (Vector3 worldCoords, int xOffset, int zOffset)
	{
		float hW = width / 2;
		float hD = depth / 2;
		int mapX = (int)((worldCoords.x / scale) + hW + xOffset + (scale / 2));
		int mapZ = (int)((worldCoords.z / -scale) + hD + zOffset + (scale / 2));

		return new Vector3 (mapX, worldCoords.y, mapZ);
	}
	
	// MODIFY MESH
	void InitMeshModifiers (int xo, int zo, int xc, int zc)
	{
		for (int z = zo; z < zc; z++) {
			for (int x = xo; x < xc; x++) {
				InitMeshModifier (x, z, GetXOffset (), GetZOffset ());
			}
		}
	}
	
	void InitMeshModifier (int mapX, int mapZ, int xOffset, int zOffset)
	{
		GameObject block = GetObjectForPosition (mapX, mapZ, xOffset, zOffset);
		if (block == null)
			return;
		MeshModder meshModder = block.GetComponent<MeshModder> ();
		meshModder.lb = GetModValues (mapX - 1, mapZ - 1, xOffset, zOffset);
		meshModder.b = GetModValues (mapX, mapZ - 1, xOffset, zOffset);
		meshModder.rb = GetModValues (mapX + 1, mapZ - 1, xOffset, zOffset);
		meshModder.l = GetModValues (mapX - 1, mapZ, xOffset, zOffset);
		meshModder.r = GetModValues (mapX + 1, mapZ, xOffset, zOffset);
		meshModder.lf = GetModValues (mapX - 1, mapZ + 1, xOffset, zOffset);
		meshModder.f = GetModValues (mapX, mapZ + 1, xOffset, zOffset);
		meshModder.rf = GetModValues (mapX + 1, mapZ + 1, xOffset, zOffset);

		meshModder.UpdateVertices ();
	}
	
	public MeshModValues GetModValues (int mapX, int mapZ, int xOffset, int zOffset)
	{
		GameObject obj = GetObjectForPosition (mapX, mapZ, xOffset, zOffset);
		if (obj) return new MeshModValues(true, obj.transform.position.y, IsWater(obj));
		else return new MeshModValues(false, 0f, false);
	}
	
	bool IsWater (GameObject block)
	{
		if (block == null)
			return false;
		return block.name.Contains ("Water");
	}

	// OBJECT POOLING
	void RecycleBlocks (int startX, int startZ, int endX, int endZ, int xOffset, int zOffset)
	{
		for (int z = startZ; z < endZ + 1; z++) {
			for (int x = startX; x < endX + 1; x++) {
				GameObject block = GetObjectForPosition (x, z, xOffset, zOffset);
				RecycleBlock (block);
			}
		}
	}

	void RecycleBlock (GameObject block)
	{
		if (block != null) {
			block.SetActive (false);
			pool.Add (block);
		}
	}

	GameObject GetBlockFromPoolOrCreate (BlockType type, int x, int z, int index)
	{
		if (blockMap [index] != null)
			return blockMap [index];
		if (type == null)
			return null;
		GameObject found = null;
		foreach (GameObject block in pool) {
			if (block.name.Contains (type.prefab.name)) {
				found = block;
				break;
			}
		}

		if (found != null) {
			found.SetActive (true);
			found.name = type.name;
			return found;
		}

		found = (GameObject)Instantiate (type.prefab, Vector3.zero, Quaternion.identity);
		found.transform.parent = gameObject.transform;
		found.name = type.name;

		return found;
	}
	
	int GetIndexForObject (GameObject block)
	{
		int c = blockMap.Length;
		for (int i = 0; i < c; i++) {
			GameObject o = blockMap [i];
			if (o == block)
				return i;
		}

		return -1;
	}

	// PLAYER
	public void UpdatePlayerPosition ()
	{
		int oldXOffset = GetXOffset ();
		int oldZOffset = GetZOffset ();

		// we find how far the player has moved since the start
		Vector3 playerMapPosition = WorldCoordsToMapCoords (player.transform.position, 0, 0);
		playerX = (int)playerMapPosition.x;
		playerZ = (int)playerMapPosition.z;

		int newXOffset = GetXOffset ();
		int newZOffset = GetZOffset ();

		if (newXOffset != oldXOffset ||
			newZOffset != oldZOffset) {

			// if positive move, recycle blocks on lower side
			// if negative move, recycle blocks on upper side
			// we need to recycle the blocks between the old position and the new position
			// if x was 0 and now x is 2, recycle between 0 and 1
			// if x was 10 and now x is 7, recycle between 10+w and 8+w
			/*
			int startX;
			int startZ;
			int endX;
			int endZ;
			if (newXOffset > oldXOffset) {
				startX = oldXOffset;
				endX = newXOffset - 1;
			} else if (newXOffset < oldXOffset) {
				startX = newXOffset + 1 + width;
				endX = oldXOffset + width;
			} else {
				startX = newXOffset;
				endX = newXOffset + width;
			}

			if (newZOffset > oldZOffset) {
				startZ = oldZOffset;
				endZ = newZOffset - 1;
			} else if (newZOffset < oldZOffset) {
				startZ = newZOffset + 1 + depth;
				endZ = oldZOffset + depth;
			} else {
				startZ = newZOffset;
				endZ = newZOffset + depth;
			}

			RecycleBlocks (startX, startZ, endX, endZ, oldXOffset, oldZOffset);
			int xo = GetXOffset();
			int zo = GetZOffset();
			GenerateBlocks(xo, zo, xo + width, zo + depth);
			*/
		}
	}
}
