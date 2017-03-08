using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class TwinShipMovement : ShipMovement {
	public TwoAxisInputControl controlStick;

	// Update is called once per frame
	protected override void Update() {
		if (movementDisabled || GameManager.S.gameState != GameStates.playing) {
			desiredRotation = startRotation;
			return;
		}

		//TODO: Keyboard support?
		//if (thisCharacter.player.device == null) {
		//	if (Input.GetKey(left)) {
		//		Move(Vector3.left);
		//	}
		//	if (Input.GetKey(right)) {
		//		Move(Vector3.right);
		//	}
		//	if (Input.GetKey(up)) {
		//		Move(Vector3.up);
		//	}
		//	if (Input.GetKey(down)) {
		//		Move(Vector3.down);
		//	}
		//	//If no directions are being pressed, have the ship face forward
		//	else if (!Input.GetKey(left) && !Input.GetKey(right) && !Input.GetKey(up) && !Input.GetKey(down)) {
		//		desiredRotation = startRotation;
		//	}
		//}

		//Controller input
		if (controlStick) {
			Move(controlStick.Vector);
		}
		else {
			desiredRotation = startRotation;
		}
	}
}
