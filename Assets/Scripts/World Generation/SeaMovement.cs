using UnityEngine;
using System.Collections;

public class SeaMovement : MonoBehaviour {

	IMeshModder meshModder;
	float startPosition;
	int offset;
	public int speed = 100;
	public static float height = 0.1f;
	public static ArrayList sinValues;

	// Use this for initialization
	void Start () {
		startPosition = transform.position.y;
		offset = Random.Range (0, 359);
		meshModder = (IMeshModder)gameObject.GetComponent(typeof(IMeshModder));

		if (sinValues == null) {
			CalculateValues();
		}
	}

	static void CalculateValues () {
		sinValues = new ArrayList();
		for (int i = 0; i < 360; i++)
		{
			sinValues.Add(Mathf.Sin(i) * height);
		}
	}
	
	// Update is called once per frame
	void Update () {
		float delta = Time.time;
		int pos = (offset + ((int)delta * speed)) % 360;;
		float y = startPosition + (float)sinValues[pos];
		
		Vector3 position = transform.position;
		position.y = y;
		transform.position = position;

		if (meshModder != null)
			meshModder.UpdateVertices();
	}
}
