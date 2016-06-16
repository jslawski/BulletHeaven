using UnityEngine;
using System.Collections;

public class VampireShip : PlayerShip {
	float lifeRegen = 2f;		//Health regained per second

	new void Start() {
		base.Start();
		maxHealth = 85f;
		health = maxHealth;
	}

	// Use this for initialization
	new void Awake () {
		base.Awake();
		typeOfShip = ShipType.vampire;
		GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/VampireShip/VampireShip6");
		GetComponentInChildren<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(
			"Images/VampireShip/VampireShipAnimationController");
	}
	
	public void PercentHealthCost(float percent, bool ofCurrentHealth=true) {
		//Subtract percent% of current health from the player
		if (ofCurrentHealth) {
			health -= percent * health;
		}
		//Subtract percent% of total health from the player
		else {
			health -= percent * maxHealth;
		}
	}

	void FixedUpdate() {
		if (dead) {
			return;
		}
		health += lifeRegen * Time.fixedDeltaTime;
		if (health > maxHealth) {
			health = maxHealth;
		}
	}
}
