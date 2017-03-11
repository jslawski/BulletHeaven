using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Twins : Character {

	protected override void Awake() {
		base.Awake();

		characterType = CharactersEnum.twins;
	}

	// Use this for initialization
	void Start () {
		GetComponentInChildren<ButtonHelpUI>().SetButtons(false, false, false, false);
		(ships[0].movement as TwinShipMovement).controlStick = player.device.LeftStick;
		(ships[1].movement as TwinShipMovement).controlStick = player.device.RightStick;
	}

	private void Update() {
		//if (!player.device.LeftStick && !player.device.RightStick) {
		//	if (ships[0].transform.position.y < ships[1].transform.position.y) {
		//		(ships[0].movement as TwinShipMovement).controlStick = player.device.LeftStick;
		//		(ships[1].movement as TwinShipMovement).controlStick = player.device.RightStick;
		//	}
		//	else {
		//		(ships[0].movement as TwinShipMovement).controlStick = player.device.RightStick;
		//		(ships[1].movement as TwinShipMovement).controlStick = player.device.LeftStick;
		//	}
		//}
	}
}
