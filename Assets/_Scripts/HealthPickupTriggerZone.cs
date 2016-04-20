using UnityEngine;
using System.Collections;

public class HealthPickupTriggerZone : MonoBehaviour {
	HealthPickup thisHealthPickup;
	float defaultFollowSpeed = 0.01f;
	float acceleration = 1.05f;             //Multiplier on follow speed as the player stays in the trigger zone
	float curFollowSpeed;                   //Increases the longer the player stays in the trigger zone

	// Use this for initialization
	void Start() {
		thisHealthPickup = transform.parent.GetComponentInChildren<HealthPickup>();
		curFollowSpeed = defaultFollowSpeed;
	}

	// Update is called once per frame
	void Update() {

	}

	void OnTriggerStay(Collider other) {
		thisHealthPickup.disabledMoveToCenter = true;
		if (other.gameObject.tag != "Player") {
			return;
		}

		transform.parent.position = Vector3.Lerp(transform.parent.position, other.gameObject.transform.position, curFollowSpeed);
		curFollowSpeed *= acceleration;
	}

	void OnTriggerExit(Collider other) {
		thisHealthPickup.disabledMoveToCenter = false;
		if (other.gameObject.tag != "Player") {
			return;
		}

		curFollowSpeed = defaultFollowSpeed;
	}
}
