using UnityEngine;
using System.Collections;

public class Beam : MonoBehaviour {
	public GameObject explosionPrefab;
	float vibrationIntensity = 0.15f;

	Player _owningPlayer = Player.none;
	public Player owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
			if (value != Player.none) {
				foreach (var beam in beams) {
					beam.startColor = GameManager.S.players[(int)value].playerColor;
				}
			}
		}
	}

	float chargeDuration = 0.75f;
	float beamDuration = 3.2f;
	float damage = 0.65f;
	float slowingFactor = 0.25f;				//Percent of normal movement speed the player experiences while in the beam
	ParticleSystem[] beams;
	BoxCollider[] hitboxes;

	// Use this for initialization
	void Awake() {
		beams = GetComponentsInChildren<ParticleSystem>();
		hitboxes = GetComponentsInChildren<BoxCollider>();

		Invoke("StartBeam", chargeDuration);
    }

	void StartBeam() {
		VibrateManager.S.RumbleVibrate(Player.player1, beamDuration, vibrationIntensity, false);
		VibrateManager.S.RumbleVibrate(Player.player2, beamDuration, vibrationIntensity, false);
		CameraEffects.S.CameraShake(1.5f, 0.75f, true);

		//Stop the charge particle system and start the beam particle systems
		beams[0].Stop();
		for (int i = 1; i < beams.Length; i++) {
			beams[i].Play();
		}

		//Turn the hitboxes on for the beams
		foreach (var hitbox in hitboxes) {
			hitbox.enabled = true;
		}

		StartCoroutine(DestroyBeam());
	}

	IEnumerator DestroyBeam() {
		yield return new WaitForSeconds(beamDuration);

		Destroy(gameObject);

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
				//Slow the player while in the beam
				playerMovement.RestoreSpeed();
			}
		}
	}
}
