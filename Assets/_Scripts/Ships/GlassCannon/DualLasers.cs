using UnityEngine;
using System.Collections;

public class DualLasers : MonoBehaviour {
	public Player owningPlayer;
	PlayerShip thisPlayer;

	GameObject explosionPrefab;
	public ParticleSystem[] lasers;
	public BoxCollider[] hitboxes;

	float damage;
	float maxDamage = 0.45f;
	float minDamage = 0.1f;
	float useSlowingFactor = 0.4f;
	float slowingFactor = 0.35f;
	float chargeTime = 0.75f;
	float maxDuration = 4f;

	float maxStartSize = 2f;
	float minStartSize = 0.45f;
	float maxHitboxSize = 1.35f;
	float minHitboxSize = 0.6f;

	KeyCode X;
	bool hasEnded = false;

	// Use this for initialization
	void Awake () {
		explosionPrefab = Resources.Load<GameObject>("Prefabs/Explosion");
	}

	IEnumerator Start() {
		X = (owningPlayer == Player.player1) ? KeyCode.Alpha3 : KeyCode.Keypad3;
		damage = maxDamage;

		//Set this as a child of the player
		if (owningPlayer != Player.none) {
			thisPlayer = GameManager.S.players[(int)owningPlayer];
			transform.SetParent(thisPlayer.transform, false);
			transform.localPosition = Vector3.zero;
		}
		//Wait while charging
		yield return new WaitForSeconds(chargeTime);
		StartCoroutine(LoseEnergy());
		thisPlayer.playerShooting.ExpendAttackSlot();
		//Turn on the hitboxes
		foreach (var hitbox in hitboxes) {
			hitbox.enabled = !hasEnded;
		}
		//Slow the player while firing the beam
		if (thisPlayer != null && !hasEnded) {
			thisPlayer.playerMovement.SlowPlayer(useSlowingFactor, maxDuration);
		}
		yield return new WaitForSeconds(maxDuration);
		EndLaserAttack();
	}

	IEnumerator LoseEnergy() {
		float timeElapsed = 0;
		while (!hasEnded) {
			timeElapsed += Time.deltaTime;
			float percent = timeElapsed/maxDuration;

			foreach (var laser in lasers) {
				laser.startSize = Mathf.Lerp(maxStartSize, minStartSize, percent);
			}
			foreach (var hitbox in hitboxes) {
				Vector3 size = hitbox.size;
				size.x = Mathf.Lerp(maxHitboxSize, minHitboxSize, percent);
				hitbox.size = size;
			}
			damage = Mathf.Lerp(maxDamage, minDamage, percent);

			yield return null;
		}
	}

	void EndLaserAttack() {
		if (hasEnded) {
			return;
		}
		hasEnded = true;

		//Turn off the lasers
		foreach (var laser in lasers) {
			laser.Stop();
		}
		//Remove the hitboxes
		foreach (var hitbox in hitboxes) {
			hitbox.enabled = false;
		}
		//Restore the player's speed
		if (owningPlayer != Player.none) {
			thisPlayer.playerMovement.RestoreSpeed();
		}

		Destroy(gameObject, 2f);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(X)) {
			EndLaserAttack();
		}
		if (thisPlayer.device != null) {
			if (thisPlayer.device.Action3.WasReleased) {
				EndLaserAttack();
			}
		}
	}

	void OnTriggerStay(Collider other) {
		if (other.tag == "Player") {
			PlayerShip player = other.gameObject.GetComponentInParent<PlayerShip>();
			ShipMovement playerMovement = other.gameObject.GetComponentInParent<ShipMovement>();
			if (player.player != owningPlayer) {
				//Do damage to the player hit
				player.TakeDamage(damage);

				//Slow the player while in the beam
				playerMovement.SlowPlayer(slowingFactor);

				GameObject explosion = Instantiate(explosionPrefab, other.transform.position, new Quaternion()) as GameObject;
				Destroy(explosion, 5f);
			}
		}
		else if (other.tag == "ProtagShip") {
			DamageableObject otherShip = other.gameObject.GetComponentInParent<DamageableObject>();
			otherShip.TakeDamage(damage);

			GameObject explosion = Instantiate(explosionPrefab, other.transform.position, new Quaternion()) as GameObject;
			Destroy(explosion, 5f);
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			PlayerShip player = other.gameObject.GetComponentInParent<PlayerShip>();
			ShipMovement playerMovement = other.gameObject.GetComponentInParent<ShipMovement>();
			if (player.player != owningPlayer) {
				//Restore the player's speed after leaving the beam
				playerMovement.RestoreSpeed();
			}
		}
	}
}
