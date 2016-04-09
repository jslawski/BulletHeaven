using UnityEngine;
using System.Collections;

public class ShipMovement : MonoBehaviour {
	float shipLerpSpeed = 0.275f;				//Percent ship lerps towards desired position each FixedUpdate()
	float verticalMovespeed = 13f;				//Speed at which the player can move up and down
	float horizontalMovespeed = 10f;			//Speed at which the player can move right to left

	Vector3 desiredPosition;
	Quaternion desiredRotation;

	// Use this for initialization
	void Start () {
		desiredPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey("w")) {
			Move(Vector3.up * verticalMovespeed * Time.deltaTime);
		}
		if (Input.GetKey("s")) {
			Move(Vector3.down * verticalMovespeed * Time.deltaTime);
		}
		if (Input.GetKey("a")) {
			Move(Vector3.left * horizontalMovespeed * Time.deltaTime);
		}
		if (Input.GetKey("d")) {
			Move(Vector3.right * horizontalMovespeed * Time.deltaTime);
		}
	}

	void FixedUpdate() {
		transform.position = Vector3.Lerp(transform.position, desiredPosition, shipLerpSpeed);
	}

	void Move(Vector3 moveVector) {
		desiredPosition += moveVector;
	}
}
