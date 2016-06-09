using UnityEngine;
using System.Collections;
using PolarCoordinates;

public class SpreadShot : MonoBehaviour {
	public Player owningPlayer = Player.none;
	public Bullet bulletPrefab;
	int numBursts = 40;
	float bulletDelay = 0.05f;
	public Masochist masochistPlayer;

	// Use this for initialization
	void Start () {
	
	}
	
	public void FireBurst() {
		StartCoroutine(FireBurstCoroutine());
	}

	IEnumerator FireBurstCoroutine() {
		//Separation of bullets is smaller when the masochist has his aura up
		//Normal: 5 directions, Aura: 8 directions
		float firingSeparation = masochistPlayer.damageMultiplier == 1 ? 72 * Mathf.Deg2Rad : 45 * Mathf.Deg2Rad;
		float startingAngle = masochistPlayer.damageMultiplier == 1 ? 18 * Mathf.Deg2Rad : 0;
		float bulletOffset = 3f * Mathf.Deg2Rad;


		float bulletVelocity = 10f;

		for (int i = 0; i < numBursts; i++) {
			//Fire burst of bullets
			for (float curAngle = startingAngle; curAngle < startingAngle + (2 * Mathf.PI); curAngle += firingSeparation) {
				PolarCoordinate direction = new PolarCoordinate(1, curAngle);
				PolarCoordinate offset = new PolarCoordinate(1, bulletOffset);
				PolarCoordinate offsetDirection;

				//Bullet 1
				offsetDirection = new PolarCoordinate(1, curAngle + bulletOffset);
				Bullet curBullet = bulletPrefab.GetPooledInstance<Bullet>();
				curBullet.owningPlayer = owningPlayer;
				curBullet.transform.position = gameObject.transform.position;
				curBullet.GetComponent<PhysicsObj>().velocity = bulletVelocity * (offsetDirection.PolarToCartesian()).normalized;

				//Bullet 2
				offsetDirection = new PolarCoordinate(1, curAngle - bulletOffset);
				curBullet = bulletPrefab.GetPooledInstance<Bullet>();
				curBullet.owningPlayer = owningPlayer;
				curBullet.transform.position = gameObject.transform.position;
				curBullet.GetComponent<PhysicsObj>().velocity = bulletVelocity * (offsetDirection.PolarToCartesian()).normalized;
			}

			yield return new WaitForSeconds(bulletDelay);

		}

		Destroy(gameObject);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
