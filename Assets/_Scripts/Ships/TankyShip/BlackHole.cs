using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlackHole : MonoBehaviour, BombAttack {
	Player _owningPlayer = Player.none;

	public Player owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
		}
	}

	List<Bullet> trappedBullets = new List<Bullet>();
	int maxNumTrappedBullets = 100;
	
	ParticleSystem explosion;
	ParticleSystem outerParticleSystem;
	ParticleSystem innerParticleSystem;
	BlackHoleInner inner;
	BlackHoleOuter outer;

	float maxLifespan = 6f;
	float armTime = 0.75f;
	float explosionRadius = 4f;
	float explosionDamage = 15f;

	[HideInInspector]
	public float fieldSlowScalar = 0.995f;          //Multiplies the bullet's velocity by this amount each frame the bullet remains

	[HideInInspector]
	public float gravityForce = 48;                 

	[HideInInspector]
	public float directDamageInCenter = 0.4f;       //Done per tick while the player is in the center

	[HideInInspector]
	public float maxSlow = 0.85f;

	float rotationSpeed = -20f;
	bool hasExploded = false;

	public void FireBurst() {
		//This does nothing to appease the interface
	}

	// Use this for initialization
	IEnumerator Start () {
		Invoke("Explode", maxLifespan);
		SoundManager.instance.Play("BlackHole");
		inner = GetComponentInChildren<BlackHoleInner>();
		inner.GetComponent<SphereCollider>().enabled = false;
		outer = GetComponentInChildren<BlackHoleOuter>();
		explosion = transform.FindChild("Explosion").GetComponent<ParticleSystem>();
		outerParticleSystem = transform.FindChild("OuterParticleSystem").GetComponent<ParticleSystem>();
		innerParticleSystem = transform.FindChild("InnerParticleSystem").GetComponent<ParticleSystem>();

		//Set particle colors
		Color col = GameManager.S.players[(int)owningPlayer].playerColor;
		innerParticleSystem.startColor = Color.Lerp(col, new Color(col.r, col.g, col.b, 0), 0.97f);
		outerParticleSystem.startColor = new Color(col.r, col.g, col.b, 65f / 255f);
		innerParticleSystem.Play();
		outerParticleSystem.Play();

		yield return new WaitForSeconds(armTime);
		inner.GetComponent<SphereCollider>().enabled = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.fixedDeltaTime));
	}

	public void AddBullet(Bullet newBullet) {
		if (trappedBullets.Count < maxNumTrappedBullets) {
			newBullet.owningPlayer = owningPlayer;
			newBullet.gameObject.layer = LayerMask.NameToLayer("TrappedBullet");
			trappedBullets.Add(newBullet);
		}
		else if (!hasExploded) {
			Explode();
		}
	}

	void Explode() {
		if (hasExploded) {
			return;
		}
		hasExploded = true;
		SoundManager.instance.Play("Explosion");
		foreach (var bullet in trappedBullets) {
			if (bullet != null) {
				bullet.gameObject.layer = LayerMask.NameToLayer("Default");
			}
		}

		//Remove the hitboxes and sprite
		Destroy(inner);
		Destroy(outer);
		GetComponent<SpriteRenderer>().enabled = false;

		//Stop the inner particle system and immediately destroy the outer particle system
		innerParticleSystem.Stop();
		Destroy(outerParticleSystem.gameObject);

		//Play the explosion particle system
		explosion.Play();
		Collider[] hitTargets = Physics.OverlapSphere(transform.position, explosionRadius);
		foreach (Collider target in hitTargets) {
			//JPS: Bug, if a protag ship gets hit, it won't have a PlayerShip component
			if (target.gameObject.tag == "Player" || target.gameObject.tag == "ProtagShip") {
				DamageableObject shipHit = target.GetComponentInParent<DamageableObject>();
				shipHit.TakeDamage(explosionDamage);
			}
		}

		//Clean up
		Destroy(gameObject, 5f);
	}
}
