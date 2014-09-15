using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour {

	[Tooltip("The object to follow")] public GameObject subject;
	[Tooltip("The ease to the target, set to 1 to lock to the axis")] Vector3 ease = new Vector3(0.5f, 0.5f, 0.5f);

	Vector3 cameraOffset;
	
	// Use this for initialization
	void Start () {
		cameraOffset = transform.position - subject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 targetPosition = subject.transform.position + cameraOffset;
		Vector3 offset = (targetPosition - transform.position);
		offset.x *= ease.x;
		offset.y *= ease.y;
		offset.z *= ease.z;
		targetPosition = transform.position + offset;
		transform.position = targetPosition;
	}
}
