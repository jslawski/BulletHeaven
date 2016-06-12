using UnityEngine;
using System.Collections;

public class DualLasers : MonoBehaviour {
	public Player owningPlayer;
	PlayerShip thisPlayer;

	GameObject explosionPrefab;
	public ParticleSystem[] lasers;
	public BoxCollider[] hitboxes;

	float damage = 0.45f;
	float useSlowingFactor = 0.4f;
	float slowingFactor = 0.35f;
	float chargeTime = 0.75f;
	float maxDuration = 6f;

	KeyCode A;
	bool hasEnded = false;

	// Use this for initialization
	void Awake () {
		explosionPrefab = Resources.Load<GameObject>("Prefabs/Explosion");
	}

	IEnumerator Start() {
		A = (owningPlayer == Player.player1) ? KeyCode.Alpha1 : KeyCode.Keypad1;

		//Set this as a child of the player
		if (owningPlayer != Player.none) {
			thisPlayer = GameManager.S.players[(int)owningPlayer];
			transform.SetParent(thisPlayer.transform, false);
			transform.localPosition = Vector3.zero;
		}
		//Wait while charging
		yield return new WaitForSeconds(chargeTime);
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
		if (Input.GetKeyUp(A)) {
			EndLaserAttack();
		}
		if (thisPlayer.device != null) {
			if (thisPlayer.device.Action1.WasReleased) {
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
