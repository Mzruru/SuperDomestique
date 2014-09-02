using UnityEngine;
using System.Collections;

// Causes the camera to follow the subject, and zoom in and out based on the subject's speed
public class ZoomCameraWithSpeed : MonoBehaviour
{
	[Tooltip ("What the camera should follow")] public GameObject subject;
	[Tooltip ("The minimum speed to be used to affect the camera")] public float minSpeed = 1f;
	[Tooltip ("The maximum speed to be used to affect the camera")] public float maxSpeed = 10f;
	[Tooltip ("The minimum angle to be used to affect the camera")] public float minAngle = 0f;
	[Tooltip ("The maximum angle to be used to affect the camera")] public float maxAngle = 10f;
	[Tooltip ("Strength of easing to the new target position for speed adjustment. A value of 1 will snap to the target.")] public Vector3 speedEase = new Vector3(0.5f, 0.75f, 0.5f);
	[Tooltip ("Strength of easing to the new target position for angle adjustment. A value of 1 will snap to the target.")] public Vector3 angleEase = new Vector3(0.5f, 0.75f, 0.5f);
	[Tooltip ("When the subject is at the maximum speed, the camera will be at this position.")] public Vector3 fullSpeedZoomPosition = new Vector3(1, 1, 1);
	[Tooltip ("When the subject is at the maximum speed, the camera will be at this position.")] public Vector3 fullAngleZoomPosition = new Vector3(1, 1, 1);
	[Tooltip ("The number of frames over which subject speed will be smoothed. Higher value results in smoother movement, but also a delay in returning to position.")]public int smoothSpeedOverFrames = 100;
	[Tooltip ("The number of frames over which subject angle will be smoothed. Higher value results in smoother movement, but also a delay in returning to position.")]public int smoothAngleOverFrames = 100;
	
	Vector3 subjectLastPosition;
	Vector3 minZoomOffset;
	Vector3 maxSpeedZoomOffset;
	Vector3 maxAngleZoomOffset;
	Vector3 targetPosition;
	Vector3 targetOffset;
	
	float[] smoothSpeeds;
	float[] smoothAngles;
	int c = 0;
	int d = 0;
	int m = 0;
	int n = 0;
	
	// Use this for initialization
	void Start ()
	{
		subjectLastPosition = subject.transform.position;
		smoothSpeeds = new float[smoothSpeedOverFrames];
		smoothAngles = new float[smoothAngleOverFrames];
		minZoomOffset = transform.position - subject.transform.position;
		maxSpeedZoomOffset = fullSpeedZoomPosition - subject.transform.position;
		maxAngleZoomOffset = fullAngleZoomPosition - subject.transform.position;
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
		if (c >= smoothSpeedOverFrames) c = 0;
		
		float angle = subject.transform.localRotation.x * -180 / Mathf.PI;
		smoothAngles[d] = angle;
		if (d > n) n = d;
		float smoothedAngle = 0;
		for (int i = 0; i < n; i++)
		{
			smoothedAngle += smoothAngles[i];	
		}
		smoothedAngle /= (float)(n + 1);
		d++;
		if (d >= smoothAngleOverFrames) d = 0;
		
		Vector3 speedOffset = Vector3.Lerp(minZoomOffset, maxSpeedZoomOffset, Mathf.InverseLerp(minSpeed, maxSpeed, smoothedSpeed));
		Vector3 angleOffset = Vector3.Lerp(minZoomOffset, maxAngleZoomOffset, Mathf.InverseLerp(minAngle, maxAngle, smoothedAngle));
		if (speedEase.x == 1) speedOffset.x = maxSpeedZoomOffset.x;
		if (speedEase.y == 1) speedOffset.y = maxSpeedZoomOffset.y;
		if (speedEase.z == 1) speedOffset.z = maxSpeedZoomOffset.z;
		targetOffset = (speedOffset + angleOffset) / 2;
		targetPosition = subject.transform.position + targetOffset;
		Vector3 offset = (targetPosition - transform.position);
		offset.x *= speedEase.x;
		offset.y *= speedEase.y;
		offset.z *= speedEase.z;
		targetPosition = transform.position + offset;
		transform.position = targetPosition;
		
		subjectLastPosition = subject.transform.position;
	}
}
