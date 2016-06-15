using UnityEngine;
using System.Collections;
using PolarCoordinates;

public class SineShot : MonoBehaviour {
	public Player owningPlayer = Player.none;

	public Transform target;
	int bulletsPerBurst = 100;
	public SineBullet bulletPrefab;

	public void FireBurst() {
		StartCoroutine(FireBurstCoroutine());
	}

	IEnumerator FireBurstCoroutine() {
		if (owningPlayer == Player.none) {
			Debug.LogError("Sine shot does not have owning player set");
			yield break;
		}

		Player targetPlayer = (owningPlayer == Player.player1) ? Player.player2 : Player.player1;
		target = GameManager.S.players[(int)targetPlayer].transform;

		PolarCoordinate direction = new PolarCoordinate(1, target.position - gameObject.transform.position);

		float t = 0;
		for (int i = 0; i < bulletsPerBurst; i++) {
			t += Time.deltaTime;

			//Generate 2 waves
			GenerateBullet(direction, -1);
			GenerateBullet(direction, 1);
			
			yield return new WaitForFixedUpdate();
		}

		//Destroy this gameObject after the burst has been fired
		Destroy(gameObject);
	}

	void GenerateBullet(PolarCoordinate direction, int waveDirection) {
		SineBullet curBullet = bulletPrefab.GetPooledInstance<SineBullet>();
		curBullet.owningPlayer = owningPlayer;
		curBullet.transform.position = gameObject.transform.position;
		curBullet.GetComponent<PhysicsObj>().velocity = 10 * direction.PolarToCartesian().normalized;
		curBullet.ApplySineWave(waveDirection);
	}

}
