using UnityEngine;
using System.Collections;

public class GlassCannon : PlayerShip {
	DualLasers dualLaserPrefab;
	ChargeShot chargeShotPrefab;

	KeyCode A, B;

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

		A = (player == Player.player1) ? KeyCode.Alpha1 : KeyCode.Keypad1;
		B = (player == Player.player1) ? KeyCode.Alpha2 : KeyCode.Keypad2;
		maxHealth = 110f;
		health = maxHealth;

		playerMovement.SetBaseSpeed(1.3f);
	}

	protected override void Update() {
		base.Update();

		//Dual laser attack
		if (Input.GetKeyDown(A)) {
			DualLasers dualLaser = Instantiate(dualLaserPrefab, transform.position, new Quaternion()) as DualLasers;
			dualLaser.owningPlayer = player;
		}
		if (device != null) {
			if (device.Action1.WasPressed) {
				DualLasers dualLaser = Instantiate(dualLaserPrefab, transform.position, new Quaternion()) as DualLasers;
				dualLaser.owningPlayer = player;
			}
		}

		//Charge shot attack
		if (Input.GetKeyDown(B)) {
			ChargeShot chargeShot = Instantiate(chargeShotPrefab, transform.position, new Quaternion()) as ChargeShot;
			chargeShot.owningPlayer = player;
			chargeShot.player = this;
		}
		if (device != null) {
			if (device.Action2.WasPressed) {
				ChargeShot chargeShot = Instantiate(chargeShotPrefab, transform.position, new Quaternion()) as ChargeShot;
				chargeShot.owningPlayer = player;
				chargeShot.player = this;
			}
		}
	}
}
