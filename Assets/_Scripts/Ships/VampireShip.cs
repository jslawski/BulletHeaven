using UnityEngine;
using System.Collections;

public class VampireShip : PlayerShip {

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
}
