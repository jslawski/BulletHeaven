using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewShip : Ship {

	bool inFireChargeShotCoroutine = false;
	DualLasers dualLaserPrefab;
	ChargeShot chargeShotPrefab;
	MasochistShield masochistShieldPrefab;
	VampireShield vampireShieldPrefab;

	protected override void Awake() {
		base.Awake();

		dualLaserPrefab = Resources.Load<DualLasers>("Prefabs/DualLasers");
		chargeShotPrefab = Resources.Load<ChargeShot>("Prefabs/ChargeShot");
		masochistShieldPrefab = Resources.Load<MasochistShield>("Prefabs/MasochistShield");
		vampireShieldPrefab = Resources.Load<VampireShield>("Prefabs/VampireShield");
	}

	public override void TakeDamage(float damageIn) {
		if (invincible || dead) {
			return;
		}

		timeSinceTakenDamage = 0;
		if (!inDamageFlashCoroutine) {
			StartCoroutine(FlashOnDamage(damageIn));
		}
	}

	public void FireDualLasers() {
		DualLasers dualLaser = Instantiate(dualLaserPrefab, transform.position, new Quaternion()) as DualLasers;
		dualLaser.owningPlayer = playerEnum;
		dualLaser.SetColor(player.playerColor);
		dualLaser.thisPlayer = this.player;
	}

	public void FireChargeShot() {
		if (!inFireChargeShotCoroutine) {
			StartCoroutine(FireChargeShotCoroutine());
		}
	}
	IEnumerator FireChargeShotCoroutine() {
		inFireChargeShotCoroutine = true;
		ChargeShot chargeShot = Instantiate(chargeShotPrefab, transform.position, new Quaternion()) as ChargeShot;
		chargeShot.owningPlayer = playerEnum;
		chargeShot.playerShip = this;

		yield return new WaitForSeconds(3f);

		chargeShot.Fire();

		inFireChargeShotCoroutine = false;
	}

	public void UseMasochistShield() {
		MasochistShield newShield = Instantiate(masochistShieldPrefab, transform.position, new Quaternion()) as MasochistShield;
		newShield.transform.parent = gameObject.transform;
		newShield.thisPlayer = this.player;
		newShield.owningPlayer = playerEnum;
		newShield.ActivateShield();
	}

	public void UseVampireShield() {
		VampireShield newShield = Instantiate(vampireShieldPrefab, transform.position, new Quaternion()) as VampireShield;
		newShield.transform.parent = gameObject.transform;
		newShield.thisPlayer = this.player;
		newShield.hitboxOffset = transform.Find("Hitbox").localPosition.y;
		newShield.owningPlayer = playerEnum;
		newShield.ActivateShield();
	}
}
