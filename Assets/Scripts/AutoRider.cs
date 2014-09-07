﻿using UnityEngine;
using System.Collections;

public class AutoRider : MonoBehaviour {
	
	public GameObject world;
	
	[Tooltip("The actual bike mesh")]public GameObject meshObject;
	[Tooltip("How strong the horizontal input is")]public float riderSpeed = 1.0f;
	[Tooltip("How much friction affects the speed")]public float friction = 0.02f;
	[Tooltip("The effect of hills on speed")]public float accelerationDueToGravity = 5.0f;
	[Tooltip("How many sprints can be used")]public int numberOfSprints = 3;
	[Tooltip("How much power boost using a sprint provides")]public float sprintBoostPower = 0.1f;
	[Tooltip("How long a sprint lasts")]public float sprintDuration = 2f;
	[Tooltip("How quickly sprint boost reduces after releasing finished")]public float sprintBoostFalloff = 0.99f;
	[Tooltip("How heavy the bike is. Effects acceleration on hills")]public float bikeWeight = 1;
	[Tooltip("How fast the bike moves sideways")]public float sidewaysSpeed = 3;
	
	float currentSpeed = 0.0f;
	float currentSidewaysSpeed = 0.0f;
	Vector3 normal = Vector3.zero;
	int blockOffset = 0;
	int currentBlockPosition;
	GameObject currentBlock;
	HillGenerator generator;
	bool sprintBoosted;
	float currentSprintBoost = 0.0f;
	float sprintTime;
	
	// Use this for initialization
	void Start () {
		generator = world.GetComponent<HillGenerator>();
		blockOffset = Mathf.FloorToInt(gameObject.transform.position.x);
	}
	
	// Update is called once per frame
	void Update () {
		// along road
		float moveX = -riderSpeed;
		bool sprinting = Input.GetButton("Right") && !sprintBoosted && numberOfSprints > 0;
		float accelerationOnSlope = (accelerationDueToGravity * -normal.x) * bikeWeight;
		
		if (sprinting)
		{
			sprintTime = sprintDuration;
			sprintBoosted = true;
			currentSprintBoost = 0;
			numberOfSprints--;
		} else if (sprintBoosted) {
			sprintTime -= Time.deltaTime;
			if (sprintTime <= 0) sprintBoosted = false;
			currentSprintBoost += sprintBoostPower * Time.deltaTime;
		} else if (currentSprintBoost > 0) {
			currentSprintBoost -= sprintBoostFalloff * Time.deltaTime;
		}
		
		updateSpeed(moveX, accelerationOnSlope);
		
		int newBlockPosition = blockOffset - generator.xOffset;
		float currentPositionInBlock = (-generator.offset % 1);
		UpdateCurrentBlock(newBlockPosition);
		UpdateRiderYPosition(currentBlock, currentPositionInBlock);
		//UpdateRiderZPosition();
	}
	
	void updateSpeed (float move, float accelerationOnSlope) {
		currentSpeed = (currentSpeed + accelerationOnSlope + move) * (1 - friction);
		if (currentSpeed > 0) currentSpeed = 0;
		float totalSpeed = (currentSpeed) - currentSprintBoost;
		if (totalSpeed > -0.1f) totalSpeed = -0.1f;
		totalSpeed *= Time.deltaTime;
		generator.Move(totalSpeed);
	}
	
	void UpdateCurrentBlock (int newPosition) {
		if (currentBlockPosition == newPosition & currentBlock != null) return;
		
		currentBlockPosition = newPosition;
		int z = generator.currentWidth / 2;
		currentBlock = generator.GetObjectForPosition(currentBlockPosition, z, generator.xOffset);
		
		UpdateRiderRotation(currentBlock);
	}
	
	GameObject lastBlock;
	void UpdateRiderYPosition (GameObject block, float currentPositionInBlock) {
		Vector3 originalPosition = gameObject.transform.position;
		IMeshModder finder  = (IMeshModder)block.GetComponent(typeof(IMeshModder));
		Vector3 centre = finder.GetPointOnTopFace(currentPositionInBlock, 0.5f);
		float y = centre.y;
		centre += block.transform.position;
		centre -= normal * 0.5f;
		
		if (centre.y < gameObject.transform.position.y) { 
			float dist = gameObject.transform.position.y - centre.y;
			centre.y = gameObject.transform.position.y - dist;
		}
		
		centre.x = originalPosition.x;
		gameObject.transform.position = centre;
	}
	
	void UpdateRiderZPosition () {
		// across road
		Vector3 position = gameObject.transform.position;
		float newSpeed = Input.GetAxis("Vertical") * sidewaysSpeed * Time.smoothDeltaTime;
		float newPosition = position.x - newSpeed;
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
	
	void UpdateRiderRotation (GameObject block) {
		IMeshModder modder = (IMeshModder)block.GetComponent(typeof(IMeshModder));
		normal = modder.GetAverageOfTopNormal();
		gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
		gameObject.transform.Rotate(new Vector3(0, 90, 0));
	}
}