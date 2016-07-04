using UnityEngine;
using System.Collections;

public class ExplodeAttack : MonoBehaviour, BombAttack {
	Player _owningPlayer = Player.none;

	public Player owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
		}
	}

	public GameObject explosionPrefab;
	ParticleSystem explosionParticles;

	float baseDamage = 30f;
	float damageDealt = 1f;
	float explosionRadius = 4f;

	float GetDistance(Vector3 p1, Vector3 p2) {
		float distance = Mathf.Abs(Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2)));
		return distance;
	}

	float CalculateDamageDealt(Transform victim) {
		//Normalize the distance to be a value between 0 (center of explosion) and 1 (edge of explosion)
		//Explosion deals more damage closer to the center, so a normalized value of 0 should yield the highest scalar of 1
		float damageScalar = Mathf.Abs(1 - (GetDistance(transform.position, victim.position) / explosionRadius));

		//Minimum damage dealt by the explosion is about a third of the base damage
		if (damageScalar < 0.30f) {
			damageScalar = 0.30f;
		}

		return baseDamage * damageScalar;
	}

	public void FireBurst() {
		if (GameManager.S.inGame) {
			SoundManager.instance.Play("Explosion");
		}
		explosionParticles = transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();

		//Scale the explosion size based on the damage multiplier
		Masochist masochistPlayer = GameManager.S.players[(int)owningPlayer] as Masochist;
		if (masochistPlayer != null) {
			explosionRadius = explosionRadius * masochistPlayer.damageMultiplier;
			explosionParticles.startSize = explosionParticles.startSize * masochistPlayer.damageMultiplier;
		}

		Collider[] hitTargets = Physics.OverlapSphere(transform.position, explosionRadius);

		foreach (Collider target in hitTargets) {
			DamageTarget(target);
		}

		//Destroy after an arbitrary amount of time (explosion will be finished quick, this is just clean-up)
		Destroy(gameObject, 2f);
	}

	//Damage any player or protag ship that is within the explosion
	void DamageTarget(Collider other) {
		if (other.tag == "Player") {
			PlayerShip player = other.gameObject.GetComponentInParent<PlayerShip>();
			//Do damage to the player hit
			float multiplier = 1f;
			Masochist masochistPlayer = GameManager.S.players[(int)owningPlayer] as Masochist;
			if (masochistPlayer != null) {
				multiplier = masochistPlayer.damageMultiplier;
			}
			damageDealt = CalculateDamageDealt(other.transform) * multiplier;
			player.TakeDamage(damageDealt);
			print("Damage Dealt: " + damageDealt);

			GameObject explosion = Instantiate(explosionPrefab, other.gameObject.transform.position, new Quaternion()) as GameObject;
			Destroy(explosion, 5f);
		}
		else if (other.tag == "ProtagShip") {
			DamageableObject otherShip = other.gameObject.GetComponentInParent<DamageableObject>();
			Masochist masochistPlayer = GameManager.S.players[(int)owningPlayer] as Masochist;
			if (masochistPlayer != null) {
				damageDealt = CalculateDamageDealt(other.transform) * masochistPlayer.damageMultiplier;
			}
			else {
				damageDealt = CalculateDamageDealt(other.transform);
			}
			otherShip.TakeDamage(damageDealt);

			GameObject explosion = Instantiate(explosionPrefab, other.gameObject.transform.position, new Quaternion()) as GameObject;
			Destroy(explosion, 5f);
		}
	}
}
