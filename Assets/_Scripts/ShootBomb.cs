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
	public Ship thisShip;
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

	public KeyCode shootBomb, A, B, X, Y;

	// Use this for initialization
	void Awake () {
		thisShip = GetComponent<Ship>();
	}

	void Start() {
		//SetBombType(thisPlayer.typeOfShip);
	}
	
	public void SetBombType(CharactersEnum shipType) {
		//Attach the correct bomb prefab depending on the ship type
		switch (shipType) {
			case CharactersEnum.generalist:
				bombPrefab = Resources.Load<GameObject>("Prefabs/GeneralistBomb");
				break;
			case CharactersEnum.masochist:
				bombPrefab = Resources.Load<GameObject>("Prefabs/MasochistBomb");
				break;
			case CharactersEnum.vampire:
				bombPrefab = Resources.Load<GameObject>("Prefabs/VampireBomb");
				break;
			case CharactersEnum.tank:
				bombPrefab = Resources.Load<GameObject>("Prefabs/TankBomb");
				break;
			case CharactersEnum.glassCannon:
				bombPrefab = Resources.Load<GameObject>("Prefabs/GlassCannonBomb");
				break;
			default:
				Debug.LogError("Ship type " + thisShip.character.characterType + " is not defined!");
				break;
		}
	}

	// Update is called once per frame
	void Update () {
		if (shootingDisabled || GameManager.S.gameState != GameStates.playing) {
			return;
		}
		if (thisShip.player.device == null) {
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
				if (thisShip.player.device.RightBumper.WasPressed && bombShootCooldownRemaining <= 0) {
					Shoot();
				}
			}
			//Normal controls
			else if (thisShip.player.device.RightTrigger.WasPressed && bombShootCooldownRemaining <= 0) {
				Shoot();
			}

			//Detonating bombs
			if (thisShip.player.device.Action1.WasPressed) {
				DetonateBomb(AttackButtons.A);
			}
			else if (thisShip.player.device.Action2.WasPressed) {
				DetonateBomb(AttackButtons.B);
			}
			else if (thisShip.player.device.Action3.WasPressed) {
				DetonateBomb(AttackButtons.X);
			}
			else if (thisShip.player.device.Action4.WasPressed) {
				DetonateBomb(AttackButtons.Y);
			}
		}

		if (bombShootCooldownRemaining > 0) {
			bombShootCooldownRemaining -= Time.deltaTime;
		}
	}

	public void Shoot() {
		//Don't fire if we already have the max number of bombs in the air
		//if (bombsInAir.Count >= maxNumBombsInAir) {
		//	return;
		//}

		//Don't fire if we are out of ammo
		if (curAmmo == 0) {
			SoundManager.instance.Play("OutOfAmmo", 1);
			return;
		}

		if (GameManager.S.inGame) {
			SoundManager.instance.Play("FireBomb");
		}

		GameObject newBombGO = Instantiate(bombPrefab, transform.position, new Quaternion()) as GameObject;
		Bomb newBomb = newBombGO.GetComponentInChildren<Bomb>();
		PhysicsObj bombPhysics = newBomb.GetComponentInParent<PhysicsObj>();

		//Set the owner of the fired bomb
		newBomb.owningPlayer = thisShip.player.playerEnum;

		//Set the initial speed of the fired bomb
		float speed = Random.Range(minSpeed, maxSpeed);
		Vector3 aimDirection = ApplySpread(transform.up, bombSpread);
		Vector3 momentumVector = thisShip.movement.GetVelocity();
		momentumVector.x *= momentumInfluenceX;
		momentumVector.y *= momentumInfluenceY;
		bombPhysics.velocity = speed*aimDirection + momentumVector;

		//Add this bomb to the end of the queue
		bombsInAir.Add(newBomb);
		
		ExpendAttackSlot();

	}

	//Executing any attack results in expending an ammo slot
	public void ExpendAttackSlot() {
		if (curAmmo == 0) {
			SoundManager.instance.Play("OutOfAmmo", 1);
			return;
		}
		bombShootCooldownRemaining = bombShootCooldown;


		if (GameManager.S.inGame) {
			curAmmo--;
			StartCoroutine(ReloadBomb());
			//Disable the correct ammo image
			if (thisShip.player.ammoImages[0].reloading == false) {
				StartCoroutine(thisShip.player.ammoImages[0].DisplayReloadCoroutine(reloadDuration));
			}
			else if (thisShip.player.ammoImages[1].reloading == false) {
				StartCoroutine(thisShip.player.ammoImages[1].DisplayReloadCoroutine(reloadDuration));
			}
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

	public void DestroyAllBombs() {
		foreach (var bomb in bombsInAir) {
			Destroy(bomb);
		}
		bombsInAir.Clear();
	}
}
