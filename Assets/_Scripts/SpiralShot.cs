using UnityEngine;
using System.Collections;
using PolarCoordinates;

public class SpiralShot : MonoBehaviour {

	public Player owningPlayer = Player.none;
	public Bullet bulletPrefab;
	int numBursts = 40;
	int numDirectionFlips = 7;

	// Use this for initialization
	void Start () {
	
	}

	public void FireBurst() {
		StartCoroutine(FireBurstCoroutine());
	}

	IEnumerator FireBurstCoroutine() {
		float firingSeparation = 60 * Mathf.Deg2Rad;
		float startingAngle = 0;
		int directionScalar = 1;
		float angleOffset = 10;
		float bulletVelocity = 5;


		for (int i = 0; i < numBursts; i++) {
			//Fire burst of bullets
			for (float curAngle = startingAngle; curAngle < startingAngle + (2 * Mathf.PI); curAngle += firingSeparation) {
				PolarCoordinate direction = new PolarCoordinate(1, curAngle);
				Bullet curBullet = bulletPrefab.GetPooledInstance<Bullet>();
				curBullet.owningPlayer = owningPlayer;
				curBullet.transform.position = gameObject.transform.position;
				curBullet.GetComponent<PhysicsObj>().velocity = bulletVelocity * direction.PolarToCartesian().normalized;
			}

			//Change direction if needed
			if ((i != 0) && (i % (numBursts / numDirectionFlips) == 0)) {
				directionScalar *= -1;
			}

			//Update starting angle with the offset
			startingAngle += angleOffset * Mathf.Deg2Rad * directionScalar;

			yield return new WaitForSeconds(0.15f);

		}

		
	}
}
