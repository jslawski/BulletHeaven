using UnityEngine;
using System.Collections;

public class ProtagShip : MonoBehaviour, DamageableObject {
	GameObject shipExplosionPrefab;
	float slowdownOnHit = 0.75f;		//Amount the ship slows down from getting hit by a bullet

	float minHealth = 5f;
	float maxHealth = 10f;
	float shipHealth;

	void Awake() {
		shipHealth = Random.Range(minHealth, maxHealth);
		shipExplosionPrefab = Resources.Load<GameObject>("Prefabs/ProtagShipExplosion");
	}

	public void TakeDamage(float damageIn) {
		shipHealth -= damageIn;
		GetComponent<PhysicsObj>().velocity *= slowdownOnHit;
	}

	void Update() {
		if (shipHealth <= 0) {
			Explode();
		}
	}

	void Explode() {
		CameraEffects.S.CameraShake(1f, 1f);

		GameObject explosion = Instantiate(shipExplosionPrefab, transform.position, new Quaternion()) as GameObject;
		Destroy(explosion, 5f);
		Destroy(gameObject);
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag != "KillZone") {
			return;
		}
		Destroy(gameObject);
	}
}
