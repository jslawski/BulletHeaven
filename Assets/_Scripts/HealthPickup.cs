using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour {
	float healAmount = 35f;
	float lifespan = 15;
	Vector3 centerOfWorld;
	float minX;
	float maxX;

	// Use this for initialization
	void Start () {
		centerOfWorld = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
		centerOfWorld.z = 0;
		minX = GameManager.S.players[1].playerMovement.worldSpaceMinX;
		maxX = GameManager.S.players[0].playerMovement.worldSpaceMaxX;
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.x < minX || transform.position.x > maxX) {
			transform.position = Vector3.Lerp(transform.position, new Vector3(centerOfWorld.x, transform.position.y, centerOfWorld.z), 0.1f);
		}

		lifespan -= Time.deltaTime;
		if (lifespan <= 0) {
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			SoundManager.instance.Play("HealthPickup");
			PlayerShip thisPlayer = other.GetComponentInParent<PlayerShip>();
			thisPlayer.TakeDamage(-healAmount);
			Destroy(gameObject);
		}
	}
}
