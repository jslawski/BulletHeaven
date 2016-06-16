using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PhysicsEngine : MonoBehaviour {
	public static PhysicsEngine S;
	public static List<PhysicsObj> physicsObjects;

	public Vector3 gravity = new Vector3(0,-9.8f,0);

	void Awake() {
		if (S != null) {
			Destroy(this);
			return;
		}
		S = this;
		physicsObjects = new List<PhysicsObj>();
	}

	void FixedUpdate() {
		//Move the PE_objects
		foreach (var obj in physicsObjects) {
			//keep still objects from moving
			if (obj.still) {
				if (obj.actOnLocalSpace) {
					obj.posNow = obj.posNext = obj.transform.localPosition;
				}
				else {
					obj.posNow = obj.posNext = obj.transform.position;
				}
				continue;
			}

			//apply the acceleration due to gravity to the object
			Vector3 curAcc = obj.acceleration;
			if (obj.affectedByGravity) {
				curAcc += gravity;
			}

			//apply the new acceleration to the object's velocity
			Vector3 curVel = obj.velocity;
			curVel += curAcc * Time.fixedDeltaTime;
			obj.velocity = curVel;

			//apply the updated velocity to change the object's position
			if (obj.actOnLocalSpace) {
				obj.posNow = obj.transform.localPosition;
				obj.posNext = obj.posNow + curVel * Time.fixedDeltaTime;
				obj.transform.localPosition = obj.posNext;
			}
			else {
				obj.posNow = obj.transform.position;
				obj.posNext = obj.posNow + curVel * Time.fixedDeltaTime;
				obj.transform.position = obj.posNext;
			}
		}
	}
}
