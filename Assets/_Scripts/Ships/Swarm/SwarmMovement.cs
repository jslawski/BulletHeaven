using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmMovement : ShipMovement {
	protected override void FixedUpdate() {
		if (!movementDisabled) {
			ClampDesiredPosition();
		}

		//YO this don't work though fix it kthnx - JDS 4/30/17
		foreach (Ship ship in thisCharacter.ships) {
			Vector3 desiredMoveDir = (desiredPosition - thisCharacter.transform.position).normalized;
			Vector3 positionInSwarm = (ship.transform.position - thisCharacter.transform.position);
			float speedMultiplier = Vector3.Dot(desiredMoveDir, positionInSwarm);

			transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.fixedDeltaTime * shipLerpSpeed * speedMultiplier);

			if (!(movementDisabled || (verticalMovespeed == 0 && horizontalMovespeed == 0))) {
				transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.fixedDeltaTime * shipTurnLerpSpeed);
			}
		}
	}
}
