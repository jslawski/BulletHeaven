using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenCharacter : Character {

    protected override void Awake() {
        base.Awake();

        this.bulletShape = (BulletShapes)Random.Range(0, (int)BulletShapes.numShapes);
    }

    public override Ship GetClosestShip(Vector3 location) {
		return ship;
	}
}
