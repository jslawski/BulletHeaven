using UnityEngine;
using System.Collections;
using PolarCoordinates;

public class RotatingCircleWave : MonoBehaviour {
	public int direction = 1;
	public Player thisPlayer;
	public PlayerEnum owningPlayer;
	NonPooledBullet bulletPrefab;
	int numBulletsPerBurst = 13;
	public float timeBetweenBursts = 0.1f;
	public int numBurstsPerWave = 5;

	float bulletDamage = 1.5f;
	float bulletVelocity = 6.5f;

	float rotationSpeed = 20;
	float minRotationSpeed = 1f;

	float timeAlive = 0;
	float maxLifespan = 10f;

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
				if (!GameManager.S.inGame) {
					newBullet.thisPlayer = thisPlayer;
					newBullet.SetColor(thisPlayer.playerColor);
				}
				newBullet.transform.SetParent(transform);
				newBullet.transform.position = gameObject.transform.position;
				newBullet.physics.actOnLocalSpace = true;
				newBullet.curState = BulletState.parented;
				newBullet.physics.velocity = bulletVelocity * direction.PolarToCartesian().normalized;

				curAngle += radDelta;
			}

			yield return new WaitForSeconds(timeBetweenBursts);
		}
	}
	
	// Update is called once per frame
	void Update () {
		timeAlive += Time.deltaTime;
		float curRotSpeed = Mathf.Lerp(rotationSpeed, minRotationSpeed, timeAlive/maxLifespan);

		transform.Rotate(new Vector3(0, 0, direction * rotationSpeed * Time.deltaTime));

		if (timeAlive > maxLifespan) {
			Destroy(gameObject);
		}
	}
}
