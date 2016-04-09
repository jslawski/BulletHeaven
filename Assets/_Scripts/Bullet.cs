using UnityEngine;
using System.Collections;

public class Bullet : PooledObj {

	void OnTriggerEnter(Collider other) {
		if (other.tag != "KillZone") {
			return;
		}
		ReturnToPool();
	}
}
