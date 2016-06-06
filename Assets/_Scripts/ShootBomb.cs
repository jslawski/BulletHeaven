using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ShootBomb : MonoBehaviour {
	GameObject bombPrefab;
	PlayerShip thisPlayer;
	public bool shootingDisabled = false;

	public List<Bomb> bombsInAir = new List<Bomb>();         //Bombs that have been fired but not detonated
	int maxNumBombsInAir = 2;

	float bombShootCooldown = 0.5f;
	float bombShootCooldownRemaining = 0f;
	float bombSpread = 0.1f;

	float minSpeed = 15f;
	float maxSpeed = 18;
	float momentumInfluenceX = 0.1f;
	float momentumInfluenceY = 0.2f;

	public float reloadDuration = 4f;
	public int curAmmo = 2;
	public Ammo[] ammoImages;

	public KeyCode shootBomb, shootLeadingShot, shootSpiral, shootBeam, shootReflector;

	// Use this for initialization
	void Start () {
		thisPlayer = GetComponent<PlayerShip>();
		//Attach the correct bomb prefab depending on the ship type
		switch (thisPlayer.typeOfShip) {
			case ShipType.generalist:
				bombPrefab = Resources.Load<GameObject>("Prefabs/GeneralistBomb");
				break;
			default:
				Debug.LogError("Ship type " + thisPlayer.typeOfShip + " is not defined!");
				break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (shootingDisabled || !GameManager.S.gameHasBegun) {
			return;
		}
		if (thisPlayer.device == null) {
			if (Input.GetKeyDown(shootBomb) && bombShootCooldownRemaining <= 0) {
				Shoot();
			}

			//Detonating bombs
			if (Input.GetKeyDown(shootLeadingShot)) {
				AttackManager.S.ExecuteAttack(thisPlayer, AttackButtons.A);
			}
			else if (Input.GetKeyDown(shootSpiral)) {
				AttackManager.S.ExecuteAttack(thisPlayer, AttackButtons.B);
			}
			else if (Input.GetKeyDown(shootBeam)) {
				AttackManager.S.ExecuteAttack(thisPlayer, AttackButtons.X);
			}
			else if (Input.GetKeyDown(shootReflector)) {
				AttackManager.S.ExecuteAttack(thisPlayer, AttackButtons.Y);
			}
		}
		//Controller input
		else {
			//Emergency bumper controls
			if (GameManager.emergencyBumperControls) {
				if (thisPlayer.device.RightBumper.WasPressed && bombShootCooldownRemaining <= 0) {
					Shoot();
				}
			}
			//Normal controls
			else if (thisPlayer.device.RightTrigger.WasPressed && bombShootCooldownRemaining <= 0) {
				Shoot();
			}

			//Detonating bombs
			if (thisPlayer.device.Action1.WasPressed) {
				AttackManager.S.ExecuteAttack(thisPlayer, AttackButtons.A);
			}
			else if (thisPlayer.device.Action2.WasPressed) {
				AttackManager.S.ExecuteAttack(thisPlayer, AttackButtons.B);
			}
			else if (thisPlayer.device.Action3.WasPressed) {
				AttackManager.S.ExecuteAttack(thisPlayer, AttackButtons.X);
			}
			else if (thisPlayer.device.Action4.WasPressed) {
				AttackManager.S.ExecuteAttack(thisPlayer, AttackButtons.Y);
			}
		}

		if (bombShootCooldownRemaining > 0) {
			bombShootCooldownRemaining -= Time.deltaTime;
		}
	}

	void Shoot() {
		//Don't fire if we already have the max number of bombs in the air
		//if (bombsInAir.Count >= maxNumBombsInAir) {
		//	return;
		//}

		//Don't fire if we are out of ammo
		if (curAmmo == 0) {
			return;
		}

		SoundManager.instance.Play("FireBomb");

		GameObject newBombGO = Instantiate(bombPrefab, transform.position, new Quaternion()) as GameObject;
		Bomb newBomb = newBombGO.GetComponent<Bomb>();
		PhysicsObj bombPhysics = newBomb.GetComponent<PhysicsObj>();

		//Set the owner of the fired bomb
		newBomb.owningPlayer = thisPlayer.player;

		//Set the initial speed of the fired bomb
		float speed = Random.Range(minSpeed, maxSpeed);
		Vector3 aimDirection = ApplySpread(transform.up, bombSpread);
		Vector3 momentumVector = thisPlayer.playerMovement.GetVelocity();
		momentumVector.x *= momentumInfluenceX;
		momentumVector.y *= momentumInfluenceY;
		bombPhysics.velocity = speed*aimDirection + momentumVector;

		//Add this bomb to the end of the queue
		bombsInAir.Add(newBomb);
		bombShootCooldownRemaining = bombShootCooldown;

		curAmmo--;
		StartCoroutine(ReloadBomb());
		//Disable the correct ammo image
		if (ammoImages[0].reloading == false) {
			StartCoroutine(ammoImages[0].DisplayReloadCoroutine(reloadDuration));
		}
		else if (ammoImages[1].reloading == false) {
			StartCoroutine(ammoImages[1].DisplayReloadCoroutine(reloadDuration));
		}

	}

	IEnumerator ReloadBomb() {
		yield return new WaitForSeconds(reloadDuration);
		curAmmo++;
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
