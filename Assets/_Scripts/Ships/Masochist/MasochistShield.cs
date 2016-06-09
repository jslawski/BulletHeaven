using UnityEngine;
using System.Collections;
using PolarCoordinates;
using System.Collections.Generic;

public class MasochistShield : MonoBehaviour {
	SphereCollider thisCollider;
	Player otherPlayer;

	List<PhysicsObj> absorbedBullets;

	float rotationSpeed_c = 100f;
	public Masochist thisPlayer;
	float shieldDuration = 3f;

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

	void Start() {
		thisCollider = GetComponent<SphereCollider>();
		absorbedBullets = new List<PhysicsObj>();
	}

	// Use this for initialization
	public void ActivateShield () {
		thisPlayer.shieldUp = true;
		GetComponentInChildren<SpriteRenderer>().color = thisPlayer.playerColor;
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
		float reflectionVelocity = 20f;

		shooting = true;

		//Fire each bullet
		foreach (PhysicsObj bullet in absorbedBullets) {
			Vector3 bulletPosition = bullet.gameObject.transform.position;
			bullet.gameObject.GetComponent<Bullet>().absorbedByMasochist = false;

			if (otherPlayer != Player.none) {
				//Determine shooting vector
				Vector3 sprayVector = new Vector3(0, Random.Range(-sprayRange, sprayRange), 0);
				Vector3 reflectionVector = GameManager.S.players[(int)otherPlayer].player == Player.player1 ? Vector3.left : Vector3.right;

				//Shoot the bullet back at the opponent
				bullet.velocity = reflectionVector * reflectionVelocity;
			}
			yield return new WaitForFixedUpdate();
		}

		//Destroy the shield after it finishes shooting all of the bullets
		thisPlayer.shieldUp = false;
		Destroy(gameObject);
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Bullet" && other.GetComponent<Bullet>().owningPlayer != owningPlayer) {
			StartCoroutine(OrbitBullet(other.GetComponent<Bullet>()));
		}
	}

	IEnumerator OrbitBullet(Bullet thisBullet) {
		if (shooting) {
			yield break;
		}

		float radius = transform.localScale.y + 1.5f;
		float lerpSpeed = 0.5f;
		float orbitSpeed = Random.Range(-10 * Mathf.Deg2Rad, 10 * Mathf.Deg2Rad);

		//Add the bullet to the list
		absorbedBullets.Add(thisBullet.GetComponent<PhysicsObj>());

		//Make note of the opposing player
		otherPlayer = thisBullet.owningPlayer;

		//Change ownership of the bullet and halt its velocity
		thisBullet.owningPlayer = owningPlayer;
		thisBullet.absorbedByMasochist = true;
		thisBullet.GetComponent<PhysicsObj>().velocity = Vector3.zero;

		//Lerp to a position inside the shield
		Vector3 targetPosition = new Vector3(transform.position.x + radius, transform.position.y + radius, 0);

		while ((targetPosition - thisBullet.transform.position).magnitude >= 0.01f) {
			thisBullet.transform.position = Vector3.Lerp(thisBullet.transform.position, targetPosition, lerpSpeed);
			yield return new WaitForFixedUpdate();
		}

		//Orbit the bullets around the player
		PolarCoordinate bulletPos = new PolarCoordinate(radius, thisBullet.transform.position);
		
		while (thisBullet.absorbedByMasochist) {
			bulletPos.angle += orbitSpeed;
			thisBullet.transform.position = transform.position + bulletPos.PolarToCartesian();
			yield return new WaitForFixedUpdate();
		}
	}
}
