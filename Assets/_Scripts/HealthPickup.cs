using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour {
	public bool disabledMoveToCenter = false;
	float healAmount = 25f;
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
		if (!disabledMoveToCenter && (transform.parent.position.x < minX || transform.parent.position.x > maxX)) {
			transform.parent.position = Vector3.Lerp(transform.parent.position, new Vector3(centerOfWorld.x, transform.parent.position.y, centerOfWorld.z), 0.02f);
		}

		lifespan -= Time.deltaTime;
		if (lifespan <= 0) {
			Destroy(transform.parent.gameObject);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			SoundManager.instance.Play("HealthPickup");
			PlayerShip thisPlayer = other.GetComponentInParent<PlayerShip>();
			thisPlayer.TakeDamage(-healAmount);
			Destroy(transform.parent.gameObject);
		}
	}
}
