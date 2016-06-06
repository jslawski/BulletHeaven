using UnityEngine;
using System.Collections;

public class Masochist : PlayerShip {

	//JPS: This assumes that there will only every be one masochist on the battlefield
	//This will have to change if we don't like that limitation
	public static float damageMultiplier = 1f;

	void Awake() {
		typeOfShip = ShipType.masochist;
	}

	//Opted for a discrete method of doing a damage multiplier
	//Open to discuss the possibility of implementing a continuous method instead
	public override void TakeDamage(float damageIn) {
		base.TakeDamage(damageIn);

		//Determine the current damage multiplier
		float remainingHealthRatio = health / maxHealth;
		print("Remaining Health Ratio: " + remainingHealthRatio);

		//75% health remaining -> 20% damage increase
		if (remainingHealthRatio <= 0.75f) {
			damageMultiplier = 2f;
		}
		//50% health remaining -> 50% damage increase
		else if (remainingHealthRatio <= 0.5f) {
			damageMultiplier = 4f;
		}
		//25% health remaining -> 75% damage increase
		else if (remainingHealthRatio <= 0.25f) {
			damageMultiplier = 6f;
		}
	}
}
