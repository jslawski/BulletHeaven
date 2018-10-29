using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralistShip : Ship {

	protected override void Awake() {
		base.Awake();
		maxHealth = 200f;

		// Slightly dim the ship's sprite so it doesn't halo on bloom effect
		shipSpriteDefaultColor = Color.white * 0.87f;
	}
}
