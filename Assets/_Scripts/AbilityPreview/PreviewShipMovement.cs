using UnityEngine;
using System.Collections;

public class PreviewShipMovement : ShipMovement {
	public bool autoMove = false;
	int curDirection = 1;
	float reverseDirectionTime = .75f;
	float timeSinceDirectionChange = .375f;

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
