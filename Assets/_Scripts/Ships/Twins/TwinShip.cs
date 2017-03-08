using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinShip : Ship {

	protected override void Awake() {
		base.Awake();
		maxHealth = 120;
	}
}
