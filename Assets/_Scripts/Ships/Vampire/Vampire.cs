using UnityEngine;
using System.Collections;

public class Vampire : Character {
	[HideInInspector]
	public VampireShip vShip;

    float lifeRegen = 2f;       //Health regained per second

    void Start() {
		GetComponentInChildren<ButtonHelpUI>().SetButtons(false, false, false, true);
	}

	// Use this for initialization
	protected override void Awake () {
		base.Awake();

		characterType = CharactersEnum.vampire;
		bulletShape = BulletShapes.triangle;
		vShip = ship as VampireShip;
	}

	void Update() {
		if (ship.shooting.shootingDisabled || GameManager.S.gameState != GameStates.playing) {
			return;
		}

		if (player.device != null) {
			//Activate a shield if the button was pressed
			if (player.device.Action4.WasPressed) {
				vShip.InstantiateShield();
			}
		}
		else if (player.device == null) {
			if (Input.GetKeyDown(Y)) {
				vShip.InstantiateShield();
			}
		}
	}

    private void FixedUpdate() {
        if (GameManager.S.gameState != GameStates.playing) {
            return;
        }

        if (this.dead == false) {
            this.vShip.health += this.lifeRegen * Time.fixedDeltaTime;
            if (this.vShip.health > this.vShip.maxHealth) {
                this.vShip.health = this.vShip.maxHealth;
            }
        }
    }

    public override Ship GetClosestShip(Vector3 location) {
		return ship;
	}
}
