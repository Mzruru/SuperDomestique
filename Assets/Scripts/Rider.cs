using UnityEngine;
using System.Collections;

public class Rider : MonoBehaviour {

	public GameObject world;

	public float moveSpeed = 0.5f;
	public float inputStrength = 1.0f;
	public float accelerationDueToGravity = 5.0f;
	public float friction = 0.99f;
	public float staminaMax = 2;
	public GameObject powerOutput;
	public float powerOutputScale = 1;
	public GameObject speedOutput;
	public float speedOutputScale = 0.25f;
	public GameObject staminaOutput;
	public float staminaOutputScale = 1f;
	public float staminaUsedNormal = 0.05f;
	public float staminaUsedSprint = 0.1f;
	public float staminaRecovery = 0.05f;
	public float sprintBoostPower = 0.1f;
	public float sprintBoostMaximum = 3.0f;
	public float sprintBoostFalloff = 0.99f;
	public float bikeWeight = 1;

	float currentSpeed = 0.0f;
	Vector3 normal = Vector3.zero;
	int blockOffset = 0;
	int currentBlockPosition;
	GameObject currentBlock;
	HillGenerator generator;
	float currentStamina;
	bool sprintBoosted;
	float currentSprintBoost = 0.0f;

	float powerOutputZ;
	float speedOutputZ;
	float staminaOutputZ;

	// Use this for initialization
	void Start () {
		generator = world.GetComponent<HillGenerator>();
		blockOffset = Mathf.FloorToInt(gameObject.transform.position.z);
		currentStamina = staminaMax;

		if (powerOutput) {
			powerOutputZ = powerOutput.transform.position.z;
		}

		if (speedOutput) {
			speedOutputZ = speedOutput.transform.position.z;
		}

		if (staminaOutput) {
			staminaOutputZ = staminaOutput.transform.position.z;
		}
	}
	
	// Update is called once per frame
	void Update () {
		float move = Input.GetAxis("Horizontal") * -inputStrength;
		bool rightIsOn = Input.GetButton("Right");
		bool leftIsOn = Input.GetButton("Left");
		float accelerationOnSlope = (accelerationDueToGravity * -normal.z);

		if (currentStamina < staminaMax  / 20) {
			print ("KNACKERED");
			move /= 2;
			currentSpeed /= 1.2f;
		}
		if (currentStamina < staminaMax / 10) {
			print ("CANNOT SPRINT:" + currentStamina + " : " + staminaMax);
			leftIsOn = false;
			sprintBoosted = false;
		}

		updateStamina(leftIsOn, rightIsOn, accelerationOnSlope);
		updatePower(move, leftIsOn);
		updateSpeed(move, accelerationOnSlope);

		int newBlockPosition = blockOffset - generator.zOffset;
		float currentPositionInBlock = (-generator.offset % 1);
		UpdateCurrentBlock(newBlockPosition);
		UpdateRiderYPosition(currentBlock, currentPositionInBlock);
	}

	void updateStamina (bool leftIsOn, bool rightIsOn, float accelerationOnSlope) {
		float staminaReduction = 0; 
		if (rightIsOn) {
			staminaReduction = staminaUsedNormal * accelerationOnSlope;
			if (staminaReduction < staminaUsedNormal) staminaReduction = staminaUsedNormal;
		}
		if (leftIsOn) {
			staminaReduction = staminaUsedSprint * accelerationOnSlope;
			if (staminaReduction < staminaUsedNormal) staminaReduction = staminaUsedNormal;
		}
		currentStamina -= staminaReduction;
		
		if (!rightIsOn && !leftIsOn) {
			currentStamina += staminaRecovery;
			if (currentStamina > staminaMax) currentStamina = staminaMax;
		}
		if (currentStamina < 0) currentStamina = 0;

		positionGauge(staminaOutput, currentStamina * staminaOutputScale, staminaOutputZ);
	}

	void updatePower (float move, bool leftIsOn) {
		if (leftIsOn && !sprintBoosted) {
			currentSprintBoost += sprintBoostPower;
			if (currentSprintBoost > sprintBoostMaximum) currentSprintBoost = sprintBoostMaximum;
			sprintBoosted = true;
		} else {
			sprintBoosted = false;
			currentSprintBoost *= sprintBoostFalloff;
			if (currentSprintBoost < 0.01) currentSprintBoost = 0;
		}

		positionGauge(powerOutput, (move - (currentSprintBoost / 2)) * powerOutputScale, powerOutputZ);
	}

	void updateSpeed (float move, float accelerationOnSlope) {
		currentSpeed = (currentSpeed + accelerationOnSlope + move) * friction;
		if (currentSpeed > 0) currentSpeed = 0;
		float totalSpeed = (currentSpeed * moveSpeed) - currentSprintBoost;
		if (totalSpeed > -0.1f) totalSpeed = -0.1f;
		generator.Move(totalSpeed);

		positionGauge(speedOutput, totalSpeed * speedOutputScale, speedOutputZ);
	}

	void positionGauge (GameObject gauge, float scale, float offset) {
		if (gauge == null) return;

		scale = Mathf.Abs(scale);
		Vector3 gaugeScale = gauge.transform.localScale;
		gaugeScale.x = scale;
		gauge.transform.localScale = gaugeScale;
		
		Vector3 gaugePosition = gauge.transform.position;
		gaugePosition.z = offset + (scale / 2);
		gauge.transform.position = gaugePosition;
	}
	
	void UpdateCurrentBlock (int newPosition) {
		if (currentBlockPosition == newPosition) return;

		currentBlockPosition = newPosition;
		int x = generator.currentWidth / 2;
		currentBlock = generator.GetObjectForPosition(x, currentBlockPosition, generator.zOffset);
		
		UpdateRiderRotation(currentBlock);
	}

	void UpdateRiderYPosition (GameObject block, float currentPositionInBlock) {
		ICornerFinder finder  = (ICornerFinder)block.GetComponent(typeof(ICornerFinder));
		Vector3 centre = finder.GetTopCentrePoint(currentPositionInBlock);
		centre += block.transform.position;
		centre += normal * (gameObject.transform.localScale.y / 2);

		if (centre.y < gameObject.transform.position.y) { 
			float dist = gameObject.transform.position.y - centre.y;
			dist *= bikeWeight;
			centre.y = gameObject.transform.position.y - dist;
		}

		gameObject.transform.position = centre;
	}

	void UpdateRiderRotation (GameObject block) {
		normal = GetAverageOfTopNormal(block);
		gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
	}

	Vector3 GetAverageOfTopNormal (GameObject block) {
		Vector3[] normals = block.GetComponent<MeshFilter>().mesh.normals;

		int c = 0;
		Vector3 avg = Vector3.zero;
		foreach (Vector3 normal in normals) {
			if (normal.y == 0 || normal.y < 0) continue;
			avg += normal;
			c++;
		}

		avg /= (float)c;

		return avg;
	}
}
