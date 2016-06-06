using UnityEngine;
using System.Collections;

public class VampireShip : PlayerShip {
	float lifeRegen = 2f;		//Health regained per second

	new void Start() {
		base.Start();
		maxHealth = 100;
		health = maxHealth;
	}

	// Use this for initialization
	void Awake () {
		typeOfShip = ShipType.vampire;
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
		health += lifeRegen * Time.fixedDeltaTime;
		if (health > maxHealth) {
			health = maxHealth;
		}
	}
}
