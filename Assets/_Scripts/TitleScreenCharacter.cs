using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenCharacter : Character {

	public override Ship GetClosestShip(Vector3 location) {
		return ship;
	}
}
