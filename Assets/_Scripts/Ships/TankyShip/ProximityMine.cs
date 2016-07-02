using UnityEngine;
using System.Collections;

public class ProximityMine : MonoBehaviour {
	public Player owningPlayer = Player.none;
	PhysicsObj physics;
	SphereCollider hitbox;

	float minRandSpeed = 9f;
	float maxRandSpeed = 15f;
	float decelerationRate = 1f;

	float flatDamage = 12.5f;
	float explosionRadius = 2.5f;

	float otherMineDetonationDelay = 0.15f;

	float armTime = .75f;
	float minDetonationTime = 4f;
	float maxDetonationTime = 4.25f;

	float minAnimationSpeed = 1f;
	float maxAnimationSpeed = 8f;

	bool inExplodeCoroutine = false;
	bool hasExploded = false;

	// Use this for initialization
	IEnumerator Start () {
		//Grab references
		physics = GetComponent<PhysicsObj>();
		Animator anim = GetComponent<Animator>();
		hitbox = GetComponent<SphereCollider>();

		//Apply a random velocity to each mine
		physics.velocity = Random.Range(minRandSpeed, maxRandSpeed) * Random.insideUnitCircle;

		//Don't allow the bomb to explode before being armed
		yield return new WaitForSeconds(armTime);
		hitbox.enabled = true;

		//Start the beeping animation, speed it up over time
		anim.SetBool("isArmed", true);
		float timeArmed = 0;
		while (timeArmed < minDetonationTime) {
			timeArmed += Time.deltaTime;
			float percent = timeArmed/minDetonationTime;
			anim.speed = Mathf.Lerp(minAnimationSpeed, maxAnimationSpeed, percent);

			yield return null;
		}
		//Slightly stagger the explosions
		yield return new WaitForSeconds(Random.Range(0, maxDetonationTime - minDetonationTime));
		Explode();
	}
	
	public void Explode(bool doubleRadius=false) {
		if (hasExploded) {
			return;
		}
		if (!inExplodeCoroutine) {
			StartCoroutine(ExplodeCoroutine());
		}
	}

	IEnumerator ExplodeCoroutine(bool doubleRadius=false) {
		inExplodeCoroutine = true;

		SoundManager.instance.Play("BombExplode");
		SoundManager.instance.Play("Explosion");
		hasExploded = true;
		hitbox.enabled = false;
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponentInChildren<ParticleSystem>().Play();
		Destroy(gameObject, 2.5f);

		Collider[] allObjectsHit = Physics.OverlapSphere(transform.position, (doubleRadius ? 2 : 1)*explosionRadius);
		ProximityMine nearestOtherMine = null;
		foreach (var obj in allObjectsHit) {
			//If any players are within the blast zone
			if (obj.gameObject.tag == "Player") {
				//Ignore the mine's owner
				PlayerShip playerHit = obj.gameObject.GetComponentInParent<PlayerShip>();
				if (playerHit.player == owningPlayer) {
					continue;
				}
				//Deal damage to any other player
				else {
					playerHit.TakeDamage(flatDamage);
				}
			}
			//Deal damage to any protag ship caught in the explosion
			else if (obj.gameObject.tag == "ProtagShip") {
				ProtagShip shipHit = obj.gameObject.GetComponentInParent<ProtagShip>();
				shipHit.TakeDamage(flatDamage);
			}
			//Blow up any other nearby proximity mines, with twice the radius (more mines == bigger explosion)
			else if (obj.gameObject.tag == "ProximityMine") {
				if (nearestOtherMine == null) {
					nearestOtherMine = obj.gameObject.GetComponent<ProximityMine>();
				}

			}
		}

		if (nearestOtherMine != null) {
			yield return new WaitForSeconds(otherMineDetonationDelay);
			nearestOtherMine.Explode(true);
		}

		inExplodeCoroutine = false;
	}

	void OnTriggerEnter(Collider other) {
		//Explode on impact as well
		if (other.gameObject.tag == "Player") {
			PlayerShip otherShip = other.gameObject.GetComponentInParent<PlayerShip>();
			if (otherShip.player != owningPlayer) {
				Explode();
			}
		}
		else if (other.gameObject.tag == "ProtagShip") {
			Explode();
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		//Decelerate the mine until it comes to a stop
		physics.velocity = Vector3.Lerp(physics.velocity, Vector3.zero, Time.fixedDeltaTime*decelerationRate);
	}
}
