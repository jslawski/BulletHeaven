using UnityEngine;
using System.Collections;
using PolarCoordinates;

public class RotatingCircleWave : MonoBehaviour {
	public int direction = 1;
	public Player owningPlayer;
	NonPooledBullet bulletPrefab;
	int numBulletsPerBurst = 12;
	float timeBetweenBursts = 0.1f;
	int numBurstsPerWave = 3;

	float bulletDamage = 1.5f;
	float bulletVelocity = 5f;

	float rotationSpeed = 180f;

	// Use this for initialization
	void Awake () {
		bulletPrefab = Resources.Load<NonPooledBullet>("Prefabs/NonPooledBullet");
    }

	IEnumerator Start() {
		//Fire burst
		for (int i = 0; i < numBurstsPerWave; i++) {
			float radDelta = (2f*Mathf.PI)/numBulletsPerBurst;

			//Fire bullet
			float curAngle = 0;
			while (curAngle < 2 * Mathf.PI) {
				PolarCoordinate direction = new PolarCoordinate(1, curAngle);
				NonPooledBullet newBullet = Instantiate(bulletPrefab, transform.position, new Quaternion()) as NonPooledBullet;
				newBullet.damage = bulletDamage;
				newBullet.owningPlayer = owningPlayer;
				newBullet.transform.SetParent(transform);
				newBullet.transform.position = gameObject.transform.position;
				newBullet.physics.velocity = bulletVelocity * direction.PolarToCartesian().normalized;

				curAngle += radDelta;
			}

			yield return new WaitForSeconds(timeBetweenBursts);
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(0, 0, 180 * Time.deltaTime));
	}
}
