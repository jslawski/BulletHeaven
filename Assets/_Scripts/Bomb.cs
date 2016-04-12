﻿using UnityEngine;
using System.Collections;

public enum Attack {
	leadingShot,
	spiral,
	beam,
	attack4
}

public class Bomb : MonoBehaviour {
	Player _owningPlayer = Player.none;       //Set this when creating a new Bomb
	public Player owningPlayer {
		get {
			return _owningPlayer;
		}
		set {
			_owningPlayer = value;
			if (value != Player.none) {
				spriteRenderer.sprite = playerBombSprites[(int)value];
            }
		}
	}
	public GameObject shockwavePrefab;
	public LeadingShot leadingShotPrefab;
	public SpiralShot spiralShotPrefab;
	public Beam beamShotPrefab;

	PhysicsObj physics;
	SpriteRenderer spriteRenderer;
	Sprite[] playerBombSprites = new Sprite[2];


	float decelerationRate = 0.005f;

	void Awake() {
		physics = GetComponent<PhysicsObj>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		playerBombSprites[0] = Resources.Load<Sprite>("Images/Bomb1");
		playerBombSprites[1] = Resources.Load<Sprite>("Images/Bomb2");
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

		transform.Rotate(new Vector3(0, 0, 180 * Time.deltaTime));
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
			case Attack.spiral:
				SpiralShot spiralShot = Instantiate(spiralShotPrefab, transform.position, new Quaternion()) as SpiralShot;
				spiralShot.owningPlayer = owningPlayer;
				spiralShot.FireBurst();
				break;
			case Attack.beam:
				Beam beamShot = Instantiate(beamShotPrefab, transform.position, new Quaternion()) as Beam;
				beamShot.owningPlayer = owningPlayer;
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
