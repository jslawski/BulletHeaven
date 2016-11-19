using UnityEngine;
using System.Collections;

public class VampireShip : PlayerShip {
	KeyCode Y;

	float lifeRegen = 2f;       //Health regained per second

	public VampireShield shield;
	public bool shieldUp = false;

	new void Start() {
		base.Start();

		Y = player == Player.player1 ? KeyCode.Alpha4 : KeyCode.Keypad4;

		GetComponentInChildren<ButtonHelpUI>().SetButtons(false, false, false, true);

		health = maxHealth;
	}

	// Use this for initialization
	new void Awake () {
		base.Awake();

		maxHealth = 110f;

		typeOfShip = ShipType.vampire;
		shield = Resources.Load<VampireShield>("Prefabs/VampireShield");
		GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/VampireShip/VampireShip6");
		GetComponentInChildren<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(
			"Images/VampireShip/VampireShipAnimationController");
	}
	
	public void PercentHealthCost(float percent, bool ofCurrentHealth=true) {
		//Subtract percent% of current health from the player
		if (ofCurrentHealth) {
			health -= percent * health;
		}
		//Subtract percent% of total health from the player
		else {
			health -= percent * maxHealth;
		}
	}

	void FixedUpdate() {
		foreach (var player in GameManager.S.players) {
			if (player.dead) {
				return;
			}
		}
		health += lifeRegen * Time.fixedDeltaTime;
		if (health > maxHealth) {
			health = maxHealth;
		}
	}

	new void Update() {
		base.Update();

		if (playerShooting.shootingDisabled || GameManager.S.gameState != GameStates.playing) {
			return;
		}

		if (device != null) {
			//Activate a shield if the button was pressed
			if (device.Action4.WasPressed) {
				if (playerShooting.curAmmo != 0 && !shieldUp) {
					VampireShield newShield = Instantiate(shield, transform.position, new Quaternion()) as VampireShield;
					newShield.transform.parent = gameObject.transform;
					newShield.thisPlayer = GetComponent<VampireShip>();
					newShield.owningPlayer = player;
					newShield.ActivateShield();
					playerShooting.ExpendAttackSlot();
				}
				else {
					SoundManager.instance.Play("OutOfAmmo", 1);
				}
			}
		}
		else if (device == null) {
			if (Input.GetKeyDown(Y)) {
				if (playerShooting.curAmmo != 0 && !shieldUp) {
					VampireShield newShield = Instantiate(shield, transform.position, new Quaternion()) as VampireShield;
					newShield.transform.parent = gameObject.transform;
					newShield.thisPlayer = GetComponent<VampireShip>();
					newShield.owningPlayer = player;
					newShield.ActivateShield();
					playerShooting.ExpendAttackSlot();
				}
				else {
					SoundManager.instance.Play("OutOfAmmo", 1);
				}
			}
		}
	}
}
