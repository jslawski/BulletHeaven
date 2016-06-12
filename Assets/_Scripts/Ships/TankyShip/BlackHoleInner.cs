﻿using UnityEngine;
using System.Collections;

public class BlackHoleInner : MonoBehaviour {
	BlackHole blackHole;
	GameObject explosionPrefab;

	// Use this for initialization
	void Start () {
		blackHole = GetComponentInParent<BlackHole>();
		explosionPrefab = Resources.Load<GameObject>("Prefabs/Explosion");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Bullet") {
			Bullet bullet = other.gameObject.GetComponent<Bullet>();
			if (bullet.owningPlayer != blackHole.owningPlayer) {
				bullet.physics.velocity *= 0.2f;
				blackHole.AddBullet(bullet);
			}
		}
	}

	void OnTriggerStay(Collider other) {
		if (other.gameObject.tag == "Player") {
			PlayerShip otherShip = other.GetComponentInParent<PlayerShip>();
			if (otherShip.player != blackHole.owningPlayer) {
				//Do damage to the player hit
				otherShip.TakeDamage(blackHole.directDamageInCenter);

				GameObject explosion = Instantiate(explosionPrefab, other.transform.position, new Quaternion()) as GameObject;
				Destroy(explosion, 5f);
			}
		}
		else if (other.gameObject.tag == "ProtagShip") {
			ProtagShip protagShip = other.GetComponentInParent<ProtagShip>();
			protagShip.TakeDamage(blackHole.directDamageInCenter);

			GameObject explosion = Instantiate(explosionPrefab, other.transform.position, new Quaternion()) as GameObject;
			Destroy(explosion, 5f);
		}
		else if (other.gameObject.tag == "Bullet") {
			Bullet bullet = other.gameObject.GetComponent<Bullet>();
			if (bullet.owningPlayer == blackHole.owningPlayer) {
				bullet.physics.acceleration = (transform.position - other.transform.position).normalized * blackHole.gravityForce;
			}
		}
	}

	void OnTriggerExit(Collider other) {
		//if (other.gameObject.tag == "Bullet") {
		//	Bullet bullet = other.gameObject.GetComponent<Bullet>();
		//	if (bullet.owningPlayer == blackHole.owningPlayer) {
		//		bullet.gameObject.layer = LayerMask.NameToLayer("Default");
		//	}
		//}
	}
}
