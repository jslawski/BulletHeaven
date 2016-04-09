using UnityEngine;
using System.Collections;

public class ProtagShip : MonoBehaviour {
	void OnTriggerEnter(Collider other) {
		if (other.tag != "KillZone") {
			return;
		}
		Destroy(gameObject);
	}
}
