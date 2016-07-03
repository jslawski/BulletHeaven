using UnityEngine;
using System.Collections;
using PolarCoordinates;

public class ConeShot : MonoBehaviour, BombAttack {
	Player _owningPlayer = Player.none;

	public Player owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
			if (GameManager.S.inGame) {
				Player targetPlayer = (owningPlayer == Player.player1) ? Player.player2 : Player.player1;
				target = GameManager.S.players[(int)targetPlayer].transform;
				playerColor = GameManager.S.players[(int)value].playerColor;
			}
		}
	}

	public Color playerColor;
	public Transform target;
	int bulletsPerBurst = 15;
	int numLines = 5;
	public Bullet bulletPrefab;

	float bulletDelay = 0.1f;
	float bulletVelocity = 10f;
	float coneSpread = 30f * Mathf.Deg2Rad;

	public void FireBurst() {
		StartCoroutine(FireBurstCoroutine());
	}

	IEnumerator FireBurstCoroutine() {
		if (owningPlayer == Player.none) {
			Debug.LogError("Cone shot does not have owning player set");
			yield break;
		}

		//Set direction and separation between bullets
		PolarCoordinate direction = new PolarCoordinate(1, target.position - gameObject.transform.position);
		float bulletSeparation = coneSpread / numLines;

		for (int i = 0; i < bulletsPerBurst; i++) {
			//Start the new direction set at the edge of the cone
			PolarCoordinate newDirection = new PolarCoordinate(1, direction.angle - (Mathf.Floor(numLines / 2) * bulletSeparation));

			//Fire bullet wave
			for (int j = 0; j < numLines; j++) {
				Bullet curBullet = bulletPrefab.GetPooledInstance<Bullet>();
				curBullet.owningPlayer = owningPlayer;
				if (!GameManager.S.inGame) {
					curBullet.SetColor(playerColor);
				}
				curBullet.transform.position = gameObject.transform.position;
				curBullet.GetComponent<PhysicsObj>().velocity = bulletVelocity * newDirection.PolarToCartesian().normalized;
				newDirection.angle += bulletSeparation;
			}

			yield return new WaitForSeconds(bulletDelay);
		}

		//Destroy this gameObject after the burst has been fired
		Destroy(gameObject);
	}

}
