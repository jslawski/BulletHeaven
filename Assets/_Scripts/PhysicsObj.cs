using UnityEngine;
using System.Collections;

public class PhysicsObj : MonoBehaviour {
	public bool still = true;
	public bool affectedByGravity = true;
	public Vector3 acceleration, velocity, posNow, posNext;

	// Use this for initialization
	void Start () {
		PhysicsEngine.physicsObjects.Add(this);
	}

	void OnTriggerEnter(Collider other) {
		//don't calculate physics for still objects or objects colliding with non-scenery
		if (still || other.gameObject.layer != LayerMask.NameToLayer("LevelBounds"))
			return;
		print("Triggered by " + other.name);

		PhysicsObj otherObj = other.GetComponent<PhysicsObj>();
		if (otherObj == null) return;

		//Find the last position's bounding box values
		float oldBoxLeft = posNow.x - this.gameObject.transform.localScale.x/2f;
		float oldBoxRight = posNow.x + this.gameObject.transform.localScale.x/2f;
		float oldBoxTop = posNow.y + this.gameObject.transform.localScale.y/2f;
		float oldBoxBottom = posNow.y - this.gameObject.transform.localScale.y/2f;

		//Find the next position's bounding box values
		float newBoxLeft = posNext.x - this.gameObject.transform.localScale.x/2f;
		float newBoxRight = posNext.x + this.gameObject.transform.localScale.x/2f;
		float newBoxTop = posNext.y + this.gameObject.transform.localScale.y/2f;
		float newBoxBottom = posNext.y - this.gameObject.transform.localScale.y/2f;

		//Find the other object's bounding box values
		float otherBoxLeft = otherObj.posNow.x - otherObj.gameObject.transform.localScale.x/2f;
		float otherBoxRight = otherObj.posNow.x + otherObj.gameObject.transform.localScale.x/2f;
		float otherBoxTop = otherObj.posNow.y + otherObj.gameObject.transform.localScale.y/2f;
		float otherBoxBottom = otherObj.posNow.y - otherObj.gameObject.transform.localScale.y/2f;

		/*print("oldBoxLeft: " + oldBoxLeft +
			  " oldBoxRight: " + oldBoxRight +
			  " oldBoxTop: " + oldBoxTop +
			  " oldBoxBottom: " + oldBoxBottom +
			  "\n" +
			  "newBoxLeft: " + newBoxLeft +
			  " newBoxRight: " + newBoxRight + 
			  " newBoxTop: " + newBoxTop + 
			  " newBoxBottom: " + newBoxBottom +
			  "\n" +
			  "otherBoxLeft: " + otherBoxLeft +
			  " otherBoxRight: " + otherBoxRight + 
			  " otherBoxTop: " + otherBoxTop + 
			  " otherBoxBottom: " + otherBoxBottom);*/

		bool collidedFromLeft = false,
			 collidedFromRight = false,
			 collidedFromAbove = false,
			 collidedFromBelow = false;

		//the amount the two objects overlap in each direction in the next frame using posNext
		//positive overlapX == collided from left
		//negative overlapX == collided from right
		//positive overlapY == collided from below
		//negative overlapY == collided from above
		float overlapX = 0,
			  overlapY = 0;

		//if collided from left
		if (oldBoxRight < otherBoxLeft && newBoxRight >= otherBoxLeft) {
			//print(this.gameObject.name + " collided with " + otherObj.gameObject.name + " from the left.");
			collidedFromLeft = true;
			overlapX = newBoxRight - otherBoxLeft;
		}
		//if collided from right
		if (oldBoxLeft >= otherBoxRight && newBoxLeft < otherBoxRight) {
			//print(this.gameObject.name + " collided with " + otherObj.gameObject.name + " from the right.");
			collidedFromRight = true;
			overlapX = newBoxLeft - otherBoxRight;
		}
		//if collided from above
		if (oldBoxBottom >= otherBoxTop && newBoxBottom < otherBoxTop) {
			//print(this.gameObject.name + " collided with " + otherObj.gameObject.name + " from above.");
			collidedFromAbove = true;
			overlapY = newBoxBottom - otherBoxTop;
		}
		//if collided from below
		if (oldBoxTop < otherBoxBottom && newBoxTop >= otherBoxBottom) {
			//print(this.gameObject.name + " collided with " + otherObj.gameObject.name + " from below.");
			collidedFromBelow = true;
			overlapY = newBoxTop - otherBoxBottom;
		}

		//If we collided in both the x and y direction
		if (Mathf.Abs(overlapY) > 0 && Mathf.Abs(overlapX) > 0) {
			//we will choose to resolve the collision based on the shortest overlap distance
			bool y_resolution = (Mathf.Abs(overlapY) < Mathf.Abs(overlapX));

			if (y_resolution) {
				collidedFromRight = collidedFromLeft = false;
			}
			else {
				collidedFromAbove = collidedFromBelow = false;
			}
		}

		//resolve collisions by placing colliding object on the outside of the other object
		//*** I use values close to 2f instead of the mathematical value 2f here because rounding
		//*** would sometimes lead to an object being placed inside of the colliding object.
		//*** This places them slightly away from the object with which they are colliding.
		if (collidedFromAbove) {
			posNext.y = (transform.localScale.y + otherObj.transform.localScale.y) / 1.985f + otherObj.transform.position.y;
			velocity.y = 0;
			affectedByGravity = false;
		}
		else if (collidedFromBelow) {
			posNext.y = -(transform.localScale.y + otherObj.transform.localScale.y) / 2f + otherObj.transform.position.y;
			velocity.y = 0;
		}
		if (collidedFromRight) {
			posNext.x = (transform.localScale.x + otherObj.transform.localScale.x) / 1.999f + otherObj.transform.position.x;
			velocity.x = 0;
		}
		else if (collidedFromLeft) {
			posNext.x = -(transform.localScale.x + otherObj.transform.localScale.x) / 1.999f + otherObj.transform.position.x;
			velocity.x = 0;
		}


		transform.position = posNext;

	}

	void OnTriggerStay(Collider other) {
		OnTriggerEnter(other);
	}

	void OnDestroy() {
		PhysicsEngine.physicsObjects.Remove(this);
	}
}
