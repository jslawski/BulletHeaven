using UnityEngine;
using System.Collections;

public class TankyShip : PlayerShip {
	float damageReduction = 0.3f;		//Percent of incoming damage that will be ignored

	new void Awake() {
		base.Awake();
		typeOfShip = ShipType.tank;
		GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/TankyShip/TShip6");
		GetComponentInChildren<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(
			"Images/TankyShip/TShipAnimationController");
	}

	// Use this for initialization
	new void Start () {
		base.Start();
		maxHealth = 300;
		health = maxHealth;

		playerMovement.SetBaseSpeed(0.6f);
	}

	public override void TakeDamage(float damageIn) {
		if (damageIn > 0) {
			damageIn *= (1f - damageReduction);
		}
		base.TakeDamage(damageIn);
	}
}
