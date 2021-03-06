using UnityEngine;
using System.Collections;

public class Rider : MonoBehaviour {

	public GameObject world;
	
	[Tooltip("The actual bike mesh")]public GameObject meshObject;
	[Tooltip("How strong the horizontal input is")]public float riderSpeed = 1.0f;
	[Tooltip("The effect of hills on speed")]public float accelerationDueToGravity = 5.0f;
	[Tooltip("How quickly the rider will slow down without any input")]public float friction = 0.01f;
	[Tooltip("The rider's starting stamina")]public float staminaMax = 2;
	[Tooltip("How much stamina is used when pressing the sprint input (right)")]public float staminaUsedSprint = 0.1f;
	[Tooltip("How fast stamina recovers when not pressing an input")]public float staminaRecovery = 0.05f;
	[Tooltip("How much power boost holding sprint provides per update")]public float sprintBoostPower = 0.1f;
	[Tooltip("The maximum power boost sprinting can provide")]public float sprintBoostMaximum = 3.0f;
	[Tooltip("How quickly sprint boost reduces after releasing the sprint input")]public float sprintBoostFalloff = 0.99f;
	[Tooltip("How heavy the bike is. Effects acceleration on hills")]public float bikeWeight = 1;
	[Tooltip("How fast the bike moves sideways")]public float sidewaysSpeed = 3;
	[Tooltip("How much the bike leans when moving sideways in degrees")]public float maxLean = 30f;
	[Tooltip("The gameobject used to display the current power")]public GameObject powerOutput;
	[Tooltip("Scales the power output value in the display")]public float powerOutputScale = 1;
	[Tooltip("The gameobject used to display the current stamina")]public GameObject staminaOutput;
	[Tooltip("Scales the stamina output value in the display")]public float staminaOutputScale = 1f;

	float currentSpeed = 0.0f;
	float currentSidewaysSpeed = 0.0f;
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

/*
		if (speedOutput) {
			speedOutputZ = speedOutput.transform.position.z;
		}
*/

		if (staminaOutput) {
			staminaOutputZ = staminaOutput.transform.position.z;
		}
	}
	
	// Update is called once per frame
	void Update () {
		// along road
		float moveX = -riderSpeed;
		bool sprinting = Input.GetButton("Right");
		float accelerationOnSlope = (accelerationDueToGravity * -normal.x) * bikeWeight;

		if (currentStamina < staminaMax / 10) {
			print ("CANNOT SPRINT:" + currentStamina + " : " + staminaMax);
			moveX /= 2;
			currentSpeed /= 1.2f;
			sprinting = false;
			sprintBoosted = false;
		}

		// order is important!
		updateStamina(sprinting, accelerationOnSlope);
		updatePower(moveX, sprinting);
		updateSpeed(moveX, accelerationOnSlope);

		int newBlockPosition = blockOffset - generator.xOffset;
		float currentPositionInBlock = (-generator.offset % 1);
		UpdateCurrentBlock(newBlockPosition);
		UpdateRiderYPosition(currentBlock, currentPositionInBlock);
		
		UpdateRiderZPosition();
	}

	void updateStamina (bool sprinting, float accelerationOnSlope) {
		float staminaReduction = 0; 
		if (sprinting) {
			staminaReduction = staminaUsedSprint;
			if (accelerationOnSlope < 0) staminaReduction *= -accelerationOnSlope;
		}
		currentStamina -= staminaReduction;
		
		if (!sprinting) {
			currentStamina += staminaRecovery;
			if (currentStamina > staminaMax) currentStamina = staminaMax;
		}
		if (currentStamina < 0) currentStamina = 0;

		positionGauge(staminaOutput, currentStamina * staminaOutputScale, staminaOutputZ);
	}

	void updatePower (float move, bool sprinting) {
		if (sprinting && !sprintBoosted) {
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
		currentSpeed = (currentSpeed + accelerationOnSlope + move) * (1 - friction);
		if (currentSpeed > 0) currentSpeed = 0;
		float totalSpeed = (currentSpeed) - currentSprintBoost;
		if (totalSpeed > -0.1f) totalSpeed = -0.1f;
		generator.Move(totalSpeed);

		//positionGauge(speedOutput, totalSpeed * speedOutputScale, speedOutputZ);
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
		int z = generator.currentWidth / 2;
		currentBlock = generator.GetObjectForPosition(currentBlockPosition, z, generator.xOffset);
		
		UpdateRiderRotation(currentBlock);
	}

	void UpdateRiderZPosition () {
		// across road
		Vector3 position = gameObject.transform.position;
		float newSpeed = Input.GetAxis("Vertical") * sidewaysSpeed * Time.smoothDeltaTime;
		float newPosition = position.z - newSpeed;
		int roadHalfWidth = ((int)generator.roadWidth / 2) - 1;
		if (newPosition < -roadHalfWidth) {
			newPosition = -roadHalfWidth;
			currentSidewaysSpeed *= 0.9f;
		}
		else if (newPosition > roadHalfWidth) {
			newPosition = roadHalfWidth;
			currentSidewaysSpeed *= 0.9f;
		} else {
			currentSidewaysSpeed = newSpeed;
		}
		position.z = newPosition;
		gameObject.transform.position = position;
		
	}
	
	void UpdateRiderYPosition (GameObject block, float currentPositionInBlock) {
		Vector3 originalPosition = gameObject.transform.position;
		IMeshModder finder  = (IMeshModder)block.GetComponent(typeof(IMeshModder));
		Vector3 centre = finder.GetPointOnTopFace(0.5f, currentPositionInBlock);
		centre += block.transform.position;
		centre -= normal * 0.5f;
		
		if (centre.y < gameObject.transform.position.y) { 
			float dist = gameObject.transform.position.y - centre.y;
			dist *= bikeWeight;
			centre.y = gameObject.transform.position.y - dist;
		}

		centre.z = originalPosition.z;
		gameObject.transform.position = centre;
	}

	void UpdateRiderRotation (GameObject block) {
		IMeshModder modder = (IMeshModder)block.GetComponent(typeof(IMeshModder));
		normal = modder.GetAverageOfTopNormal();
		gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
		gameObject.transform.Rotate(new Vector3(0, 0, (currentSidewaysSpeed / sidewaysSpeed) * maxLean));
	}
}
