﻿using UnityEngine;
using System.Collections;

public class Masochist : PlayerShip {
	KeyCode Y;

	public MasochistShield shield;

	MasochistAura curAura;
	MasochistAura auraPrefab;
	public bool shieldUp = false;

	public float damageMultiplier = 1f;

	void Awake() {
		base.Awake();
		typeOfShip = ShipType.masochist;
		shield = Resources.Load<MasochistShield>("Prefabs/MasochistShield");
		auraPrefab = Resources.Load<MasochistAura>("Prefabs/MasochistAura");
		GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/MasochistShip/MShip6");
		GetComponentInChildren<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(
			"Images/MasochistShip/MShipAnimationController");

		Y = player == Player.player1 ? KeyCode.Alpha4 : KeyCode.Keypad4; 
	}

	//Opted for a discrete method of doing a damage multiplier
	//Open to discuss the possibility of implementing a continuous method instead
	public override void TakeDamage(float damageIn) {
		if (shieldUp) {
			return;
		}

		base.TakeDamage(damageIn);

		//Determine the current damage multiplier
		float remainingHealthRatio = health / maxHealth;

		//50% health remaining -> 50% damage increase
		if (remainingHealthRatio <= 0.5f && damageMultiplier == 1) {
			damageMultiplier = 1.5f;
			curAura = Instantiate(auraPrefab, transform.position, new Quaternion()) as MasochistAura;
			curAura.player = this;
		}
		else if (remainingHealthRatio > 0.5f && damageMultiplier == 1.5f) {
			Destroy(curAura.gameObject);
			damageMultiplier = 1f;
		}
	}

	protected override void Update() {
		base.Update();

		if (device != null) {
			//Activate a shield if the button was pressed
			if (device.Action4.WasPressed && playerShooting.curAmmo != 0 && !shieldUp) {
				MasochistShield newShield = Instantiate(shield, transform.position, new Quaternion()) as MasochistShield;
				newShield.transform.parent = gameObject.transform;
				newShield.thisPlayer = GetComponent<Masochist>();
				newShield.owningPlayer = player;
				newShield.ActivateShield();
				playerShooting.ExpendAttackSlot();
			}
		}
		else if (device == null) {
			if (Input.GetKeyDown(Y) && playerShooting.curAmmo != 0 && !shieldUp) {
				MasochistShield newShield = Instantiate(shield, transform.position, new Quaternion()) as MasochistShield;
				newShield.transform.parent = gameObject.transform;
				newShield.thisPlayer = GetComponent<Masochist>();
				newShield.owningPlayer = player;
				newShield.ActivateShield();
				playerShooting.ExpendAttackSlot();
			}
		}
	}
}
