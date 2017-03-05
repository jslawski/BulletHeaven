using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmShip : PlayerShip {
	KeyCode X, Y;

	new void Awake() {
		base.Awake();

		maxHealth = 180f;
	}

	// Use this for initialization
	new void Start () {
		base.Start();

		X = (playerEnum == PlayerEnum.player1) ? KeyCode.Alpha3 : KeyCode.Keypad3;
		Y = (playerEnum == PlayerEnum.player1) ? KeyCode.Alpha4 : KeyCode.Keypad4;

		GetComponentInChildren<ButtonHelpUI>().SetButtons(false, false, true, true);

		health = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
