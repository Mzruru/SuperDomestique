using UnityEngine;
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
	[Tooltip("How heavy the bike is. Effects acceleration on hills and height of jumps")]public float bikeWeight = 1;
	[Tooltip("How fast the bike moves sideways")]public float sidewaysSpeed = 3;
	[Tooltip("How much power jumps have")]public float jumpPower = 1;
	[Tooltip("The strength of gravity, when jumping")]public float gravity = 8.0f;
	
	Bounds riderBounds;
	int blockOffset = 0;
	int currentBlockPosition;
	GameObject currentBlock;
	HillGenerator generator;
	
	// movement
	Vector3 normal = Vector3.zero;
	float currentSpeed = 0.0f;
	Vector3 moveVec;
	
	// sprint
	bool sprintBoosted;
	float currentSprintBoost = 0.0f;
	float sprintTime;
	
	// dodge
	int targetZ;
	
	// jump
	bool useJump = true;
	float jumpVelocity;
	bool isJumping;
	
	// Use this for initialization
	void Start () {
		generator = world.GetComponent<HillGenerator>();
		blockOffset = Mathf.FloorToInt(gameObject.transform.position.x);
		
		riderBounds = meshObject.GetComponent<MeshFilter>().mesh.bounds;
		targetZ = (int)gameObject.transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {
		// along road
		float moveX = -riderSpeed;
		bool sprinting = Input.GetButton("Right") && !sprintBoosted && numberOfSprints > 0;
		float accelerationOnSlope = (accelerationDueToGravity * -normal.x) * bikeWeight;
		
		CheckForInput();
		
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
		
		moveVec = Vector3.zero;
		updateBasedOnSpeed(moveX, accelerationOnSlope);
		
		int newBlockPosition = blockOffset - generator.xOffset;
		float currentPositionInBlock = (-generator.offset % 1);
		UpdateCurrentBlock(newBlockPosition);
		UpdateRiderPosition(currentBlock, currentPositionInBlock);
	}
	
	void CheckForInput () {
		if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
		{
			if (Input.GetMouseButtonDown(0) || Input.GetTouch(0).phase == TouchPhase.Began)
			{
				if (useJump) DoJump();
				else DoDodge();	
			}
		}
	}
	
	void DoDodge () {
		float zPosition = gameObject.transform.position.z;
		if (zPosition > 0) SetRiderPosition(-3);
		else SetRiderPosition(3);
	}
	
	void DoJump () {
		if (isJumping) return;
		isJumping = true;
		
		jumpVelocity = jumpPower / bikeWeight;
	}
	
	void updateBasedOnSpeed (float move, float accelerationOnSlope) {
		currentSpeed = (currentSpeed + accelerationOnSlope + move) * (1 - friction);
		if (currentSpeed > 0) currentSpeed = 0;
		float totalSpeed = (currentSpeed) - currentSprintBoost;
		if (totalSpeed > -0.1f) totalSpeed = -0.1f;
		totalSpeed *= Time.smoothDeltaTime;
		
		float angle = Mathf.Atan2(normal.x, normal.y);
		float speedZComp = Mathf.Cos(angle) * totalSpeed;
		generator.Move(speedZComp);
	}
	
	void UpdateCurrentBlock (int newPosition) {
		if (currentBlockPosition == newPosition & currentBlock != null) return;
		
		currentBlockPosition = newPosition;
		int z = generator.currentWidth / 2;
		currentBlock = generator.GetObjectForPosition(currentBlockPosition, z, generator.xOffset);
		
		UpdateRiderRotation(currentBlock);
	}
	
	void UpdateRiderPosition (GameObject block, float currentPositionInBlock) {
		IMeshModder finder  = (IMeshModder)block.GetComponent(typeof(IMeshModder));
		
		moveVec = finder.GetPointOnTopFace(currentPositionInBlock, 0.5f);
		moveVec += block.transform.position;
		moveVec += normal * riderBounds.extents.y * meshObject.transform.localScale.y;
		if (isJumping) {
			Vector3 currentPosition = gameObject.transform.position;
			currentPosition.y += jumpVelocity;
			jumpVelocity -= gravity * Time.smoothDeltaTime;
			if (currentPosition.y < moveVec.y) {
				isJumping = false;
			} else {
				moveVec.y = currentPosition.y;
			}
		}
		
		UpdateRiderZPosition();
		
		gameObject.transform.position = moveVec;
	}
	
	void UpdateRiderZPosition () {
		
		// across road
		Vector3 position = gameObject.transform.position;
		int input = 0;
		if (position.z > targetZ) {
			input = -1;
		} else if (position.z < targetZ){
			input = 1;
		}
			
		float newSpeed = input * sidewaysSpeed * Time.smoothDeltaTime;
		if (newSpeed == 0) moveVec.z = targetZ;
		else moveVec.z = transform.position.z + newSpeed;
	}
	
	// rider position is from 4 (left hand edge) to -4 (right hand edge)
	void SetRiderPosition (int index) {
		if (index > 4 || index < -4) return;
		targetZ = index;
	}
	
	void UpdateRiderRotation (GameObject block) {
		IMeshModder modder = (IMeshModder)block.GetComponent(typeof(IMeshModder));
		
		if (isJumping) {
			float rotation = -jumpVelocity * 2;
			if (rotation < -2) rotation = -2;
			else if (rotation > 2) rotation = 2;
			gameObject.transform.Rotate(new Vector3(rotation, 0, 0));
			// TODO: when the bike's rotation is too high in comparision to the normal value, make it crash
		} else {
			normal = modder.GetAverageOfTopNormal();
			gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
			gameObject.transform.Rotate(new Vector3(0, 90, 0));
		}
	}
}
