using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]

public class Unit : MonoBehaviour {
	
	public float moveSpeed = 2f;
	public float turnSpeed = 90f;
	protected CharacterController control;

	protected Vector3 move = Vector3.zero;

	// Use this for initialization
	public virtual void Start () {
		control = gameObject.GetComponent<CharacterController>();	
	}
	
	// Update is called once per frame
	public virtual void Update () {
		control.SimpleMove(move * moveSpeed);
	}
}
