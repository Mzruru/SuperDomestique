using UnityEngine;
using System.Collections;

public class ZoomCameraWithAngle : MonoBehaviour {

	[Tooltip ("What the camera should follow")] public GameObject subject;
	[Tooltip ("The minimum angle to be used to affect the camera")] public float minAngle = -30f;
	[Tooltip ("The maximum angle to be used to affect the camera")] public float maxAngle = 30f;
	[Tooltip ("Strength of easing to the new target position. A value of 1 will snap to the target.")] public Vector3 ease = new Vector3(0.5f, 0.75f, 0.5f);
	[Tooltip ("When the subject is at the maximum upward angle, the camera will be at this position.")] public Vector3 fullUpwardPosition = new Vector3(1, 1, 1);
	[Tooltip ("When the subject is at a neutral angle, the camera will be at this position.")] public Vector3 neutralPosition = new Vector3(1, 1, 1);
	[Tooltip ("When the subject is at the maximum downward angle, the camera will be at this position.")] public Vector3 fullDownwardPosition = new Vector3(1, 1, 1);
	[Tooltip ("The number of frames over which subject speed will be smoothed. Higher value results in smoother movement.")]public int smoothOverFrames = 100;
	
	Vector3 upZoomOffset;
	Vector3 neutralZoomOffset;
	Vector3 downZoomOffset;
	Vector3 targetPosition;
	Vector3 targetOffset;
	
	float[] smoothAngles;
	int c = 0;
	int m = 0;
	
	// Use this for initialization
	void Start () {
		smoothAngles = new float[smoothOverFrames];
		upZoomOffset = fullUpwardPosition - subject.transform.position;
		neutralZoomOffset = transform.position - subject.transform.position;
		downZoomOffset = fullDownwardPosition - subject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		RepositionCamera();
	}
	
	void RepositionCamera () 
	{
		float angle = subject.transform.localRotation.x;
		angle = Mathf.Clamp(angle, minAngle, maxAngle);
		
		smoothAngles[c] = angle;
		if (c > m) m = c;
		float smoothedAngle = 0;
		for (int i = 0; i < m; i++)
		{
			smoothedAngle += smoothAngles[i];	
		}
		smoothedAngle /= (float)(m + 1);
		c++;
		if (c >= smoothOverFrames) c = 0;
		
		print ("angle:" + smoothedAngle);
		if (smoothedAngle == 0) targetOffset = neutralZoomOffset;
		else if (smoothedAngle < 0) {
			targetOffset = Vector3.Lerp(upZoomOffset, neutralZoomOffset, Mathf.InverseLerp(minAngle, 0, smoothedAngle));
		} else {
			targetOffset = Vector3.Lerp(neutralZoomOffset, downZoomOffset, Mathf.InverseLerp(0, maxAngle, smoothedAngle));
		}
		targetPosition = subject.transform.position + targetOffset;
		Vector3 offset = (targetPosition - transform.position);
		offset.x *= ease.x;
		offset.y *= ease.y;
		offset.z *= ease.z;
		targetPosition = transform.position + offset;
		transform.position = targetPosition;
	}
}
