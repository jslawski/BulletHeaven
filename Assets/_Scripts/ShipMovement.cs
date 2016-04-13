using UnityEngine;
using System.Collections;

public class ShipMovement : MonoBehaviour {
	PlayerShip thisPlayer;
	float shipLerpSpeed = 0.275f;               //Percent ship lerps towards desired position each FixedUpdate()
	float vertMovespeedDefault = 16f;
	float horizMovespeedDefault = 12.5f;
	float verticalMovespeed;					//Speed at which the player can move up and down
	float horizontalMovespeed;					//Speed at which the player can move right to left

	float shipTurnLerpSpeed = 0.1f;             //Percent ship lerps towards the desired rotation each FixedUpdate()
	float maxTurnAngle = 15f;                   //Maximum amount a ship can toward in a certain direction

	public float viewportMinX;
	public float viewportMaxX;
	public float viewportMinY;
	public float viewportMaxY;

	public float worldSpaceMinX;
	public float worldSpaceMaxX;
	public float worldSpaceMinY;
	public float worldSpaceMaxY;

	Vector3 desiredPosition;					//The position that the transform lerps towards each FixedUpdate()
	Quaternion startRotation;					//The beginning rotation of the ship
	Quaternion desiredRotation;					//The rotation that the transform lerps towards each FixedUpdate()
	Vector3 dotVector;                          //Used to determine which way the ship should turn when moving up and down

	public bool movementDisabled = false;

	public KeyCode left, right, up, down;

	// Use this for initialization
	void Start() {
		thisPlayer = GetComponent<PlayerShip>();

		verticalMovespeed = vertMovespeedDefault;
		horizontalMovespeed = horizMovespeedDefault;

		Vector3 worldSpaceMin = Camera.main.ViewportToWorldPoint(new Vector3(viewportMinX, viewportMinY, 0));
		worldSpaceMinX = worldSpaceMin.x;
		worldSpaceMinY = worldSpaceMin.y;
		Vector3 worldSpacemax = Camera.main.ViewportToWorldPoint(new Vector3(viewportMaxX, viewportMaxY, 0));
		worldSpaceMaxX = worldSpacemax.x;
		worldSpaceMaxY = worldSpacemax.y;

		desiredPosition = transform.position;
		startRotation = transform.rotation;
		desiredRotation = startRotation;
		dotVector = -transform.right;
	}

	// Update is called once per frame
	void Update() {
		if (movementDisabled) {
			desiredRotation = startRotation;
			return;
		}

		if (thisPlayer.device == null) {
			if (Input.GetKey(left)) {
				Move(Vector3.left * horizontalMovespeed * Time.deltaTime);
			}
			if (Input.GetKey(right)) {
				Move(Vector3.right * horizontalMovespeed * Time.deltaTime);
			}
			if (Input.GetKey(up)) {
				Move(Vector3.up * verticalMovespeed * Time.deltaTime);
			}
			if (Input.GetKey(down)) {
				Move(Vector3.down * verticalMovespeed * Time.deltaTime);
			}
			//If no directions are being pressed, have the ship face forward
			else if (!Input.GetKey(left) && !Input.GetKey(right) && !Input.GetKey(up) && !Input.GetKey(down)) {
				desiredRotation = startRotation;
			}
		}
		//Controller input
		else {
			if (thisPlayer.device.LeftStick) {
				Move(thisPlayer.device.LeftStick.Vector);
			}
			else {
				desiredRotation = startRotation;
			}
		}
	}

	void FixedUpdate() {
		if (!movementDisabled) {
			ClampDesiredPosition();
		}
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
	void Move(Vector2 stickPosition) {
		Vector3 moveVector = new Vector3(stickPosition.x, stickPosition.y, 0);
		moveVector.x *= horizontalMovespeed * Time.deltaTime;
		moveVector.y *= verticalMovespeed * Time.deltaTime;
		Move(moveVector);
	}
	void Move(Vector3 moveVector) {
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

	public void SlowPlayer(float percentOfNormalMovespeed) {
		verticalMovespeed = vertMovespeedDefault * percentOfNormalMovespeed;
		horizontalMovespeed = horizMovespeedDefault * percentOfNormalMovespeed;
		StartCoroutine(RestoreSpeedCoroutine());
	}

	public void RestoreSpeed() {
		verticalMovespeed = vertMovespeedDefault;
		horizontalMovespeed = horizMovespeedDefault;
	}

	IEnumerator RestoreSpeedCoroutine() {
		yield return new WaitForSeconds(0.2f);
		RestoreSpeed();
	}
}
