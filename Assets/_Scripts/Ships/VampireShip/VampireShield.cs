using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VampireShield : MonoBehaviour {
	SphereCollider thisCollider;
	SpriteRenderer shieldSprite;
	Player otherPlayer;

	float rotationSpeed_c = 100f;
	public VampireShip thisPlayer;
	float shieldDuration = 1.5f;

	Bullet lastAbsorbedBullet = null;

	List<Bullet> absorbedBullets;

	public Player _owningPlayer = Player.none;
	public Player owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
		}
	}

	void Start() {
		thisCollider = GetComponent<SphereCollider>();
		shieldSprite = GetComponentInChildren<SpriteRenderer>();
		absorbedBullets = new List<Bullet>();
	}

	// Use this for initialization
	public void ActivateShield() {
		thisPlayer.shieldUp = true;
		GetComponentInChildren<SpriteRenderer>().color = thisPlayer.playerColor;
		StartCoroutine(RotateShield());
		Invoke("DestroyShield", shieldDuration);
	}

	void DestroyShield() {
		StartCoroutine(DestroyShieldCoroutine());
	}

	//Shield clean-up
	IEnumerator DestroyShieldCoroutine() {
		//Prevent any more bullets from getting absorbed
		shieldSprite.enabled = false;
		thisCollider.enabled = false;

		//Clean up all of the remaining bullets being absorbed, and teleport them
		//directly to the player's hitbox
		for (int i = 0; i < absorbedBullets.Count; i++) {
			if (absorbedBullets[i].absorbedByVampire == true) {
				Vector3 endPosition = new Vector3(transform.position.x + 1, transform.position.y, 0);
				absorbedBullets[i].transform.position = endPosition;
			}
			yield return new WaitForFixedUpdate();
		}
		
		thisPlayer.shieldUp = false;
		Destroy(gameObject);
	}

	IEnumerator RotateShield() {
		for (float i = 0; i < shieldDuration; i += Time.fixedDeltaTime) {
			gameObject.transform.Rotate(new Vector3(0, 0, rotationSpeed_c * Time.fixedDeltaTime));
			yield return new WaitForFixedUpdate();
		}
	}

	void OnTriggerEnter(Collider other) {
		if (shieldSprite.enabled == true && other.tag == "Bullet" && other.GetComponent<Bullet>().owningPlayer != owningPlayer) {
			StartCoroutine(AbsorbBullet(other.GetComponent<Bullet>()));
		}
	}

	IEnumerator AbsorbBullet(Bullet thisBullet) {
		float radius = transform.localScale.y;
		float lerpSpeed = 0.4f;

		absorbedBullets.Add(thisBullet);
		thisBullet.absorbedByVampire = true;
		thisBullet.partOfHomingGroup = false;

		//Change color of the bullet and halt its velocity
		thisBullet.owningPlayer = thisPlayer.player;
		thisBullet.GetComponent<PhysicsObj>().velocity = Vector3.zero;

		//Lerp to a position inside the shield
		Vector3 targetPosition = new Vector3(transform.position.x + 1, transform.position.y, 0);

		while (thisBullet.absorbedByVampire == true && (targetPosition - thisBullet.transform.position).magnitude >= 0.01f) {
			targetPosition = new Vector3(transform.position.x + 1, transform.position.y, 0);
			thisBullet.transform.position = Vector3.Lerp(thisBullet.transform.position, targetPosition, lerpSpeed);
			yield return new WaitForFixedUpdate();
		}
	}
}
