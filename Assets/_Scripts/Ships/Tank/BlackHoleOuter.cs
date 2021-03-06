﻿using UnityEngine;
using System.Collections;

public class BlackHoleOuter : MonoBehaviour {
	BlackHole blackHole;

	float outerRadius;

	// Use this for initialization
	void Start () {
		blackHole = GetComponentInParent<BlackHole>();
		outerRadius = GetComponent<SphereCollider>().radius;
	}

	void OnTriggerStay(Collider other) {
		if (other.gameObject.tag == "Bullet") {
			Bullet bullet = other.gameObject.GetComponent<Bullet>();

			//Don't do anything if the bullet can't be interacted with
			if (!bullet.IsInteractable()) {
				return;
			}

			if (bullet.curState == BulletState.parented) {
				PhysicsObj parentPhysics = bullet.transform.parent.GetComponent<PhysicsObj>();
				if (parentPhysics != null && parentPhysics.velocity != Vector3.zero) {
					bullet.physics.velocity = parentPhysics.velocity;
					bullet.physics.acceleration = parentPhysics.acceleration;
				}
				else {
					bullet.physics.velocity = 10f * (transform.position-bullet.transform.position).normalized;
					bullet.physics.acceleration = Vector2.zero;
				}
				bullet.curState = BulletState.affectedByBlackHole;
			}

			if (bullet.owningPlayer != blackHole.owningPlayer) {
				bullet.curState = BulletState.affectedByBlackHole;
				float t = 1 - (other.transform.position - transform.position).magnitude / outerRadius;
				bullet.physics.velocity *= blackHole.fieldSlowScalar;
				bullet.physics.acceleration = (transform.position - other.transform.position).normalized * Mathf.Lerp(0, blackHole.gravityForce, t);
			}

				
        }
		else if (other.gameObject.tag == "Player") {
			Ship otherPlayerShip = other.gameObject.GetComponentInParent<Ship>();
			if (otherPlayerShip.playerEnum != blackHole.owningPlayer) {
				float t = 1-(other.transform.position - transform.position).magnitude/outerRadius;
                float slow = Mathf.Lerp(0, blackHole.maxSlow, t*t);
				otherPlayerShip.movement.SlowPlayer(1-slow);
			}
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "Bullet") {
			Bullet bullet = other.gameObject.GetComponent<Bullet>();
			bullet.physics.acceleration = Vector3.zero;
		}
	}
}
