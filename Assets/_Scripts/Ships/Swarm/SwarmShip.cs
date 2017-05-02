using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmShip : Ship {

	protected override void Awake() {
		base.Awake();
		maxHealth = 40f;
	}
}
