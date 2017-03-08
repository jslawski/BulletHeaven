using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankShip : Ship {
	float damageReduction = 0.3f;       //Percent of incoming damage that will be ignored

	protected override void Awake() {
		base.Awake();

		maxHealth = 300;
	}

	// Use this for initialization
	protected override void Start () {
		base.Start();

		movement.SetBaseSpeed(0.6f);
	}

	public override void TakeDamage(float damageIn) {
		if (damageIn > 0) {
			damageIn *= (1f - damageReduction);
		}
		base.TakeDamage(damageIn);
	}
}
