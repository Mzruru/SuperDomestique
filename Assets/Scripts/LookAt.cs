using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour {

	public Transform target;
	public bool lockX;
	public bool lockY;
	public bool lockZ;
	
	Vector3 preRotation;
	// Use this for initialization
	void Start () {
		preRotation = transform.localEulerAngles;
		transform.LookAt (target);
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt (target);
		
		Vector3 local = transform.localEulerAngles;
		if (lockX) local.x = preRotation.x;
		if (lockY) local.y = preRotation.y;
		if (lockZ) local.z = preRotation.z;
		
		transform.localEulerAngles = local;
	}
}
