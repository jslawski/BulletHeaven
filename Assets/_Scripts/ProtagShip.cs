using UnityEngine;
using System.Collections;

public class ProtagShip : MonoBehaviour, DamageableObject {
	float minLifespan = 4f;
	float timeAlive = 0;
	GameObject shipExplosionPrefab;
	GameObject healthSpawnPrefab;
	SpriteRenderer sprite;
	float slowdownOnHit = 0.75f;		//Amount the ship slows down from getting hit by a bullet

	float minHealth = 5f;
	float maxHealth = 10f;
	float shipHealth;

	void Awake() {
		shipHealth = Random.Range(minHealth, maxHealth);
		shipExplosionPrefab = Resources.Load<GameObject>("Prefabs/ProtagShipExplosion");
		healthSpawnPrefab = Resources.Load<GameObject>("Prefabs/HealthPickup");
		sprite = GetComponent<SpriteRenderer>();
	}

	public void TakeDamage(float damageIn) {
		shipHealth -= damageIn;
		GetComponent<PhysicsObj>().velocity *= slowdownOnHit;
	}

	void Update() {
		if (shipHealth <= 0) {
			Explode();
		}

		if (!sprite.isVisible && timeAlive > minLifespan) {
			Destroy(gameObject);
		}
		timeAlive += Time.deltaTime;
	}

	void Explode() {
		CameraEffects.S.CameraShake(1f, 1.5f, true);

		GameObject explosion = Instantiate(shipExplosionPrefab, transform.position, new Quaternion()) as GameObject;

		//Don't spawn health on the title screen
		if (Application.loadedLevelName != GameManager.S.titleSceneName) {
			Instantiate(healthSpawnPrefab, transform.position, new Quaternion());
		}

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
