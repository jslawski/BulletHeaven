using UnityEngine;
using System.Collections;
using PolarCoordinates;

public class TrailShot : MonoBehaviour, BombAttack {
	PlayerEnum _owningPlayer = PlayerEnum.none;

	public PlayerEnum owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
			if (GameManager.S.inGame) {
				PlayerEnum targetPlayer = (owningPlayer == PlayerEnum.player1) ? PlayerEnum.player2 : PlayerEnum.player1;
				target = GameManager.S.players[(int)targetPlayer].ship.transform;
			}
		}
	}

	public Player thisPlayer;
	public Transform target;
	public TrailBullet bulletPrefab;

	int numTrails = 5;
	int bulletsPerTrail = 20;
	float coneOfFiring = 15f * Mathf.Deg2Rad;
	float bulletDelay = 0.02f;
	float bulletVelocity = 5f;

	public void FireBurst() {
		StartCoroutine(FireBurstCoroutine());
	}

	IEnumerator FireBurstCoroutine() {
		if (owningPlayer == PlayerEnum.none) {
			Debug.LogError("Trail shot does not have owning player set");
			yield break;
		}

		PolarCoordinate direction = new PolarCoordinate(1, target.position - gameObject.transform.position);

		//Generate semi-random trails of bullets
		for (int i = 0; i < numTrails; i++) {
			StartCoroutine(GenerateTrail(direction));
			yield return new WaitForSeconds(Random.Range(0.1f, 0.6f));
			direction = new PolarCoordinate(1, target.position - gameObject.transform.position);
		}

		//Destroy this gameObject after the burst has been fired
		Destroy(gameObject);
	}

	IEnumerator GenerateTrail(PolarCoordinate direction) {
		//Determine direction of trail
		PolarCoordinate shootDirection = direction;
		shootDirection.angle += Random.Range(-coneOfFiring, coneOfFiring);

		//Keep track of the previously fired bullet
		Bullet prevBullet;

		//Fire the leading bullet that will home in on the target
		TrailBullet leadingBullet = bulletPrefab.GetPooledInstance<TrailBullet>();
		leadingBullet.owningPlayer = owningPlayer;
		if (!GameManager.S.inGame) {
			leadingBullet.thisPlayer = thisPlayer;
			leadingBullet.SetColor(thisPlayer.playerColor);
		}
		leadingBullet.transform.position = gameObject.transform.position;
		leadingBullet.GetComponent<PhysicsObj>().velocity = bulletVelocity * shootDirection.PolarToCartesian().normalized;
		leadingBullet.target = target;
		leadingBullet.leadingBullet = null;
		prevBullet = leadingBullet;
		leadingBullet.BeginHoming();
		yield return new WaitForSeconds(bulletDelay);

		//Fire the rest of the bullets that will follow the leading bullet
		for (int i = 1; i < bulletsPerTrail; i++) {
			TrailBullet curBullet = bulletPrefab.GetPooledInstance<TrailBullet>();
			curBullet.owningPlayer = owningPlayer;
			if (!GameManager.S.inGame) {
				curBullet.thisPlayer = thisPlayer;
				curBullet.SetColor(thisPlayer.playerColor);
			}
			curBullet.transform.position = gameObject.transform.position;
			curBullet.GetComponent<PhysicsObj>().velocity = bulletVelocity * shootDirection.PolarToCartesian().normalized;

			curBullet.target = target;
			curBullet.leadingBullet = prevBullet;
			prevBullet = curBullet;
			curBullet.BeginHoming();

			yield return new WaitForSeconds(bulletDelay);
		}
	}
}
