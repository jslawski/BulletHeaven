using UnityEngine;
using System.Collections;

public class Beam : MonoBehaviour {
	public GameObject explosionPrefab;

	Player _owningPlayer = Player.none;
	public Player owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
		}
	}

	float damage = 1f;
	float slowingFactor = 0.25f;				//Percent of normal movement speed the player experiences while in the beam
	ParticleSystem[] beams;

	// Use this for initialization
	void Awake () {
		beams = GetComponentsInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
	
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
