using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentedBullet : NonPooledBullet, SpecialBullet {

	public void ReturnBulletToNormal() {
		PhysicsObj parentPhysics = GetComponentInParent<PhysicsObj>();
		if (parentPhysics != null) {
			physics.velocity = parentPhysics.velocity;
			physics.acceleration = parentPhysics.acceleration;
		}
		transform.SetParent(null, true);
		curState = BulletState.none;
	}
}
