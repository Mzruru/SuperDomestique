using UnityEngine;
using System.Collections;

public class MeshModder : MonoBehaviour {

	ICornerFinder finder;
	float yScale;
	float lbAdjust;
	float rbAdjust;
	float lfAdjust;
	float rfAdjust;

	public float pointAdjustmentAmount = 0;
	[HideInInspector] public MeshModValues l = null;
	[HideInInspector] public MeshModValues lb = null;
	[HideInInspector] public MeshModValues b = null;
	[HideInInspector] public MeshModValues rb = null;
	[HideInInspector] public MeshModValues r = null;
	[HideInInspector] public MeshModValues rf = null;
	[HideInInspector] public MeshModValues f = null;
	[HideInInspector] public MeshModValues lf = null;
	
	// Use this for initialization
	void Awake () {
		finder = gameObject.GetComponent<CubeCornerFinder>();
		if (finder == null) finder = gameObject.GetComponent<QuadCornerFinder>();
		finder.Initialise();

		lbAdjust = Random.Range(-pointAdjustmentAmount, pointAdjustmentAmount);
		rbAdjust = Random.Range(-pointAdjustmentAmount, pointAdjustmentAmount);
		lfAdjust = Random.Range(-pointAdjustmentAmount, pointAdjustmentAmount);
		rfAdjust = Random.Range(-pointAdjustmentAmount, pointAdjustmentAmount);
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void UpdateVertices () {
		float y = gameObject.transform.position.y;
		yScale = gameObject.transform.localScale.y;
		Vector3[] vertices = finder.GetVertices();
		SetVerticesUp (vertices, y, l, lb, b, finder.GetCorner (-1, 1, 1), lbAdjust);
		SetVerticesUp (vertices, y, b, rb, r, finder.GetCorner (1, 1, 1), rbAdjust);
		SetVerticesUp (vertices, y, f, lf, l, finder.GetCorner (-1, 1, -1), lfAdjust);
		SetVerticesUp (vertices, y, r, rf, f, finder.GetCorner (1, 1, -1), rfAdjust);

		Mesh mesh = finder.GetMesh();
		mesh.vertices = vertices;
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
	}

	public void ResetVertices () {
		ResetVertices(finder.GetCorner (-1, 1, 1));
		ResetVertices(finder.GetCorner (1, 1, 1));
		ResetVertices(finder.GetCorner (-1, 1, -1));
		ResetVertices(finder.GetCorner (1, 1, -1));
		Mesh mesh = finder.GetMesh();
		mesh.vertices = finder.GetVertices();
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
	}

	void ResetVertices(int[] targets) {
		Vector3[] vertices = finder.GetVertices();
		foreach (int i in targets)
		{
			if (i > -1 && i < vertices.Length - 1) vertices[i].y = 0.5f;
		}
	}

	void SetVerticesUp (Vector3[] vertices, float y, MeshModValues obj1, MeshModValues obj2, MeshModValues obj3, int[] targets, float adjust)
	{
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

		if (IsWater(gameObject)) {
			ResetVertices();
		} else {
			bool nextToWater = obj1.isWater || obj2.isWater || obj3.isWater;

			if (nextToWater) cy = AverageMin(c1y, c2y, c3y) + (yScale / 2f);
			else cy = AverageMax(c1y, c2y, c3y) + (yScale / 2f);
			if (cy > 0.5 || nextToWater) {
				foreach (int target in targets) {
					if (target > -1) {
						vertices [target].y = cy + adjust;
					}
				}
			} else {
				foreach (int target in targets) {
					if (target > -1) {
						vertices [target].y = 0.5f;
					}
				}
			}
		}
	}
	
	bool IsWater (GameObject block)
	{
		if (block == null)
			return false;
		return block.name.Contains ("Water");
	}

	float Average (float c1y, float c2y, float c3y) {
		return ((c1y + c2y + c3y) / 3);
	}
	
	float Max (float c1y, float c2y, float c3y) {
		return Mathf.Max(c1y, Mathf.Max(c2y, c3y));
	}
	
	float Min (float c1y, float c2y, float c3y) {
		return Mathf.Min(c1y, Mathf.Min(c2y, c3y));
	}
	
	float AverageMax (float c1y, float c2y, float c3y) {
		float max = Max(c1y, c2y, c3y);
		float avg = (c1y + c2y + c3y) / 3;
		float val = max > avg ? max : avg;
		return val;
	}

	float AverageMin  (float c1y, float c2y, float c3y) {
		float min = Min(c1y, c2y, c3y);
		float avg = (c1y + c2y + c3y) / 3;
		float val = min > avg ? avg : min;
		return val;
	}
}
