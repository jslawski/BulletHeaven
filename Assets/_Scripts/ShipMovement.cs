using UnityEngine;
using System.Collections;

public class ShipMovement : MonoBehaviour {
	float shipLerpSpeed = 0.275f;               //Percent ship lerps towards desired position each FixedUpdate()
	float vertMovespeedDefault = 13f;
	float horizMovespeedDefault = 10.5f;
	float verticalMovespeed;					//Speed at which the player can move up and down
	float horizontalMovespeed;					//Speed at which the player can move right to left

	float shipTurnLerpSpeed = 0.1f;             //Percent ship lerps towards the desired rotation each FixedUpdate()
	float maxTurnAngle = 10f;                   //Maximum amount a ship can toward in a certain direction

	public float viewportMinX;
	public float viewportMaxX;
	public float viewportMinY;
	public float viewportMaxY;

	Vector3 desiredPosition;					//The position that the transform lerps towards each FixedUpdate()
	Quaternion startRotation;					//The beginning rotation of the ship
	Quaternion desiredRotation;					//The rotation that the transform lerps towards each FixedUpdate()
	Vector3 dotVector;							//Used to determine which way the ship should turn when moving up and down

	// Use this for initialization
	void Start() {
		verticalMovespeed = vertMovespeedDefault;
		horizontalMovespeed = horizMovespeedDefault;

		desiredPosition = transform.position;
		startRotation = transform.rotation;
		desiredRotation = startRotation;
		dotVector = -transform.right;
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKey("a")) {
			Move(Vector3.left * horizontalMovespeed * Time.deltaTime);
		}
		if (Input.GetKey("d")) {
			Move(Vector3.right * horizontalMovespeed * Time.deltaTime);
		}
		if (Input.GetKey("w")) {
			Move(Vector3.up * verticalMovespeed * Time.deltaTime);
		}
		if (Input.GetKey("s")) {
			Move(Vector3.down * verticalMovespeed * Time.deltaTime);
		}
		//If no directions are being pressed, have the ship face forward
		else if (!Input.GetKey("a") && !Input.GetKey("d") && !Input.GetKey("w") && !Input.GetKey("s")) {
			desiredRotation = startRotation;
		}
	}

	void FixedUpdate() {
		ClampDesiredPosition();
         transform.position = Vector3.Lerp(transform.position, desiredPosition, shipLerpSpeed);

		transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, shipLerpSpeed);
	}
	void ClampDesiredPosition() {
		Vector3 desiredViewportPos = Camera.main.WorldToViewportPoint(desiredPosition);

		//Clamp x-direction
		if (desiredViewportPos.x < viewportMinX) {
			desiredPosition.x = Camera.main.ViewportToWorldPoint(new Vector3(viewportMinX, desiredViewportPos.y, desiredViewportPos.z)).x;
		}
		else if (desiredViewportPos.x > viewportMaxX) {
			desiredPosition.x = Camera.main.ViewportToWorldPoint(new Vector3(viewportMaxX, desiredViewportPos.y, desiredViewportPos.z)).x;
		}
		//Clamp y-direction
		if (desiredViewportPos.y < viewportMinY) {
			desiredPosition.y = Camera.main.ViewportToWorldPoint(new Vector3(desiredViewportPos.x, viewportMinY, desiredViewportPos.z)).y;
		}
		else if (desiredViewportPos.y > viewportMaxY) {
			desiredPosition.y = Camera.main.ViewportToWorldPoint(new Vector3(desiredViewportPos.x, viewportMaxY, desiredViewportPos.z)).y;
		}
	}

	void Move(Vector3 moveVector) {
		desiredPosition += moveVector;
		Vector3 temp = transform.forward;

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
		desiredRotation = Quaternion.Euler(startRotation.eulerAngles + new Vector3(0, 0, sign*maxTurnAngle));
	}

	public void SlowPlayer(float percentOfNormalMovespeed) {
		verticalMovespeed = vertMovespeedDefault * percentOfNormalMovespeed;
		horizontalMovespeed = horizMovespeedDefault * percentOfNormalMovespeed;
	}
	public void RestoreSpeed() {
		verticalMovespeed = vertMovespeedDefault;
		horizontalMovespeed = horizMovespeedDefault;
	}
}
