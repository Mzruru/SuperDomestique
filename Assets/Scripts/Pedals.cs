using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class Pedals : MonoBehaviour {

	public Color32 activeLeftColour;
	public Color32 activeRightColour;
	public Color32 inactiveColour;
	
	float pedalPower = 10f;
	float outputPower = 0;
	float leftPower = 0;
	float rightPower = 0;
	float maxPower = 5f;
	float drag = 0.98f;
	Transform leftPedal;
	Transform rightPedal;
	Mesh leftPedalMesh;
	Mesh rightPedalMesh;
	VertexColouriser leftPedalColouriser;
	VertexColouriser rightPedalColouriser;
	Vector3 rotator;

	// Use this for initialization
	void Start () {
		foreach (Transform child in transform) {
			if (child.name == "Left Pedal") leftPedal = child;
			if (child.name == "Right Pedal") rightPedal = child;
		}

		leftPedalMesh = leftPedal.GetComponent<MeshFilter>().sharedMesh;
		rightPedalMesh = rightPedal.GetComponent<MeshFilter>().sharedMesh;
		leftPedalColouriser = leftPedal.GetComponent<VertexColouriser>();
		rightPedalColouriser = rightPedal.GetComponent<VertexColouriser>();

		rotator = Vector3.zero;
	}

	void OnEnable () {
		foreach (Transform child in transform) {
			if (child.name == "Left Pedal") leftPedal = child;
			if (child.name == "Right Pedal") rightPedal = child;
		}
		leftPedalMesh = leftPedal.GetComponent<MeshFilter>().sharedMesh;
		rightPedalMesh = rightPedal.GetComponent<MeshFilter>().sharedMesh;
		leftPedalColouriser = leftPedal.GetComponent<VertexColouriser>();
		rightPedalColouriser = rightPedal.GetComponent<VertexColouriser>();
	}
	
	// Update is called once per frame
	void Update () {

		bool leftDown = Input.GetButton("Left");
		bool rightDown = Input.GetButton("Right");

		// rotation 0 = left pedal up

		float rot = transform.localEulerAngles.x % 360;
		leftPower = GetPowerFromRange(leftDown, rightDown, rot, 0, 180);
		rightPower = GetPowerFromRange(rightDown, leftDown, rot, 180, 360);
		float totalAddedPower = (leftPower + rightPower) * Time.deltaTime;
		outputPower += totalAddedPower;
		if (outputPower < 0) outputPower = 0;
		if (outputPower > maxPower) outputPower = maxPower;
		outputPower *= drag;
		if (outputPower < 0.01) outputPower = 0;
		rotator.x = outputPower;

		transform.Rotate(rotator);
		Quaternion mainRotation = Quaternion.Inverse(transform.localRotation);
		leftPedal.transform.localRotation = mainRotation;
		rightPedal.transform.localRotation = mainRotation;

		if (rot > 0 && rot < 180) {
			leftPedalColouriser.UpdateWithColour(leftPedalMesh, activeLeftColour);
			rightPedalColouriser.UpdateWithColour(rightPedalMesh, inactiveColour);
		} else {
			leftPedalColouriser.UpdateWithColour(leftPedalMesh, inactiveColour);
			rightPedalColouriser.UpdateWithColour(rightPedalMesh, activeRightColour);
		}
	}

	float GetPowerFromRange(bool pushingPrimary, bool pushingSecondary, float rot, float min, float max) {
		if (!pushingPrimary) return 0f;
		if (rot < min || rot > max) return pushingSecondary ? -pedalPower : pedalPower / 4f;
		return pedalPower;
	}
}
