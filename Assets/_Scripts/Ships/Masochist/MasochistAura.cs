using UnityEngine;
using System.Collections;

public class MasochistAura : MonoBehaviour {
	public Masochist playerShip;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (playerShip != null) {
			gameObject.transform.position = playerShip.transform.position;
		}
		else {
			Destroy(gameObject);
		}
	}
}
