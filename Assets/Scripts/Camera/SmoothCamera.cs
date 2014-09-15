using UnityEngine;
using System.Collections;

// Causes the camera to follow the subject, and zoom in and out based on the subject's speed
public class SmoothCamera : MonoBehaviour
{
	[Tooltip ("What the camera should follow")] public GameObject subject;
	[Tooltip ("Strength of easing to the new target position for adjustment. A value of 1 will snap to the target.")] public Vector3 ease = new Vector3(0.5f, 0.75f, 0.5f);
	[Tooltip ("Whether or not to use orthographic depth sorting (might help z-fighting)")] public bool useOrtographicSorting = false;
	Vector3 targetOffset;
	
	// Use this for initialization
	void Start ()
	{
		targetOffset = transform.position - subject.transform.position;
		if (useOrtographicSorting) gameObject.camera.transparencySortMode = TransparencySortMode.Orthographic;
	 }
	
	// Update is called once per frame
	void Update ()
	{
		RepositionCamera();
	}
	
	void RepositionCamera () 
	{
		Vector3 move = (subject.transform.position + targetOffset) - transform.position;
		move.x *= ease.x;
		move.y *= ease.y;
		move.z *= ease.z;
		transform.position += move;
	}
}
