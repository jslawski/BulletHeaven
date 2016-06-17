using UnityEngine;
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
			bullet.absorbedByBlackHole = true;
			if (bullet.partOfHomingGroup) {
				PhysicsObj parentPhysics = bullet.transform.parent.GetComponent<PhysicsObj>();
				//if (parentPhysics != null) {
				print("Parent Velocity: " + parentPhysics.velocity);
					bullet.physics.velocity = parentPhysics.velocity;
					bullet.physics.acceleration = parentPhysics.acceleration;
				//}
				bullet.partOfHomingGroup = false;
			}
			if (bullet.owningPlayer != blackHole.owningPlayer) {
				float t = 1-(other.transform.position - transform.position).magnitude/outerRadius;
				bullet.physics.velocity *= blackHole.fieldSlowScalar;
				bullet.physics.acceleration = (transform.position - other.transform.position).normalized * Mathf.Lerp(0, blackHole.gravityForce, t);
			}
        }
		else if (other.gameObject.tag == "Player") {
			PlayerShip otherPlayer = other.gameObject.GetComponentInParent<PlayerShip>();
			if (otherPlayer.player != blackHole.owningPlayer) {
				float t = 1-(other.transform.position - transform.position).magnitude/outerRadius;
                float slow = Mathf.Lerp(0, blackHole.maxSlow, t*t);
				//print(blackHole.maxSlow + " " + slow);
				otherPlayer.playerMovement.SlowPlayer(1-slow);
			}
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "Bullet") {
			Bullet bullet = other.gameObject.GetComponent<Bullet>();
			bullet.physics.acceleration = Vector3.zero;
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
