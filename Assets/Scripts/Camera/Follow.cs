using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour
{

		[Tooltip("The object to follow")]
		public GameObject
				subject;
		[Tooltip("The ease to the target, set to 1 to lock to the axis")]
		public Vector3
				ease = new Vector3 (0.5f, 0.5f, 0.5f);
		[Tooltip("Movement will be multiplied by this value. E.g. a value of 0.5 will make the object move half the distance the subject moved")]
		public Vector3
				followMultiplier = new Vector3 (1f, 1f, 1f);
		Vector3 cameraOffset;
		Vector3 lastSubjectPosition;
	
		// Use this for initialization
		void Start ()
		{
				cameraOffset = transform.position - subject.transform.position;
				lastSubjectPosition = subject.transform.position;
		}
	
		// Update is called once per frame
		void Update ()
		{
				Vector3 movement = subject.transform.position - lastSubjectPosition;
				movement.x *= followMultiplier.x;
				movement.y *= followMultiplier.y;
				movement.z *= followMultiplier.z;
				Vector3 targetPosition = lastSubjectPosition + movement + cameraOffset;
				Vector3 offset = (targetPosition - transform.position);
				offset.x *= ease.x;
				offset.y *= ease.y;
				offset.z *= ease.z;
				targetPosition = transform.position + offset;
				transform.position = targetPosition;
		}
}
