using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassCannonShip : Ship {
	DualLasers dualLaserPrefab;
	ChargeShot chargeShotPrefab;

	protected override void Awake() {
		base.Awake();
		maxHealth = 145;

		dualLaserPrefab = Resources.Load<DualLasers>("Prefabs/DualLasers");
		chargeShotPrefab = Resources.Load<ChargeShot>("Prefabs/ChargeShot");

		// Slightly dim the ship's sprite so it doesn't halo on bloom effect
		shipSpriteDefaultColor = Color.white * 0.87f;
	}

	// Use this for initialization
	protected override void Start () {
		base.Start();

		movement.SetBaseSpeed(1.3f);
	}

	public void DualLaserAttack() {
		DualLasers dualLaser = Instantiate(dualLaserPrefab, transform.position, new Quaternion()) as DualLasers;
		dualLaser.owningPlayer = playerEnum;
	}

	public void ChargeShotAttack() {
		ChargeShot chargeShot = Instantiate(chargeShotPrefab, transform.position, new Quaternion()) as ChargeShot;
		chargeShot.owningPlayer = playerEnum;
		chargeShot.playerShip = this;
	}
}
