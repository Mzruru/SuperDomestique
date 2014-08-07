using UnityEngine;
using System.Collections;

public class ZoomCameraWithSpeed : MonoBehaviour
{

	public GameObject subject;
	public Vector3 fullyOutPosition;
	public float maxSpeed = 10f;
	public Vector3 easeSpeed = new Vector3(0.5f, 0.5f, 0.5f);
	Vector3 vectorToSubject = Vector3.zero;
	Vector3 vectorToFullyOut = Vector3.zero;
	Vector3 subjectLastPosition;
	Vector3 movement;
	Vector3 targetOffset;
	Vector3 currentPosition;
	Vector3 targetPosition;
	
	// Use this for initialization
	void Start ()
	{
		vectorToSubject = gameObject.transform.position - subject.transform.position;
		vectorToFullyOut = fullyOutPosition - subject.transform.position;
		subjectLastPosition = subject.transform.position;
		targetOffset = gameObject.transform.position;
		currentPosition = gameObject.transform.position;
		targetPosition = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
		movement = (subject.transform.position - subjectLastPosition);
		float speed = movement.sqrMagnitude;
		if (speed > maxSpeed * maxSpeed) targetOffset = vectorToFullyOut;
		else targetOffset = Vector3.Lerp(vectorToSubject, vectorToFullyOut, Mathf.Lerp(0, maxSpeed * maxSpeed, speed));
		
		targetPosition = subject.transform.position + targetOffset;
		currentPosition = gameObject.transform.position;
		currentPosition.x = Mathf.Lerp(currentPosition.x, targetPosition.x, easeSpeed.x);
		currentPosition.y = Mathf.Lerp(currentPosition.y, targetPosition.y, easeSpeed.y);
		currentPosition.z = Mathf.Lerp(currentPosition.z, targetPosition.z, easeSpeed.z);
		
		if (currentPosition.y < targetPosition.y) currentPosition.y = targetPosition.y;
		gameObject.transform.position = currentPosition;
		subjectLastPosition = subject.transform.position;
	}
}
