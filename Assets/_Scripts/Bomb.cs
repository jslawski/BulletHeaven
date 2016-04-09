using UnityEngine;
using System.Collections;

public enum Attack {
	leadingShot,
	spiral,
	attack3,
	attack4
}

public class Bomb : MonoBehaviour {
	public Player owningPlayer = Player.none;       //Set this when creating a new Bomb
	public GameObject shockwavePrefab;
	public LeadingShot leadingShotPrefab;

	PhysicsObj physics;
	SpriteRenderer spriteRenderer;


	float decelerationRate = 0.005f;

	// Use this for initialization
	void Start () {
		physics = GetComponent<PhysicsObj>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		physics.velocity = Vector3.Lerp(physics.velocity, Vector3.zero, decelerationRate);
	}

	void Update() {
		//Destroy the bomb if it goes off-screen
		if (!spriteRenderer.isVisible) {
			//print("Bomb " + gameObject.name + " went offscreen and died");
			Destroy(gameObject);
		}
	}

	public void Detonate(Attack attackToPerform) {
		//Stop moving the bomb
		physics.velocity = Vector3.zero;

		switch (attackToPerform) {
			case Attack.leadingShot:
				LeadingShot newShot = Instantiate(leadingShotPrefab, transform.position, new Quaternion()) as LeadingShot;
				newShot.owningPlayer = owningPlayer;
				newShot.FireBurst();
				break;
			default:
				Debug.LogError("Attack type " + attackToPerform.ToString() + " not handled in Bomb.Detonate()");
				break;
		}

		GameObject shockwave = Instantiate(shockwavePrefab, transform.position, new Quaternion()) as GameObject;
		Destroy(shockwave, 5f);
		Destroy(gameObject);
	}

	void OnDestroy() {
		//print("Destroyed bomb");
		//Remove this bomb from the bombsInAir queue
		if (GameManager.S && owningPlayer != Player.none) {
			GameManager.S.players[(int)owningPlayer].GetComponent<ShootBomb>().bombsInAir.Remove(this);
		}
	}
}
