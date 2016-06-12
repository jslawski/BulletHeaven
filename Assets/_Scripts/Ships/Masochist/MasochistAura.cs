using UnityEngine;
using System.Collections;

public class MasochistAura : MonoBehaviour {
	public Masochist player;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (player != null) {
			gameObject.transform.position = player.transform.position;
		}
		else {
			Destroy(gameObject);
		}
	}
}
