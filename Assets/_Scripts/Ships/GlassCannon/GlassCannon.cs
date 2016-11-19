using UnityEngine;
using System.Collections;

public class GlassCannon : PlayerShip {
	DualLasers dualLaserPrefab;
	ChargeShot chargeShotPrefab;

	KeyCode X, Y;

	new void Awake() {
		base.Awake();

		maxHealth = 145f;

		dualLaserPrefab = Resources.Load<DualLasers>("Prefabs/DualLasers");
		chargeShotPrefab = Resources.Load<ChargeShot>("Prefabs/ChargeShot");
		GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/GlassCannonShip/GCShip6");
		GetComponentInChildren<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(
			"Images/GlassCannonShip/GCShipAnimationController");
	}

	// Use this for initialization
	new void Start () {
		base.Start();

		X = (player == Player.player1) ? KeyCode.Alpha3 : KeyCode.Keypad3;
		Y = (player == Player.player1) ? KeyCode.Alpha4 : KeyCode.Keypad4;

		GetComponentInChildren<ButtonHelpUI>().SetButtons(false, false, true, true);

		health = maxHealth;

		playerMovement.SetBaseSpeed(1.3f);
	}

	protected override void Update() {
		base.Update();

		if (playerShooting.shootingDisabled || GameManager.S.gameState != GameStates.playing) {
			return;
		}

		//Dual laser attack
		if (Input.GetKeyDown(X)) {
			DualLasers dualLaser = Instantiate(dualLaserPrefab, transform.position, new Quaternion()) as DualLasers;
			dualLaser.owningPlayer = player;
		}
		if (device != null) {
			if (device.Action3.WasPressed) {
				DualLasers dualLaser = Instantiate(dualLaserPrefab, transform.position, new Quaternion()) as DualLasers;
				dualLaser.owningPlayer = player;
			}
		}

		//Charge shot attack
		if (Input.GetKeyDown(Y)) {
			ChargeShot chargeShot = Instantiate(chargeShotPrefab, transform.position, new Quaternion()) as ChargeShot;
			chargeShot.owningPlayer = player;
			chargeShot.player = this;
		}
		if (device != null) {
			if (device.Action4.WasPressed) {
				ChargeShot chargeShot = Instantiate(chargeShotPrefab, transform.position, new Quaternion()) as ChargeShot;
				chargeShot.owningPlayer = player;
				chargeShot.player = this;
			}
		}
	}
}
