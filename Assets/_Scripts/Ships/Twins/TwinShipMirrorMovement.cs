using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinShipMirrorMovement : ShipMovement {
	ShipMovement leader;
	[SerializeField]
	Vector3 mirrorPoint = Vector3.zero;

	protected override void Awake() {
		base.Awake();

		mirrorPoint = new Vector3((worldSpaceMinX + worldSpaceMaxX) / 2f, (worldSpaceMinY + worldSpaceMaxY) / 2f, 0);
	}

	private void Start() {
		leader = thisCharacter.ships[0].movement;
	}

	protected override void Update() {
		if (movementDisabled || GameManager.S.gameState != GameStates.playing) {
			desiredRotation = startRotation;
			return;
		}

		desiredPosition = (2*mirrorPoint - leader.desiredPosition);
	}
}
