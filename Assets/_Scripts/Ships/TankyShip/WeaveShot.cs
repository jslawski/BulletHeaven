using UnityEngine;
using System.Collections;
using PolarCoordinates;

public class WeaveShot : MonoBehaviour, BombAttack {
	PlayerEnum _owningPlayer = PlayerEnum.none;

	public Player thisPlayer;
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

	public Transform target;
	int bulletsPerBurst = 100;
	public Bullet bulletPrefab;
	int bulletsPerWeave = 20;
	float bulletDelay = 0.05f;
	float bulletVelocity = 7f;

	public void FireBurst() {
		StartCoroutine(FireBurstCoroutine());
	}

	IEnumerator FireBurstCoroutine() {
		if (owningPlayer == PlayerEnum.none) {
			Debug.LogError("Weave shot does not have owning player set");
			yield break;
		}

		PolarCoordinate direction = new PolarCoordinate(1, target.position - gameObject.transform.position);
		PolarCoordinate shootDir1 = new PolarCoordinate(1, direction.angle + 90 * Mathf.Deg2Rad);
		PolarCoordinate shootDir2 = new PolarCoordinate(1, direction.angle - 90 * Mathf.Deg2Rad);

		//Weaves will span the 180 degree area that is perpendicular to the target
		float bulletSeparation = 180 / bulletsPerWeave * Mathf.Deg2Rad;

		int curDirection = 1;

		for (int i = 0; i < bulletsPerBurst; i++) {
			//Fire bullet from first weave
			Bullet curBullet = bulletPrefab.GetPooledInstance<Bullet>();
			curBullet.owningPlayer = owningPlayer;
			if (!GameManager.S.inGame) {
				curBullet.SetColor(thisPlayer.playerColor);
				curBullet.thisPlayer = thisPlayer;
			}
			curBullet.transform.position = gameObject.transform.position;
			curBullet.GetComponent<PhysicsObj>().velocity = bulletVelocity * shootDir1.PolarToCartesian().normalized;

			//Fire bullet from second weave
			curBullet = bulletPrefab.GetPooledInstance<Bullet>();
			curBullet.owningPlayer = owningPlayer;
			if (!GameManager.S.inGame) {
				curBullet.SetColor(thisPlayer.playerColor);
				curBullet.thisPlayer = thisPlayer;
			}
			curBullet.transform.position = gameObject.transform.position;
			curBullet.GetComponent<PhysicsObj>().velocity = bulletVelocity * shootDir2.PolarToCartesian().normalized;

			//Adjust the angles for the next bullets
			shootDir1.angle += bulletSeparation * curDirection;
			shootDir2.angle -= bulletSeparation * curDirection;

			//Reverse direction
			if (i % bulletsPerWeave == 0) {
				curDirection *= -1;
			}

			yield return new WaitForSeconds(bulletDelay);
		}

		//Destroy this gameObject after the burst has been fired
		Destroy(gameObject);
	}
}
