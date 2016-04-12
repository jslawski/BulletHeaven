using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShootBomb : MonoBehaviour {
	public GameObject bombPrefab;
	PlayerShip thisPlayer;

	public List<Bomb> bombsInAir = new List<Bomb>();         //Bombs that have been fired but not detonated
	int maxNumBombsInAir = 2;

	float bombShootCooldown = 0.5f;
	float bombShootCooldownRemaining = 0f;
	float bombSpread = 0.2f;

	float minSpeed = 10f;
	float maxSpeed = 20;

	public KeyCode shootBomb, shootLeadingShot, shootSpiral, shootBeam;

	// Use this for initialization
	void Start () {
		thisPlayer = GetComponent<PlayerShip>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(shootBomb) && bombShootCooldownRemaining <= 0) {
			Shoot();
		}

		//Detonating bombs
		if (Input.GetKeyDown(shootLeadingShot)) {
			DetonateBomb(Attack.leadingShot);
		}
		else if (Input.GetKeyDown(shootSpiral)) {
			DetonateBomb(Attack.spiral);
		}
		else if (Input.GetKeyDown(shootBeam)) {
			DetonateBomb(Attack.beam);
		}

		if (bombShootCooldownRemaining > 0) {
			bombShootCooldownRemaining -= Time.deltaTime;
		}
	}

	void Shoot() {
		//Don't fire if we already have the max number of bombs in the air
		if (bombsInAir.Count >= maxNumBombsInAir) {
			return;
		}

		GameObject newBombGO = Instantiate(bombPrefab, transform.position, new Quaternion()) as GameObject;
		Bomb newBomb = newBombGO.GetComponent<Bomb>();
		PhysicsObj bombPhysics = newBomb.GetComponent<PhysicsObj>();

		//Set the owner of the fired bomb
		newBomb.owningPlayer = thisPlayer.player;

		//Set the initial speed of the fired bomb
		float speed = Random.Range(minSpeed, maxSpeed);
		Vector3 aimDirection = ApplySpread(transform.up, bombSpread);
		bombPhysics.velocity = speed*aimDirection;

		//Add this bomb to the end of the queue
		bombsInAir.Add(newBomb);
		bombShootCooldownRemaining = bombShootCooldown;
	}

	public void DetonateBomb(Attack attackToPerform) {
		if (bombsInAir.Count == 0) {
			return;
		}
		bombsInAir[0].Detonate(attackToPerform);
	}

	public static Vector3 ApplySpread(Vector3 aim, float spread) {
		aim.y += Random.Range(-spread, spread);
		return aim;
	}
}
