using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampireShip : Ship {
	float lifeRegen = 2f;       //Health regained per second

	[HideInInspector]
	public VampireShield shield;
	public bool shieldUp = false;

	protected override void Awake() {
		base.Awake();
		maxHealth = 110f;

		shield = Resources.Load<VampireShield>("Prefabs/VampireShield");
	}

	void FixedUpdate() {
		if (GameManager.S.gameState != GameStates.playing) {
			return;
		}

		health += lifeRegen * Time.fixedDeltaTime;
		if (health > maxHealth) {
			health = maxHealth;
		}
	}

	public void InstantiateShield() {
		if (shooting.curAmmo != 0 && !shieldUp) {
			VampireShield newShield = Instantiate(shield, transform.position, new Quaternion()) as VampireShield;
			newShield.transform.parent = gameObject.transform;
			newShield.thisPlayer = player;
			newShield.owningPlayer = playerEnum;
			newShield.ActivateShield();
			shooting.ExpendAttackSlot();
		}
		else {
			SoundManager.instance.Play("OutOfAmmo", 1);
		}
	}
}
