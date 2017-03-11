using UnityEngine;
using System.Collections;

public class GlassCannon : Character {
	GlassCannonShip gcShip;

	protected override void Awake() {
		base.Awake();

		characterType = CharactersEnum.glassCannon;
		bulletShape = BulletShapes.roundedSquare;
		gcShip = ship as GlassCannonShip;
	}

	// Use this for initialization
	void Start () {
		GetComponentInChildren<ButtonHelpUI>().SetButtons(false, false, true, true);
	}

	void Update() {
		if (gcShip.shooting.shootingDisabled || GameManager.S.gameState != GameStates.playing) {
			return;
		}

		//Dual laser attack
		if (Input.GetKeyDown(X)) {
			gcShip.DualLaserAttack();
		}
		if (player.device != null) {
			if (player.device.Action3.WasPressed) {
				gcShip.DualLaserAttack();
			}
		}

		//Charge shot attack
		if (Input.GetKeyDown(Y)) {
			gcShip.ChargeShotAttack();
		}
		if (player.device != null) {
			if (player.device.Action4.WasPressed) {
				gcShip.ChargeShotAttack();
			}
		}
	}

	public override Ship GetClosestShip(Vector3 location) {
		return ship;
	}
}
