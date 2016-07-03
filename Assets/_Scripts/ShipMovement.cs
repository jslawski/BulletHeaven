using UnityEngine;
using System.Collections;

public class ShipMovement : MonoBehaviour {
	public PlayerShip thisPlayer;
	public Camera renderCamera;
	float shipLerpSpeed = 13.75f;               //Percent ship lerps towards desired position each FixedUpdate()
	protected float vertMovespeedDefault = 16f;
	protected float horizMovespeedDefault = 12.5f;
	protected float verticalMovespeed;                  //Speed at which the player can move up and down
	protected float horizontalMovespeed;					//Speed at which the player can move right to left

	float shipTurnLerpSpeed = 5f;             //Percent ship lerps towards the desired rotation each FixedUpdate()
	float maxTurnAngle = 15f;                   //Maximum amount a ship can toward in a certain direction

	public float viewportMinX;
	public float viewportMaxX;
	public float viewportMinY;
	public float viewportMaxY;

	[HideInInspector]
	public float worldSpaceMinX;
	[HideInInspector]
	public float worldSpaceMaxX;
	[HideInInspector]
	public float worldSpaceMinY;
	[HideInInspector]
	public float worldSpaceMaxY;

	protected Vector3 desiredPosition;                  //The position that the transform lerps towards each FixedUpdate()
	protected Quaternion startRotation;                 //The beginning rotation of the ship
	protected Quaternion desiredRotation;					//The rotation that the transform lerps towards each FixedUpdate()
	protected Vector3 dotVector;                          //Used to determine which way the ship should turn when moving up and down

	public bool movementDisabled = false;

	public KeyCode left, right, up, down;

	// Use this for initialization
	protected virtual void Awake() {
		renderCamera = Camera.main;
		thisPlayer = GetComponent<PlayerShip>();

		verticalMovespeed = vertMovespeedDefault;
		horizontalMovespeed = horizMovespeedDefault;

		Vector3 worldSpaceMin = renderCamera.ViewportToWorldPoint(new Vector3(viewportMinX, viewportMinY, 0));
		worldSpaceMinX = worldSpaceMin.x;
		worldSpaceMinY = worldSpaceMin.y;
		Vector3 worldSpacemax = renderCamera.ViewportToWorldPoint(new Vector3(viewportMaxX, viewportMaxY, 0));
		worldSpaceMaxX = worldSpacemax.x;
		worldSpaceMaxY = worldSpacemax.y;

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
		
		transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.fixedDeltaTime*shipLerpSpeed);

		transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.fixedDeltaTime*shipTurnLerpSpeed);
	}
	void ClampDesiredPosition() {
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
	protected void Move(Vector2 stickPosition) {
		Vector3 moveVector = new Vector3(stickPosition.x, stickPosition.y, 0);
		moveVector.x *= horizontalMovespeed * Time.deltaTime;
		moveVector.y *= verticalMovespeed * Time.deltaTime;
		Move(moveVector);
	}
	protected void Move(Vector3 moveVector) {
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
