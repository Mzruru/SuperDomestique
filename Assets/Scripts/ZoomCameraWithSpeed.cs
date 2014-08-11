using UnityEngine;
using System.Collections;

// Causes the camera to follow the subject, and zoom in and out based on the subject's speed
public class ZoomCameraWithSpeed : MonoBehaviour
{
	[Tooltip ("What the camera should follow")] public GameObject subject;
	[Tooltip ("The minimum speed to be used to affect the camera")] public float minSpeed = 1f;
	[Tooltip ("The maximum speed to be used to affect the camera")] public float maxSpeed = 10f;
	[Tooltip ("Strength of easing to the new target position. A value of 1 will snap to the target.")] public Vector3 ease = new Vector3(0.5f, 0.75f, 0.5f);
	[Tooltip ("When the subject is at the maximum speed, the camera will be at this position.")] public Vector3 fullZoomPosition = new Vector3(1, 1, 1);
	[Tooltip ("The number of frames over which subject speed will be smoothed. Higher value results in smoother movement.")]public int smoothOverFrames = 100;
	
	Vector3 subjectLastPosition;
	Vector3 cameraOffset;
	Vector3 minZoomOffset;
	Vector3 maxZoomOffset;
	Vector3 targetPosition;
	Vector3 targetOffset;
	
	float[] smoothSpeeds;
	int c = 0;
	int m = 0;
	
	// Use this for initialization
	void Start ()
	{
		subjectLastPosition = subject.transform.position;
		cameraOffset = transform.position - subject.transform.position;
		smoothSpeeds = new float[smoothOverFrames];
		minZoomOffset = transform.position - subject.transform.position;
		maxZoomOffset = fullZoomPosition - subject.transform.position;
	 }
	
	// Update is called once per frame
	void Update ()
	{
		RepositionCamera();
	}
	
	void RepositionCamera () 
	{
		float speed = (subjectLastPosition - subject.transform.position).magnitude;
		speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
		
		smoothSpeeds[c] = speed;
		if (c > m) m = c;
		float smoothedSpeed = 0;
		for (int i = 0; i < m; i++)
		{
			smoothedSpeed += smoothSpeeds[i];	
		}
		smoothedSpeed /= (float)(m + 1);
		c++;
		if (c >= smoothOverFrames) c = 0;
		
		targetOffset = Vector3.Lerp(minZoomOffset, maxZoomOffset, Mathf.InverseLerp(minSpeed, maxSpeed, smoothedSpeed));
		targetPosition = subject.transform.position + targetOffset;
		Vector3 offset = (targetPosition - transform.position);
		offset.x *= ease.x;
		offset.y *= ease.y;
		offset.z *= ease.z;
		targetPosition = transform.position + offset;
		transform.position = targetPosition;
		
		subjectLastPosition = subject.transform.position;
	}
}
