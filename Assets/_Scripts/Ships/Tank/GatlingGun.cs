using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingGun : MonoBehaviour {
	Ship thisShip;
	public Bullet bulletPrefab;

	private float maxDuration = 4f;
	private float timeBetweenShots = 0.03f;
	private float bulletVelocity = 15f;
	private float bulletAcceleration = 4f;
	private bool inGatlingMode = false;

	private void Awake() {
		bulletPrefab = Resources.Load<Bullet>("Prefabs/Bullets/Bullet");
	}

	// Use this for initialization
	void Start () {
		thisShip = GetComponent<Ship>();
	}
	
	// Update is called once per frame
	void Update () {
		if (thisShip.player.device != null && thisShip.player.device.Action3.WasPressed) {
			if (!inGatlingMode) {
				StartCoroutine(EnterGatlingMode());
			}
			else {
				EndGatlingMode();
			}
		}
	}

	IEnumerator EnterGatlingMode() {
		inGatlingMode = true;
		float timeElapsed = 0;
		float spread = 0;
		const float turnSpeed = 0.005f;
		const float spreadIncrease = 0.15f;

		Vector3 aimDirection = thisShip.transform.up;
		StartCoroutine(UpdateDurationBar());
		while (timeElapsed < maxDuration && inGatlingMode) {
			//Freeze the player's movement while in GatlingMode
			thisShip.movement.SlowPlayer(0, timeBetweenShots);
			timeElapsed += timeBetweenShots;
			spread = (spread * spreadIncrease) + spreadIncrease;

			Vector2 stickVector2 = thisShip.player.device.LeftStick.Vector;
			Vector3 stickVector3 = new Vector3(stickVector2.x, stickVector2.y, 0);
			aimDirection = Vector3.Lerp(aimDirection, stickVector3, turnSpeed);
			thisShip.transform.up = aimDirection;

			if (stickVector3.magnitude > 0.05f) {
				FireBullet(aimDirection, spread);
			}

			yield return new WaitForSeconds(timeBetweenShots);
		}
		inGatlingMode = false;
	}

	IEnumerator UpdateDurationBar() {
		if (thisShip.player.durationBar == null) yield break;

		float timeElapsed = 0;
		while (inGatlingMode) {
			timeElapsed += Time.deltaTime;
			float t = timeElapsed / maxDuration;

			thisShip.player.durationBar.SetPercent(1 - t);
			yield return null;
		}
		thisShip.player.durationBar.SetPercent(0);
	}

	void EndGatlingMode() {
		thisShip.movement.RestoreSpeed();
		inGatlingMode = false;
	}

	void FireBullet(Vector3 fireDirection, float spread) {
		Bullet newBullet = Instantiate(bulletPrefab, transform.position + (thisShip.transform.up * 0.5f), new Quaternion());

		//Apply spread to each bullet fired
		float spreadApplied = Random.Range(0, spread);
		fireDirection += (new Vector3(-fireDirection.y, fireDirection.x) / fireDirection.magnitude) * spreadApplied;
		newBullet.physics.velocity = fireDirection.normalized * bulletVelocity;
		newBullet.physics.acceleration = fireDirection.normalized * bulletAcceleration;
		newBullet.owningPlayer = thisShip.playerEnum;
	}
}
