﻿using UnityEngine;
using System.Collections;

public class ShipMovement : MonoBehaviour {
	public Character thisCharacter;
	public Camera renderCamera;
	public float shipLerpSpeed = 13.75f;						//Percent ship lerps towards desired position each FixedUpdate()
	protected float vertMovespeedDefault = 16f;
	protected float horizMovespeedDefault = 12.5f;
	public float verticalMovespeed;						//Speed at which the player can move up and down
	public float horizontalMovespeed;					//Speed at which the player can move right to left

	protected float shipTurnLerpSpeed = 5f;						//Percent ship lerps towards the desired rotation each FixedUpdate()
	float maxTurnAngle = 45f;							//Maximum amount a ship can toward in a certain direction

	public float viewportMinX { get { return thisCharacter.player.viewportMinX; } }
	public float viewportMaxX { get { return thisCharacter.player.viewportMaxX; } }
	public float viewportMinY { get { return thisCharacter.player.viewportMinY; } }
	public float viewportMaxY { get { return thisCharacter.player.viewportMaxY; } }

	[HideInInspector]
	public float worldSpaceMinX { get { return thisCharacter.player.worldSpaceMinX; } }
	[HideInInspector]
	public float worldSpaceMaxX { get { return thisCharacter.player.worldSpaceMaxX; } }
	[HideInInspector]
	public float worldSpaceMinY { get { return thisCharacter.player.worldSpaceMinY; } }
	[HideInInspector]
	public float worldSpaceMaxY { get { return thisCharacter.player.worldSpaceMaxY; } }

	public Vector3 desiredPosition;						//The position that the transform lerps towards each FixedUpdate()
	protected Quaternion startRotation;                 //The beginning rotation of the ship
	protected Quaternion desiredRotation;				//The rotation that the transform lerps towards each FixedUpdate()
	protected Vector3 dotVector;                        //Used to determine which way the ship should turn when moving up and down

	public bool movementDisabled = false;

	public KeyCode left, right, up, down;

	// Use this for initialization
	protected virtual void Awake() {
		renderCamera = Camera.main;
		thisCharacter = GetComponentInParent<Character>();

		verticalMovespeed = vertMovespeedDefault;
		horizontalMovespeed = horizMovespeedDefault;

		desiredPosition = transform.position;
		startRotation = transform.rotation;
		desiredRotation = startRotation;
		dotVector = -transform.right;
	}

	// Update is called once per frame
	virtual protected void Update() {
		if (movementDisabled || GameManager.S.gameState != GameStates.playing) {
			desiredRotation = startRotation;
			return;
		}

		if (thisCharacter.player.device == null) {
			if (Input.GetKey(left)) {
				Move(Vector3.left);
			}
			if (Input.GetKey(right)) {
				Move(Vector3.right);
			}
			if (Input.GetKey(up)) {
				Move(Vector3.up);
			}
			if (Input.GetKey(down)) {
				Move(Vector3.down);
			}
			//If no directions are being pressed, have the ship face forward
			else if (!Input.GetKey(left) && !Input.GetKey(right) && !Input.GetKey(up) && !Input.GetKey(down)) {
				desiredRotation = startRotation;
			}
		}
		//Controller input
		else {
			if (thisCharacter.player.device.LeftStick) {
				Move(thisCharacter.player.device.LeftStick.Vector);
			}
			else {
				desiredRotation = startRotation;
			}
		}
	}

	protected virtual void FixedUpdate() {
		if (!movementDisabled) {
			ClampDesiredPosition();
		}
		
		transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.fixedDeltaTime*shipLerpSpeed);

		if (!(movementDisabled || (verticalMovespeed == 0 && horizontalMovespeed == 0))) {
			transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.fixedDeltaTime * shipTurnLerpSpeed);
		}
	}
	//TODO 3/7/17: Can't this just be done purely in worldspace coordinates?
	protected void ClampDesiredPosition() {
		Vector3 desiredViewportPos = renderCamera.WorldToViewportPoint(desiredPosition);

		//Clamp x-direction
		if (desiredViewportPos.x < viewportMinX) {
			desiredPosition.x = renderCamera.ViewportToWorldPoint(new Vector3(viewportMinX, desiredViewportPos.y, desiredViewportPos.z)).x;
		}
		else if (desiredViewportPos.x > viewportMaxX) {
			desiredPosition.x = renderCamera.ViewportToWorldPoint(new Vector3(viewportMaxX, desiredViewportPos.y, desiredViewportPos.z)).x;
		}
		//Clamp y-direction
		if (desiredViewportPos.y < viewportMinY) {
			desiredPosition.y = renderCamera.ViewportToWorldPoint(new Vector3(desiredViewportPos.x, viewportMinY, desiredViewportPos.z)).y;
		}
		else if (desiredViewportPos.y > viewportMaxY) {
			desiredPosition.y = renderCamera.ViewportToWorldPoint(new Vector3(desiredViewportPos.x, viewportMaxY, desiredViewportPos.z)).y;
		}
	}
	public void Move(Vector2 moveDirection) {
		Move(new Vector3(moveDirection.x, moveDirection.y, 0));
	}
	public void Move(Vector3 moveVector) {
		moveVector.x *= horizontalMovespeed * Time.deltaTime;
		moveVector.y *= verticalMovespeed * Time.deltaTime;
		desiredPosition += moveVector;

		//Turn the ship slightly
		float dotValue = Vector3.Dot(moveVector, dotVector);
		int sign = 0;
		//0.01 because of floating point inaccuracies
		if (dotValue > 0.01f) {
			sign = 1;
		}
		else if (dotValue < -0.01f) {
			sign = -1;
		}
		float turnAngle = maxTurnAngle * Mathf.Abs(Vector3.Dot(moveVector, Vector3.up));
		desiredRotation = Quaternion.Euler(startRotation.eulerAngles + new Vector3(0, 0, sign*turnAngle));
	}

	public void SlowPlayer(float percentOfNormalMovespeed, float duration=0.2f, bool permaSlow=false) {
		//print("Slowed to " + percentOfNormalMovespeed);
		verticalMovespeed = vertMovespeedDefault * percentOfNormalMovespeed;
		horizontalMovespeed = horizMovespeedDefault * percentOfNormalMovespeed;
		if (!permaSlow) {
			StartCoroutine(RestoreSpeedCoroutine(duration));
		}
	}

	public void RestoreSpeed() {
		verticalMovespeed = vertMovespeedDefault;
		horizontalMovespeed = horizMovespeedDefault;
	}

	IEnumerator RestoreSpeedCoroutine(float duration=0.2f) {
		yield return new WaitForSeconds(duration);
		RestoreSpeed();
	}

	public Vector3 GetVelocity() {
		ClampDesiredPosition();

		Vector3 moveVector = Vector3.Lerp(transform.position, desiredPosition, Time.fixedDeltaTime*shipLerpSpeed) - transform.position; // m
		moveVector /= Time.fixedDeltaTime; // m/s
		return moveVector;
	}

	public void SetBaseSpeed(float percent) {
		vertMovespeedDefault *= percent;
		horizMovespeedDefault *= percent;

		verticalMovespeed = vertMovespeedDefault;
		horizontalMovespeed = horizMovespeedDefault;
	}
}
