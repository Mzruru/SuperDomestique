using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour {

	public GameObject target;
	public bool lockX;
	public bool lockY;
	public bool lockZ;

	Vector3 startOffset;

	// Use this for initialization
	void Start () {
		startOffset = gameObject.transform.position - target.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 newPos = target.transform.position + startOffset;
		if (lockX) newPos.x = gameObject.transform.position.x;
		if (lockY) newPos.y = gameObject.transform.position.y;
		if (lockZ) newPos.z = gameObject.transform.position.z;
		gameObject.transform.position = newPos;
	}
}
