using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum AttackButtons {
	A,
	B,
	X,
	Y,
	none
}

//Enum of ALL of the attacks in the game
public enum Attack {
	leadingShot,
	spiral,
	beam,
	reflector
}

public class ShootBomb : MonoBehaviour {
	GameObject bombPrefab;
	public PlayerShip thisPlayer;
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

	public KeyCode shootBomb, A, B, X, Y;

	// Use this for initialization
	void Awake () {
		thisPlayer = GetComponent<PlayerShip>();
	}

	void Start() {
		SetBombType(thisPlayer.typeOfShip);
	}
	
	public void SetBombType(ShipType shipType) {
		//Attach the correct bomb prefab depending on the ship type
		switch (shipType) {
			case ShipType.generalist:
				bombPrefab = Resources.Load<GameObject>("Prefabs/GeneralistBomb");
				break;
			case ShipType.masochist:
				bombPrefab = Resources.Load<GameObject>("Prefabs/MasochistBomb");
				break;
			case ShipType.vampire:
				bombPrefab = Resources.Load<GameObject>("Prefabs/VampireBomb");
				break;
			case ShipType.tank:
				bombPrefab = Resources.Load<GameObject>("Prefabs/TankBomb");
				break;
			case ShipType.glassCannon:
				bombPrefab = Resources.Load<GameObject>("Prefabs/GlassCannonBomb");
				break;
			default:
				Debug.LogError("Ship type " + thisPlayer.typeOfShip + " is not defined!");
				break;
		}
	}

	// Update is called once per frame
	void Update () {
		if (shootingDisabled || GameManager.S.gameState != GameStates.playing) {
			return;
		}
		if (thisPlayer.device == null) {
			if (Input.GetKeyDown(shootBomb) && bombShootCooldownRemaining <= 0) {
				Shoot();
			}

			//Detonating bombs
			if (Input.GetKeyDown(A)) {
				DetonateBomb(AttackButtons.A);
			}
			else if (Input.GetKeyDown(B)) {
				DetonateBomb(AttackButtons.B);
			}
			else if (Input.GetKeyDown(X)) {
				DetonateBomb(AttackButtons.X);
			}
			else if (Input.GetKeyDown(Y)) {
				DetonateBomb(AttackButtons.Y);
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
				DetonateBomb(AttackButtons.A);
			}
			else if (thisPlayer.device.Action2.WasPressed) {
				DetonateBomb(AttackButtons.B);
			}
			else if (thisPlayer.device.Action3.WasPressed) {
				DetonateBomb(AttackButtons.X);
			}
			else if (thisPlayer.device.Action4.WasPressed) {
				DetonateBomb(AttackButtons.Y);
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
		ExpendAttackSlot();

	}

	//Executing any attack results in expending an ammo slot
	public void ExpendAttackSlot() {
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

	public void DetonateBomb(AttackButtons buttonPressed) {
		if (bombsInAir.Count == 0) {
			return;
		}
		bombsInAir[0].Detonate(buttonPressed);
	}

	public static Vector3 ApplySpread(Vector3 aim, float spread) {
		aim.y += Random.Range(-spread, spread);
		return aim;
	}
}
