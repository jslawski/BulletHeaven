using UnityEngine;
using System.Collections;

public class GlassCannon : PlayerShip {
	DualLasers dualLaserPrefab;
	ChargeShot chargeShotPrefab;

	KeyCode X, Y;

	new void Awake() {
		base.Awake();

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
		maxHealth = 110f;
		health = maxHealth;

		playerMovement.SetBaseSpeed(1.3f);
	}

	protected override void Update() {
		base.Update();

		if (playerShooting.shootingDisabled || GameManager.S.gameState != GameStates.playing) {
			return;
		}

		//Dual laser attack
		if (Input.GetKeyDown(X) && playerShooting.curAmmo != 0) {
			DualLasers dualLaser = Instantiate(dualLaserPrefab, transform.position, new Quaternion()) as DualLasers;
			dualLaser.owningPlayer = player;
			playerShooting.ExpendAttackSlot();
		}
		if (device != null) {
			if (device.Action3.WasPressed && playerShooting.curAmmo != 0) {
				DualLasers dualLaser = Instantiate(dualLaserPrefab, transform.position, new Quaternion()) as DualLasers;
				dualLaser.owningPlayer = player;
				playerShooting.ExpendAttackSlot();
			}
		}

		//Charge shot attack
		if (Input.GetKeyDown(Y) && playerShooting.curAmmo != 0) {
			ChargeShot chargeShot = Instantiate(chargeShotPrefab, transform.position, new Quaternion()) as ChargeShot;
			chargeShot.owningPlayer = player;
			chargeShot.player = this;
			playerShooting.ExpendAttackSlot();
		}
		if (device != null) {
			if (device.Action4.WasPressed && playerShooting.curAmmo != 0) {
				ChargeShot chargeShot = Instantiate(chargeShotPrefab, transform.position, new Quaternion()) as ChargeShot;
				chargeShot.owningPlayer = player;
				chargeShot.player = this;
				playerShooting.ExpendAttackSlot();
			}
		}
	}
}
