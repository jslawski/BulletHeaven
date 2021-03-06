﻿using UnityEngine;
using System.Collections;
using PolarCoordinates;

public class AltCircleShot : MonoBehaviour, BombAttack {

	PlayerEnum _owningPlayer = PlayerEnum.none;
	public Player thisPlayer;
	public PlayerEnum owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
		}
	}

	public Bullet bulletPrefab;
	int numBursts = 18;
	int numBulletsPerBurst = 17;
	float bulletDelay = 0.2f;
	float bulletVelocity = 7;
	float bulletDamage = 1.5f;
	float accelerationFactor = -0.7f;

	// Use this for initialization
	void Start() {

	}

	public void FireBurst() {
		StartCoroutine(FireBurstCoroutine());
	}

	IEnumerator FireBurstCoroutine() {
		float radDelta = (2f*Mathf.PI)/numBulletsPerBurst;

		for (int i = 0; i < numBursts; i++) {
			float curAngle = ((i%2)*radDelta)/2f;
			while (curAngle < 2 * Mathf.PI) {
				PolarCoordinate direction = new PolarCoordinate(1, curAngle);
				Bullet curBullet = bulletPrefab.GetPooledInstance<Bullet>();
				curBullet.damage = bulletDamage;
				curBullet.owningPlayer = owningPlayer;
				if (!GameManager.S.inGame) {
					curBullet.SetColor(thisPlayer.playerColor);
					curBullet.thisPlayer = thisPlayer;
				}
				curBullet.transform.position = gameObject.transform.position;
				curBullet.physics.velocity = bulletVelocity * direction.PolarToCartesian().normalized;
				curBullet.physics.acceleration = accelerationFactor*bulletVelocity * direction.PolarToCartesian().normalized;
				curBullet.physics.acceleration += (i%2==0 ? 1 : -1) * 0.1f*Vector3.Cross(curBullet.physics.acceleration, Vector3.forward);

				curAngle += radDelta;
			}
			////Fire burst of bullets
			//for (float curAngle = 0; curAngle < startingAngle + (2 * Mathf.PI); curAngle += firingSeparation) {
			//	PolarCoordinate direction = new PolarCoordinate(1, curAngle);
			//	Bullet curBullet = bulletPrefab.GetPooledInstance<Bullet>();
			//	curBullet.damage = 1.5f;
			//	curBullet.owningPlayer = owningPlayer;
			//	curBullet.transform.position = gameObject.transform.position;
			//	curBullet.GetComponent<PhysicsObj>().velocity = bulletVelocity * direction.PolarToCartesian().normalized;
			//}

			////Change direction if needed
			//if ((i != 0) && (i % (numBursts / numDirectionFlips) == 0)) {
			//	directionScalar *= -1;
			//}

			////Update starting angle with the offset
			//startingAngle += angleOffset * Mathf.Deg2Rad * directionScalar;

			yield return new WaitForSeconds(bulletDelay);

		}

		Destroy(gameObject);
	}
}
