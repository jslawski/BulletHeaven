using UnityEngine;
using System.Collections;

public class ExplodeAttack : MonoBehaviour {
	public GameObject explosionPrefab;
	SphereCollider explosionZone;
	ParticleSystem explosionParticles;

	float baseDamage = 30f;
	float damageDealt = 1f;

	float GetDistance(Vector3 p1, Vector3 p2) {
		float distance = Mathf.Abs(Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2)));
		return distance;
	}

	float CalculateDamageDealt(Transform victim) {
		//Normalize the distance to be a value between 0 (center of explosion) and 1 (edge of explosion)
		//Explosion deals more damage closer to the center, so a normalized value of 0 should yield the highest scalar of 1
		float damageScalar = Mathf.Abs(1 - (GetDistance(transform.position, victim.position) / explosionZone.radius));
		return baseDamage * damageScalar;
	}

	void Start() {
		explosionZone = GetComponent<SphereCollider>();
		explosionParticles = transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();

		//Scale the explosion size based on the damage multiplier
		//Currently this is INSANE
		explosionZone.radius = explosionZone.radius * Masochist.damageMultiplier;
		explosionParticles.startSize = explosionParticles.startSize * Masochist.damageMultiplier;

		//Destroy after 1.3 seconds
		Invoke("DestroyInstance", 1.3f);
	}

	void DestroyInstance() {
		Destroy(gameObject);
	}

	//Damage any player or protag ship that is within the explosion
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			PlayerShip player = other.gameObject.GetComponentInParent<PlayerShip>();
			//Do damage to the player hit
			damageDealt = CalculateDamageDealt(other.transform) * Masochist.damageMultiplier;
			player.TakeDamage(damageDealt);
			print("Damage Dealt: " + damageDealt);

			GameObject explosion = Instantiate(explosionPrefab, other.gameObject.transform.position, new Quaternion()) as GameObject;
			Destroy(explosion, 5f);
		}
		else if (other.tag == "ProtagShip") {
			DamageableObject otherShip = other.gameObject.GetComponentInParent<DamageableObject>();
			damageDealt = CalculateDamageDealt(other.transform) * Masochist.damageMultiplier;
			otherShip.TakeDamage(damageDealt);

			GameObject explosion = Instantiate(explosionPrefab, other.gameObject.transform.position, new Quaternion()) as GameObject;
			Destroy(explosion, 5f);
		}
	}
}
