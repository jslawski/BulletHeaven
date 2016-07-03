using UnityEngine;
using System.Collections;

public class PreviewShipMovement : ShipMovement {
	[SerializeField]
	bool _autoMove = false;
	public bool autoMove {
		get {
			return _autoMove;
		}
		set {
			_autoMove = value;
			if (!value) {
				desiredPosition = startPos;
			}
			else {
				timeSinceDirectionChange = reverseDirectionTime / 2f;
			}
		}
	}
	int curDirection = 1;
	float reverseDirectionTime = 1;
	float timeSinceDirectionChange = .5f;

	Vector3 startPos;

	// Use this for initialization
	protected override void Awake() {
		thisPlayer = GetComponent<PlayerShip>();

		startPos = transform.position;

		vertMovespeedDefault *= 0.6f;
		horizMovespeedDefault *= 0.6f;

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

	protected override void Update() {
		if (movementDisabled || GameManager.S.gameState != GameStates.shipSelect) {
			desiredRotation = startRotation;
			return;
		}


		if (autoMove) {
			timeSinceDirectionChange += Time.deltaTime;
			if (timeSinceDirectionChange > reverseDirectionTime) {
				curDirection *= -1;
				timeSinceDirectionChange = 0;
			}

			Move(Vector3.up * verticalMovespeed * Time.deltaTime * curDirection);
		}
	}
}
