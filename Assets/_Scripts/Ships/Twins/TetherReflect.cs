using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetherReflect : MonoBehaviour {
	Transform shipA;
	Transform shipB;

	// Use this for initialization
	void Start () {
		shipA = transform.parent.Find("TwinShipA");
		shipB = transform.parent.Find("TwinShipB");
	}

	// Update is called once per frame
	float shipSize = 2f;
	void Update () {
		transform.position = (shipA.position + shipB.position) / 2f;
		Vector3 diffVector = (shipA.position - shipB.position);
		transform.up = diffVector.normalized;

		Vector3 scale = transform.localScale;
		scale.y = diffVector.magnitude - shipSize;
		transform.localScale = scale;
	}

	float tooSlowThreshold = 0.5f;
	float reflectVelocity = 20f;
	float reflectVelocityVariance = 5f;
	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag != "Bullet") {
			return;
		}

		//Handle special types of bullets
		Bullet bullet = other.gameObject.GetComponent<Bullet>();
		if (bullet is SpecialBullet) {
			(bullet as SpecialBullet).ReturnBulletToNormal();
		}

		PhysicsObj bulletPhysics = bullet.physics;
		if (bulletPhysics.velocity.magnitude > 0.5f) {
			bulletPhysics.velocity = Vector3.Reflect(bulletPhysics.velocity, transform.right);
		}
		else {
			bulletPhysics.velocity = transform.right * Random.Range(reflectVelocity-reflectVelocityVariance, reflectVelocity+reflectVelocityVariance);
		}
	}
}
