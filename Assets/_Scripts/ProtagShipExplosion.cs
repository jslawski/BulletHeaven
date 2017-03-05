using UnityEngine;
using System.Collections;

public class ProtagShipExplosion : MonoBehaviour {
	Bullet bulletPrefab;

	float bulletMovespeed = 10f;
	float bulletAcceleration = 2f;

	int numBullets = 100;
	int numBulletsPerFrame = 5;

	float offscreeLeeway = 0.5f;		//How far a ship is allowed to be offscreen and still explode

	// Use this for initialization
	IEnumerator Start () {
		if (!IsOnScreen()) {
			Destroy(gameObject);
		}

		bulletPrefab = Resources.Load<Bullet>("Prefabs/Bullets/ProtagBullet");
		int bulletsFired = 0;
		while (bulletsFired < numBullets) {
			for (int i = 0; i < numBulletsPerFrame; i++) {
				Vector2 randCircle = Random.insideUnitCircle;
				Vector3 spawnPos = transform.position + new Vector3(randCircle.x, randCircle.y, 0);

				Bullet newBullet = bulletPrefab.GetPooledInstance<Bullet>(transform.position);
				newBullet.owningPlayer = PlayerEnum.none;
				newBullet.physics.velocity = (transform.position - spawnPos).normalized * bulletMovespeed;
				newBullet.physics.acceleration = (transform.position - spawnPos).normalized * bulletAcceleration;
			}

			bulletsFired += numBulletsPerFrame;
			yield return null;
		}
	}
	
	bool IsOnScreen() {
		Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
		if (viewportPos.x < -offscreeLeeway || viewportPos.x > 1 + offscreeLeeway ||
			viewportPos.y < -offscreeLeeway || viewportPos.y > 1 + offscreeLeeway) {
			return false;
		}
		else {
			return true;
		}
	}
}
