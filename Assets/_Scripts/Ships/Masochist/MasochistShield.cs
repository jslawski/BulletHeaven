using UnityEngine;
using System.Collections;
using PolarCoordinates;
using System.Collections.Generic;

public class MasochistShield : MonoBehaviour {
	SphereCollider thisCollider;
	SpriteRenderer shieldSprite;
	Player otherPlayer;

	List<PhysicsObj> absorbedBullets;
	int maxBulletCount = 100;

	float rotationSpeed_c = 100f;
	public Masochist thisPlayer;
	float shieldDuration = 1.5f;

	bool shooting = false;

	public Player _owningPlayer = Player.none;
	public Player owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
		}
	}

	IEnumerator Start() {
		thisCollider = GetComponent<SphereCollider>();
		shieldSprite = GetComponentInChildren<SpriteRenderer>();
		absorbedBullets = new List<PhysicsObj>();

		float timeElapsed = 0;
		while (timeElapsed < shieldDuration) {
			timeElapsed += Time.deltaTime;

			thisPlayer.durationBar.SetPercent(1 - timeElapsed / shieldDuration);

			yield return null;
		}
		thisPlayer.durationBar.SetPercent(0);
	}

	// Use this for initialization
	public void ActivateShield () {
		SoundManager.instance.Play("ShieldUp");
		thisPlayer.shieldUp = true;
		Color shieldColor = thisPlayer.playerColor;
		shieldColor.a = 180f / 255f;
		GetComponentInChildren<SpriteRenderer>().color = shieldColor;
		StartCoroutine(RotateShield());
		
	}

	IEnumerator RotateShield() {
		for (float i = 0; i < shieldDuration; i+= Time.fixedDeltaTime) {
			gameObject.transform.Rotate(new Vector3(0, 0, rotationSpeed_c * Time.fixedDeltaTime));
			yield return new WaitForFixedUpdate();
		}
		StartCoroutine(FireBullets());
	}

	IEnumerator FireBullets() {
		float sprayRange = 1.5f;
		float reflectionVelocity = thisPlayer.damageMultiplier == 1 ? 20f : 30f;

		shooting = true;

		//Remove the shield visual while shooting.  The player can be hit at this point
		shieldSprite.enabled = false;
		thisPlayer.shieldUp = false;

		Vector3 reflectionVector = GameManager.S.players[(int)owningPlayer].player == Player.player1 ? Vector3.right : Vector3.left;
		//Fire each bullet
		foreach (PhysicsObj bullet in absorbedBullets) {
			if (bullet == null) {
				continue;
			}

			Vector3 bulletPosition = bullet.gameObject.transform.position;
			bullet.gameObject.GetComponent<Bullet>().curState = BulletState.none;

			//Determine shooting vector
			Vector3 sprayVector = new Vector3(0, Random.Range(-sprayRange, sprayRange), 0);

			//Shoot the bullet back at the opponent
			bullet.velocity = reflectionVector * reflectionVelocity;

			yield return new WaitForFixedUpdate();
		}

		//Destroy the shield after it finishes shooting all of the bullets
		Destroy(gameObject);
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Bullet" && other.GetComponent<Bullet>().owningPlayer != owningPlayer) {
			Bullet thisBullet = other.GetComponent<Bullet>();
			//Only absorb bullets that haven't been absorbed by anything else
			if (thisBullet.IsInteractable()) {
				if (absorbedBullets.Count < maxBulletCount) {
					StartCoroutine(OrbitBullet(other.GetComponent<Bullet>()));
				}
			}
		}
	}

	IEnumerator OrbitBullet(Bullet thisBullet) {
		if (shooting) {
			yield break;
		}

		float radius = transform.localScale.y;
		float lerpSpeed = 0.5f;
		float orbitSpeed = Random.Range(-10 * Mathf.Deg2Rad, 10 * Mathf.Deg2Rad);

		//Add the bullet to the list
		absorbedBullets.Add(thisBullet.GetComponent<PhysicsObj>());

		//Change ownership of the bullet and halt its velocity
		thisBullet.owningPlayer = owningPlayer;
		thisBullet.curState = BulletState.absorbedByMasochist;
		thisBullet.GetComponent<PhysicsObj>().velocity = Vector3.zero;

		//Lerp to a position inside the shield
		//Vector3 targetPosition = new PolarCoordinate(radius, thisBullet.transform.position).PolarToCartesian() + transform.position;

		//while ((targetPosition - thisBullet.transform.position).magnitude >= 0.01f) {
		//	thisBullet.transform.position = Vector3.Lerp(thisBullet.transform.position, targetPosition, lerpSpeed);
		//	yield return new WaitForFixedUpdate();
		//}

		//Orbit the bullets around the player
		PolarCoordinate bulletPos = new PolarCoordinate(radius, thisBullet.transform.position);
		
		while (thisBullet.curState == BulletState.absorbedByMasochist) {
			bulletPos.angle += orbitSpeed;
			thisBullet.transform.position = transform.position + bulletPos.PolarToCartesian();
			yield return new WaitForFixedUpdate();
		}
	}
}
