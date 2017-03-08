using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenShip : Ship {

	protected override void Start() {
		//Don't need to set bomb type or anything for TitleScreenShip
	}

	public override void TakeDamage(float damageIn) {
		return;
	}
}
