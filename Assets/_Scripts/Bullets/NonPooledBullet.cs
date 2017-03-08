using UnityEngine;
using System.Collections;

public class NonPooledBullet : Bullet {
	protected override void DestroyThisBullet() {
		Destroy(gameObject);
	}
}
