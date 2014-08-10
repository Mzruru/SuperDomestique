using UnityEngine;
using System.Collections;

public class ZoomCameraWithSpeed : MonoBehaviour
{

	public GameObject subject;
	public float minSpeed = 1f;
	public float maxSpeed = 10f;
	public Vector3 ease = new Vector3(0.5f, 0.5f, 0.5f);
	public Vector3 zoomToSpeedRatio = new Vector3(1, 1, 1);
	public int smoothOverFrames = 30;
	
	Vector3 subjectLastPosition;
	Vector3 cameraOffset;
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
		
		targetOffset = Vector3.Scale(cameraOffset, zoomToSpeedRatio * (smoothedSpeed / minSpeed));
			
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
