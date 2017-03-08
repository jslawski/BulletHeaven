using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralistShip : Ship {

	protected override void Awake() {
		base.Awake();
		maxHealth = 200f;
	}
}
