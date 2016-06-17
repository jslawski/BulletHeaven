using UnityEngine;
using System.Collections;

public class Generalist : PlayerShip {
	void Awake() {
		base.Awake();
		typeOfShip = ShipType.generalist;
		GetComponentInChildren<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/GeneralistShip/GShip6");
		GetComponentInChildren<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(
			"Images/GeneralistShip/GShipAnimationController");
	}

	new void Start() {
		base.Start();

		maxHealth = 200f;
		health = maxHealth;

		GetComponentInChildren<ButtonHelpUI>().SetButtons(false, false, false, false);
	}
}
