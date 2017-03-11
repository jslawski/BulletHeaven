using UnityEngine;
using System.Collections;

public class Vampire : Character {
	[HideInInspector]
	public VampireShip vShip;

	void Start() {
		GetComponentInChildren<ButtonHelpUI>().SetButtons(false, false, false, true);
	}

	// Use this for initialization
	protected override void Awake () {
		base.Awake();

		characterType = CharactersEnum.vampire;
		bulletShape = BulletShapes.triangle;
		vShip = ship as VampireShip;
	}

	void Update() {
		if (ship.shooting.shootingDisabled || GameManager.S.gameState != GameStates.playing) {
			return;
		}

		if (player.device != null) {
			//Activate a shield if the button was pressed
			if (player.device.Action4.WasPressed) {
				vShip.InstantiateShield();
			}
		}
		else if (player.device == null) {
			if (Input.GetKeyDown(Y)) {
				vShip.InstantiateShield();
			}
		}
	}

	public override Ship GetClosestShip(Vector3 location) {
		return ship;
	}
}
